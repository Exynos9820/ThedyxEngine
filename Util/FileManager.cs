using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ThedyxEngine.Engine;
using LukeMauiFilePicker;
using ThedyxEngine.Engine.Managers;

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
        public static string GetSimulationRepresentation() {
            //  first line is version, date, time,platform(macos, windows, linux)
            string version = GlobalVariables.MajorVersion + "." + GlobalVariables.MinorVersion;
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string time = DateTime.Now.ToString("HH:mm:ss");
            string platform = Environment.OSVersion.Platform.ToString();
            string firstLine = version + "," + date + "," + time + "," + platform;
            List<EngineObject> engineObjects = Engine.Engine.EngineObjectsManager.GetObjects();
            List<string> jsonObjects = new List<string>();

            try {
                log.Info("Info: Starting saving Materials.");
                foreach (var material in MaterialManager.Materials) {
                    string json = material.ToJson();
                    jsonObjects.Add(json);
                }
                
                log.Info("Info: Starting saving Objects.");
                foreach (var obj in engineObjects) {
                    string json = obj.GetJsonRepresentation();
                    jsonObjects.Add(json);
                }
            }catch(Exception e) {
                log.Error("Error: " + e.Message);
            }

            string jsonOutput = JsonConvert.SerializeObject(jsonObjects, Formatting.Indented);
            return firstLine + "\n" + jsonOutput;
        }
        
        
        public static void LoadFromContent(string simulationStringInput) {
            Engine.Engine.ClearSimulation();
            MaterialManager.Materials.Clear();
            log.Info("Info: Starting reading.");
            
            try {
                // first line is version, date, time,platform(macos, windows, linux)
                // check if platform is MacOs
                if (!simulationStringInput.Contains("Unix")) {
                    log.Error("Only MacOS is supported.");
                    return;
                }
                // check if the version is 0.1
                if (!simulationStringInput.Contains("0.1")) {
                    log.Error("Only version 0.1 is supported.");
                    return;
                }
                
                // remove the first line
                simulationStringInput = simulationStringInput.Substring(simulationStringInput.IndexOf('\n') + 1);
                List<string> jsonObjects = JsonConvert.DeserializeObject<List<string>>(simulationStringInput);
                LoadJsonObjects(jsonObjects);                
            }
            catch (Exception e) {
                log.Error("Error: " + e.Message);
            }
        }
        
        private static void LoadJsonObjects(List<string> jsonObjects) {
            List<EngineObject> engineObjects = new List<EngineObject>();

            foreach (var json in jsonObjects) {
                var jObject = JsonConvert.DeserializeObject<dynamic>(json);
                EngineObject engineObject = null;
                string type = jObject["Type"].Value;
                if (type == ObjectType.GrainSquare.ToString()) {
                    engineObject = JsonConverters.GrainSquareFromJson(json);
                }else if (type == ObjectType.StateGrainSquare.ToString()) {
                    engineObject = JsonConverters.StateGrainSquareFromJson(json);
                }else if (type == ObjectType.Rectangle.ToString()) {
                    engineObject = JsonConverters.RectangleFromJson(json);
                }else if (type == ObjectType.StateRectangle.ToString()) {
                    engineObject = JsonConverters.StateRectangleFromJson(json);
                }else if (type == "Material") {
                    Material material = JsonConverters.MaterialFromJson(json);
                    MaterialManager.Materials.Add(material);
                }

                if (engineObject != null) {
                    engineObjects.Add(engineObject);
                    Engine.Engine.EngineObjectsManager.AddObject(engineObject);
                }
            }
        }

    }
}
