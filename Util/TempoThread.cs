using log4net;

namespace ThedyxEngine.Util {
    /**
     * \class TempoThread
     * \brief Represents a thread.
     * 
     * The TempoThread class provides methods for managing a thread.
     */
    internal class TempoThread {
        private readonly Thread _thread;  /// Thread
        public readonly string Name;      /// Name of the thread
        private static readonly ILog log = LogManager.GetLogger(typeof(TempoThread)); /// Logger

        /**
         * \brief Initializes a new instance of the TempoThread class.
         * \param name Name of the thread.
         * \param start Start method.
         */
        public TempoThread(string name, ThreadStart start) {
            Name = name;
            _thread = new Thread(start) {
                Name = name
            };
        }

        /**
         * \brief Starts the thread.
         */
        public void Start() {
            _thread.Start();
            log.Info("Thread " + Name + " started");
        }

        /**
         * \brief Joins the thread.
         */
        public void Join() {
            _thread.Join();
            log.Info("Thread " + Name + " joined");
        }

        /**
         * \brief Sets the priority of the thread.
         */
        public void SetPriority(ThreadPriority priority) {
            _thread.Priority = priority;
        }

        /**
         * \brief ABort Thread
         * Doesn
         */
        public void Abort() {
            _thread.Abort();
        }
    }
}
