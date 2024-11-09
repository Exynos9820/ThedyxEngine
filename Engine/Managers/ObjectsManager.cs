using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThedyxEngine.UI;

namespace ThedyxEngine.Engine.Managers{
    /** 
     * \class ObjectsManager
     * \brief Manages the objects in the simulation.
     * 
     * The ObjectsManager class provides methods for managing the objects in the simulation.
     */
    public class ObjectsManager{
        private List<EngineObject> _objects = [];
        private object _engineLock;
        
        /**
         * Constructor
         * \param engineLock Lock object
         */
        public ObjectsManager(object engineLock) {
            _engineLock = engineLock;
        }


        /** 
         * Get all visible objects on the current Canvas
         * \param manager The canvas manager provides the current view context
         * \return List<EngineObject> of the visivle objects
         */
        public List<EngineObject> GetVisibleObjects(CanvasManager manager) {
            return _objects.Where(obj => obj.IsVisible(manager)).ToList();
        }

        /**
         * \param name Name of the object that need to be checked
         * \return True if name is unique
         */
        public bool IsNameAvailable(string name) {
            return !_objects.Any(obj => obj.Name == name);
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

        /// TODO: Implement the logic for this function
        /// Check if the position is available for the object, if not return false
        public  bool IsPositionAvailable(EngineObject obj) {
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
         * \returns List<EngineObject> All objects
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
         * Applies energy delta after the frame
         */
        public void AppplyEnergyDeltaObjects() {
            for (int i = 0; i < _objects.Count; i++) {
                List<GrainSquare> list = _objects[i].GetSquares();
                for (int j = 0; j < list.Count; j++) {
                    list[j].ApplyEnergyDelta();
                }
            }

        }

    }
}
