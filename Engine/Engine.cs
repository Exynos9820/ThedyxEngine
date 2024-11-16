using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThedyxEngine.UI;
using ThedyxEngine.Util;
using ThedyxEngine.Engine.Managers;
using ThedyxEngine.Engine.Examples;
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

        private static readonly ILog Log = LogManager.GetLogger(typeof(Engine)); // log4net logger
        private static object?          _engineLock; // lock for the engine
        public static ObjectsManager EngineObjectsManager = new(_engineLock); // manager for the engine objects
        private static MainPage?      _mainWindow; // main window
        private static TempoThread?     _engineThread; // engine thread
        private static long             _lastUpdateTime = 0; // last update time
        static int _frames = 0;     // frames counter
        private static int _simulationRefreshRate = 60; // updates per second 
        private static long _simulationTime = 0; // time of the simulation in microseconds
        private static bool _optimize = true; // // should we optimize the engine by setting adjacent squares to be touching
        public static EngineMode Mode { get; private set; } = EngineMode.Stopped; // engine mode
        public const double AirTemperature = 293; // air temperature in Kelvin

        /**
         * \brief Initialize the engine
         */
        public static void Init(MainPage window){
            _engineLock = new object();
            _mainWindow = window;
            EngineObjectsManager = new ObjectsManager(_engineLock);
            MaterialManager.Init();
            //SimpleExamples.RectangleWithTempDifference(15, 15);
            SimpleExamples.TwoEngineRectangles();
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
                PrepareObjects();
                Mode = EngineMode.Running;
                _frames = 0;
                _engineThread = new TempoThread("EngineThread", Run);
                _engineThread.SetPriority(ThreadPriority.Highest);
                _engineThread.Start();
            }
        }


        /**
         * \brief Prepare objects for the simulation before starting it
         */
        private static void PrepareObjects() {
            if(_engineLock == null)         throw new InvalidOperationException("Engine lock is not initialized");

            lock (_engineLock) {
                EngineObjectsManager.ResetObjectsTemperature();
                if (_optimize) {
                    ObjectsOptimizationManager.Optimize(EngineObjectsManager.GetObjects());
                }
            }
        }

        /**
         * 
         * \brief Run the engine
         */
        public static void Run() {
            if(_engineLock == null)         throw new InvalidOperationException("Engine lock is not initialized");

            Stopwatch stopwatch = new Stopwatch();

            while (true) {
                stopwatch.Restart();
                double msPerFrame = 1000.0 / (double)_simulationRefreshRate;  // milliseconds per frame

                lock (_engineLock) {
                    if (Mode != EngineMode.Running) break;
                }
                // update logic

                // simplify the logic for now
                RadiationTransferManager.TransferRadiationHeat(EngineObjectsManager.GetObjects());
                ConductionTransferManager.TransferConductionHeat(EngineObjectsManager.GetObjects());
                EngineObjectsManager.AppplyEnergyDeltaObjects();
                

                double elapsedTimeMs = stopwatch.ElapsedMilliseconds;
                if (msPerFrame - elapsedTimeMs < 0) {
                    Log.Info($"Simulation is too slow, time is {elapsedTimeMs - msPerFrame} ms");
                }
                

                while (stopwatch.ElapsedMilliseconds < Const.EngineIntervalUpdate) { }
                stopwatch.Stop();

                lock (_engineLock) {
                    _frames++;
                    _simulationTime = (int)(_frames * Const.EngineIntervalUpdate);
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
