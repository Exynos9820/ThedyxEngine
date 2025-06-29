using log4net;
using ThedyxEngine.Engine.Objects;

namespace ThedyxEngine.Engine.Managers {
    /**
     * \class OptimizationManager
     * \brief Manages the optimization of the engine
     *
     * The OptimizationManager class provides methods for optimizing the engine by setting adjacent sq quares to be touching
     */
    public static class ObjectsOptimizationManager {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ObjectsOptimizationManager));

        /**
         * Optimize objects
         * \param objects list of objects
         */
    public static void Optimize(List<EngineObject>? objects, int threads) {
        if (objects == null) return;    
        List<Task> tasks = new List<Task>();

        for (int i = 0; i < Math.Min(threads, objects.Count); i++) {
            int startPosition = i * objects.Count / threads;
            int endPosition = (i + 1) * objects.Count / threads;
            int threadIndex = i;

            tasks.Add(Task.Run(() => {
                ClearOptimization(objects, startPosition, endPosition);
                Engine.TasksResults[threadIndex] = 5;
                Log.Info($"Thread {threadIndex}: Cleared Optimization");

                OptimizeTouching(objects, startPosition, endPosition);
                Engine.TasksResults[threadIndex] = 40;
                Log.Info($"Thread {threadIndex}: Optimized Touching");

                RadiationOptimization.Optimize(objects, startPosition, endPosition);
                Engine.TasksResults[threadIndex] = 100;
                Log.Info($"Thread {threadIndex}: Radiation Optimized");
            }));
        }

        // Show progress bar before starting
        Engine.MainWindow.LoadingProgressBar.IsVisible = true;
        Engine.MainWindow.LoadingProgressBar.Progress = 0;

        // While there are tasks running, update the progress bar
        while (tasks.Count > 0) {
            // Calculate progress based on completed tasks
            int sum = 0;
            foreach (var task in tasks) {
                sum += Engine.TasksResults[tasks.IndexOf(task)];
            }
            float progressValue = sum / (threads/ 100.0f);

            // Update the progress bar on UI thread
            Engine.MainWindow.LoadingProgressBar.Progress = progressValue;

            // Remove completed tasks
            tasks.RemoveAll(t => t.IsCompleted);
        }

        // Ensure progress bar is fully completed before hiding
        MainThread.BeginInvokeOnMainThread(() => {
            Engine.MainWindow.LoadingProgressBar.Progress = 1.0;
            Engine.MainWindow.LoadingProgressBar.IsVisible = false;
        });
    }



        /**
         * Fill adjacent squares for two lists of squares
         * \param squares1 first list of squares
         * \param squares2 second list of squares
         */
        private static void FillAdjacentSquares(List<GrainSquare> squares1, List<GrainSquare> squares2) {
            foreach(var square1 in squares1) {
                foreach(var square2 in squares2) {
                    if (square1.AreTouching(square2)) {
                        square1.AddAdjacentSquare(square2);
                        square1.AddRadiationExchangeSquare(square2);
                        square2.AddAdjacentSquare(square1);
                        square2.AddRadiationExchangeSquare(square1);
                    }
                }
            }
        }

        /**
         * Optimize touching objects, by setting adjacent squares for every square of an object
         * \param objects list of objects
         */
        private static void OptimizeTouching(List<EngineObject>? objects, int startPosition, int endPosition) {
            for (int i = startPosition; i < endPosition; i++) {
                List<GrainSquare> firstExternal = objects[i].GetExternalSquares();
                FillAdjacentSquares(objects[i].GetSquares(), objects[i].GetSquares());

                for (int j = 0; j < objects.Count; j++) {
                    if (i == j) continue;
                    List<GrainSquare> secondExternal = objects[j].GetExternalSquares();
                    FillAdjacentSquares(firstExternal, secondExternal);
                }
            }
        }

        /**
         * Clear optimization
         * \param objects list of objects
         */
        private static void ClearOptimization(List<EngineObject>? objects, int startPosition, int endPosition) {
            if(objects == null) return;
            for (int i = startPosition; i < endPosition; i++) {
                List<GrainSquare> squares = objects[i].GetSquares();
                foreach (var square in squares) {
                    // clear adjacent and radiation squares
                    square.ClearOptimizationSquares();
                }
            }
        }
    }
}