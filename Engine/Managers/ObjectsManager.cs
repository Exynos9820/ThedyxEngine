using ThedyxEngine.Engine.Objects;
using ThedyxEngine.UI;

namespace ThedyxEngine.Engine.Managers{
    /** 
     * \class ObjectsManager
     * \brief Manages the objects in the simulation.
     * 
     * The ObjectsManager class provides methods for managing the objects in the simulation.
     */
    public class ObjectsManager{
        /**
         * List of all objects
         */
        private List<EngineObject> _objects = [];
        /**
         * engine lock for object manager
         */
        private object _engineLock;
        /**
         * Smallest temperature for a color scale
         */
        public double SmallestTemperature = -1000;
        /**
         * Biggest temperature for a color scale
         */
        public double LargestTemperature = -1000;
        /**
         * Constructor
         * \param engineLock Lock object
         */
        public ObjectsManager(object engineLock) {
            _engineLock = engineLock;
        }
        
        /**
         * Update the smallest and biggest temperature of the objects
         */
        public void UpdateSmallestAndBiggestTemperature() {
            foreach (var o in _objects) {
                foreach(var sq in o.GetSquares()) {
                    if (sq.SimulationTemperature < SmallestTemperature) {
                        SmallestTemperature = sq.SimulationTemperature;
                    }
                    if (sq.SimulationTemperature > LargestTemperature) {
                        LargestTemperature = sq.SimulationTemperature;
                    }
                }
            } 
        }


        /** 
         * Get all visible objects on the current Canvas
         * \param manager The canvas manager provides the current view context
         * \return List<EngineObject> of the visible objects
         */
        public List<EngineObject> GetVisibleObjects(CanvasManager manager) {
            return _objects.Where(obj => obj.IsVisible(manager)).ToList();
        }

        /**
         * \param name Name of the object that need to be checked
         * \return True if name is unique
         */
        public bool IsNameAvailable(string? name) {
            return _objects.All(obj => obj.Name != name);
        }


        /**
         * Add object to the simulation
         * \param obj EngineObject that need to be added
         */
        public void AddObject(EngineObject obj) {
            lock (_engineLock) {
                if (!IsNameAvailable(obj.Name)) throw new InvalidOperationException("Object name is not available");
                _objects.Add(obj);
            }
        }

        
        /**
         * Generate a unique name for the object
         * \return Unique name
         */
        public string GenerateUniqueName() {
            string name = "Object";
            int i = 1;
            while (!IsNameAvailable(name)) {
                name = "Object" + i;
                i++;
            }
            return name;
        }

        /**
         * Check if the position is available for the object
         * \param obj EngineObject that need to be checked
         */
        public  bool IsPositionAvailable(EngineObject obj) {
            // check intersection with other objects
            foreach (var ob in _objects) {
                if (ob == obj) continue;
                    if (ob.IsIntersecting(obj)) return false;
            }
            return true;
        }

        /** 
         * Remove object from the simulation
         * \param obj EngineObject that need to be removed
         */
        public void RemoveObject(EngineObject obj) {
            lock (_engineLock) {
                _objects.Remove(obj);
            }
        }

        /** 
         * Get all objects of the simulation
         * \returns List of EngineObject All objects
         */
        public List<EngineObject> GetObjects() {
            return _objects;
        }

        /**
         * Get object by it's index
         * \param index Index of the object
         * \return EngineObject 
         */
        public EngineObject GetObjectByIndex(int index) {
            if (index < 0 || index >= _objects.Count) throw new InvalidOperationException("Index out of bounds");
            return _objects[index];
        }

       /**
        * Delete all objects 
        */
        public void ClearObjects() {
            lock (_engineLock) {
                _objects.Clear();
            }
        }


        /** 
         * Reset the temperature of all objects
         */
        public void ResetObjectsTemperature() {
            foreach (var obj in _objects) {
                obj.SetStartTemperature();
            }
        }

        /**
         * Apply the energy delta for the selected objects
         * \param objects List of objects
         */
        public void ApplyEnergyDelta(List<EngineObject> objects) {
            foreach (var obj in objects) {
                obj.ApplyEnergyDelta();
            }
        }

        /**
         * Applies energy delta after the frame
         */
        public void ApplyEnergyDeltaObjects() {
            for (int i = 0; i < _objects.Count; i++) {
                List<GrainSquare> list = _objects[i].GetSquares();
                for (int j = 0; j < list.Count; j++) {
                    list[j].ApplyEnergyDelta();
                }
            }

        }

    }
}
