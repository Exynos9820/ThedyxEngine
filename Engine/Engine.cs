using log4net;
using System.Collections.Concurrent;
using ThedyxEngine.Util;
using ThedyxEngine.Engine.Managers;
using System.Diagnostics;

namespace ThedyxEngine.Engine{
    static class Engine{
        /**
        * \enum EngineMode
        * \brief Engine mode
        */  
        public enum EngineMode {
            Stopped,
            Running,
            Paused
        }
        
        /** Do we want to show the temperature */
        public static bool ShowTemperature = false;
        /** Do we want to show the grid */
        public static bool ShowGrid = true;
        /** Do we want to show the color */
        public static bool ShowColor = false;
        
        /** Logger for the engine */
        private static readonly ILog Log = LogManager.GetLogger(typeof(Engine));
        /** Lock for the engine */
        private static object? _engineLock; 
        /** Manager for the engine objects */
        public static ObjectsManager? EngineObjectsManager;
        /** Main window */
        public static MainPage? _mainWindow;
        /** Engine thread */
        private static TempoThread? _engineThread;
        /** Frames counter */
        private static int _frames = 0;
        /** Simulation refresh rate, per second */
        private static int _simulationRefreshRate = 60;
        /** Action to Show an error message */
        public static Action<string>? ShowErrorMessage;

        /** Time of the simulation in microseconds */
        private static long _simulationTime = 0;
        /** Should we optimize the engine by setting adjacent squares to be touching */
        private static bool _optimize = true;
        /** Current Engine mode */
        public static EngineMode Mode { get; private set; } = EngineMode.Stopped; // engine mode
        /** Variable to store the biggest results of tasks */
        public static ConcurrentDictionary<int, int> TasksResults = new();

        /**
         * \brief Initialize the engine
         */
        public static void Init(MainPage window){
            _engineLock = new object();
            _mainWindow = window;
            EngineObjectsManager = new ObjectsManager(_engineLock);
            MaterialManager.Init();
            // load kettle copper example
            /*try {
                using var stream = File.OpenRead("../../../../../Examples/liveDemo.tdx");
                using var reader = new StreamReader(stream);
                string fileContent = reader.ReadToEnd(); // Sync version
                FileManager.LoadFromContent(fileContent);
            }
            catch (Exception ex) {
                Log.Error("Problem with loading file: " + ex.Message);
            }*/

            
            
            _simulationRefreshRate = Util.SystemInfo.GetRefreshRate();
            Log.Info("Engine initialized");
        }

        /*
         * *
         * \brief Start the engine
         */
        public static void Start() {
            if(_engineLock == null)         throw new InvalidOperationException("Engine lock is not initialized");
            lock (_engineLock) {
                Log.Info("Engine started");
                if (Mode != EngineMode.Paused) {
                    PrepareObjects();
                    _frames = 0;
                }
                Mode = EngineMode.Running;
                _engineThread = new TempoThread("EngineThread", Run);
                _engineThread.SetPriority(ThreadPriority.Highest);
                _engineThread.Start();
            }
        }
        
        


        /**
         * \brief Prepare objects for the simulation before starting it
         */
        private static void PrepareObjects() {
            if(_engineLock == null) 
                throw new InvalidOperationException("Engine lock is not initialized");

            int threads = Math.Max(Environment.ProcessorCount - 2, 1);
            // zero the results with the number of threads
            TasksResults = new ConcurrentDictionary<int, int>();
            for (int i = 0; i < threads; i++) {
                TasksResults[i] = 0;
            }
            
            // Show progress bar before starting
            MainThread.BeginInvokeOnMainThread(() => _mainWindow.LoadingProgressBar.IsVisible = false);


            lock (_engineLock) {
                EngineObjectsManager.ResetObjectsTemperature();
                if (_optimize) {
                    ObjectsOptimizationManager.Optimize(EngineObjectsManager.GetObjects(),
                        threads);
                }
            }
            
            
            Log.Info("Preparation done");
        }


