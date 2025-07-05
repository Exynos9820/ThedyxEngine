using log4net;

namespace ThedyxEngine.Engine.Managers {
    /**
     * \class ObjectsOptimizer
     * \brief Manages the optimization of the Engine
     *
     * The ObjectsOptimizer class provides methods for optimizing the engine by setting adjacent sq quares to be touching
     */
    public static class ObjectsOptimizer {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ObjectsOptimizer));

        /**
         * Optimize objects
         * \param objects list of objects
         */
        public static  void Optimize(List<EngineObject> objects, int threads) {
            List<Task> tasks = new List<Task>();
            List<IOptimizer> optimizers = new List<IOptimizer>();
            optimizers.Add(new ClearingOptimizer());
            optimizers.Add(new NeighborsOptimizer());
            optimizers.Add(new RadiationOptimizer());
            for (int i = 0; i < Math.Min(threads, objects.Count); i++) {
                int startPosition = i * objects.Count / threads;
                int endPosition = (i + 1) * objects.Count / threads;
                int threadIndex = i;

                tasks.Add(Task.Run(() => {
                    foreach (var optimizer in optimizers) {   
                        optimizer.Optimize(objects, startPosition, endPosition);
                        Log.Info($"Thread {threadIndex}: {optimizer.GetName()} Optimized");
                    }
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
                float progressValue = (float)sum / (threads/ 100.0f);

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
            Task.WaitAll(tasks.ToArray());
        }
    }
}