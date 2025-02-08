using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ThedyxEngine.Engine;
using LukeMauiFilePicker;

namespace ThedyxEngine.Util
{
    /**
     * \class FileManager
     * \brief Manages the file operations.
     *
     * The FileManager class provides methods for managing the file operations.
     */
    public static class FileManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FileManager));   // Logger
        /**
        * Save to file
        * \param path Path to save the file
        */
        public static string GetObjectsJsonRepresentation() {
            List<EngineObject> engineObjects = Engine.Engine.EngineObjectsManager.GetObjects();
            List<string> jsonObjects = new List<string>();

            log.Info("Info: Starting saving.");

            try {
                foreach (var obj in engineObjects) {
                    string json = obj.GetJsonRepresentation();
                    jsonObjects.Add(json);
                }
            }catch(Exception e) {
                log.Error("Error: " + e.Message);
            }

            string jsonOutput = JsonConvert.SerializeObject(jsonObjects, Formatting.Indented);
            return jsonOutput;
        }
        
        
        public static void LoadFromContent(string jsonInput) {
            Engine.Engine.ClearSimulation();
            log.Info("Info: Starting reading.");

            try {
                List<string> jsonObjects = JsonConvert.DeserializeObject<List<string>>(jsonInput);
                List<EngineObject> engineObjects = new List<EngineObject>();

                foreach (var json in jsonObjects) {
                    var jObject = JsonConvert.DeserializeObject<dynamic>(json);
                    EngineObject engineObject = null;

                    string type = jObject["Type"].Value;
                    if (type == ObjectType.GrainSquare.ToString()) {
                        engineObject = EngineObjectsFactory.GrainSquareFromJson(json);
                    }else if (type == ObjectType.StateGrainSquare.ToString()) {
                        engineObject = EngineObjectsFactory.StateGrainSquareFromJson(json);
                    }else if (type == ObjectType.Rectangle.ToString()) {
                        engineObject = EngineObjectsFactory.RectangleFromJson(json);
                    }else if (type == ObjectType.StateRectangle.ToString()) {
                        engineObject = EngineObjectsFactory.StateRectangleFromJson(json);
                    }

                    if (engineObject != null) {
                        engineObjects.Add(engineObject);
                        Engine.Engine.EngineObjectsManager.AddObject(engineObject);
                    }
                }
            }
            catch (Exception e) {
                log.Error("Error: " + e.Message);
            }
        }

    }
}