        /**
         * 
         * \brief Run the engine
         */
        public static void Run() {
            if(_engineLock == null)         throw new InvalidOperationException("Engine lock is not initialized");
            
            // if we have more than 2 cores, use all but 2
            int coresToUse = Environment.ProcessorCount - 2 > 0 ? Environment.ProcessorCount - 2 : 1;
            // now we need to divide all the work between the cores
            // by dividing all the objects between the cores
            // Create a list for each core
            List<List<EngineObject>> objectsToProcess = new List<List<EngineObject>>();
            for (int i = 0; i < coresToUse; i++) {
                objectsToProcess.Add(new List<EngineObject>());
            }

            // Distribute objects round-robin style
            List<EngineObject> allObjects = EngineObjectsManager.GetObjects();
            for (int i = 0; i < allObjects.Count; i++) {
                int coreIndex = i % coresToUse;
                objectsToProcess[coreIndex].Add(allObjects[i]);
            }
                
            // each task will process it's own list of objects
            // then wait for all tasks to process one frame
            // then they will apply energy delta for it's objects
            // wait for all tasls to apply energy delta
            // then start the next frame
            Stopwatch stopwatch = new Stopwatch();

            while (true) {
                stopwatch.Restart();
                double msPerFrame = 1000.0 / (double)_simulationRefreshRate;  // milliseconds per frame

                lock (_engineLock) {
                    if (Mode != EngineMode.Running) break;
                }
                List<Task> tasks = new List<Task>();
                // update logic
                // create tasks
                try {
                    for (int i = 0; i < coresToUse; i++) {
                        var i1 = i;
                        tasks.Add(Task.Run(() => {
                            if (i1 < objectsToProcess.Count) {
                                RadiationTransferManager.TransferRadiationHeat(objectsToProcess[i1]);
                                ConductionTransferManager.TransferConductionHeat(objectsToProcess[i1]);
                                ConvectionTransferManager.TransferConvectionHeat(objectsToProcess[i1]);
                            }
                        }));
                    }
                    // wait for all tasks to finish
                    Task.WaitAll(tasks.ToArray());
                    // apply energy delta
                    tasks = new List<Task>();
                    for(int i = 0; i < coresToUse; i++) {
                        var i1 = i;
                        tasks.Add(Task.Run(() => {
                            if (i1 < objectsToProcess.Count)
                                EngineObjectsManager.ApplyEnergyDelta(objectsToProcess[i1]);
                        }));
                    }
                    // wait for all tasks to finish
                    Task.WaitAll(tasks.ToArray());
                    EngineObjectsManager.UpdateSmallestAndBiggestTemperature();
                }
                catch (Exception e) {
                    ShowErrorMessage?.Invoke($"Engine failed to run with {e.Message}. Check the parameters of the materials and objects");
                    break;
                }


                double elapsedTimeMs = stopwatch.ElapsedMilliseconds;
                if (msPerFrame - elapsedTimeMs < 0) {
                    Log.Info($"Simulation is too slow, time is {elapsedTimeMs - msPerFrame} ms");
                }
                

                while (GlobalVariables.WaitToBeInTime && stopwatch.ElapsedMilliseconds < 1000 / GlobalVariables.EngineIntervalUpdatePerSecond) { }
                stopwatch.Stop();

                lock (_engineLock) {
                    _frames++;
                    _simulationTime = (int)(_frames * 1000 / GlobalVariables.EngineIntervalUpdatePerSecond);
                }
            }
        }

        /**
         * \brief Get the engine mode
         * \return EngineMode Current state of the engine
         */
        public static EngineMode GetMode() {
            if (_engineLock == null)        throw new InvalidOperationException("Engine lock is not initialized");
            lock (_engineLock) {
                return Mode;
            }
        }

        /**
         * \brief Get the simulation time
         * \return long Time in milliseconds
         */
        public static long GetSimulationTime() {
            if (_engineLock == null)        throw new InvalidOperationException("Engine lock is not initialized");
            return _simulationTime;
        }

        /**
         * \brief Stop the engine
         */
        public static void Stop() {
            if (_engineLock == null)        throw new InvalidOperationException("Engine lock is not initialized");
            lock (_engineLock) {
                Log.Info("Engine stopped");
                _simulationTime = 0;
                Mode = EngineMode.Stopped;
            }
        }

        /** 
         * \brief Pause the engine
         */
        public static void Pause() {
            if (_engineLock == null)        throw new InvalidOperationException("Engine lock is not initialized");
            lock (_engineLock) {
                Log.Info("Engine paused");
                Mode = EngineMode.Paused;
            }
        }

        /**
         * \brief Check if engine is running
         * \return bool True if engine is running
         */
        public static bool IsRunning() {
            if (_engineLock == null)        throw new InvalidOperationException("Engine lock is not initialized");
            lock (_engineLock) {
                return Mode == EngineMode.Running;
            }
        }


        /**
         * \brief Reset the simulation
         */
        public static void ResetSimulation() {
            if (Mode == EngineMode.Running) throw new InvalidOperationException("Cannot reset simulation while running");

            EngineObjectsManager.ResetObjectsTemperature();
            _simulationTime = 0;
            Log.Info("Simulation reset");
            _mainWindow.UpdateAll();
        }

        /**
         * \brief Clear the simulation
         */
        public static void ClearSimulation() {
            if (_engineLock == null)        throw new InvalidOperationException("Engine lock is not initialized");

            lock (_engineLock) {
                EngineObjectsManager.ClearObjects();
            }
            Log.Info("All objects cleared");
            _mainWindow.UpdateAll();
        }

    }
}
