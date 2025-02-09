using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
        public class SimulationData {
            public Metadata Metadata { get; set; }
            public List<dynamic?> Materials { get; set; }
            public List<dynamic?> Objects { get; set; }
        }

        public class Metadata {
            public string Version { get; set; }
            public string Date { get; set; }
            public string Time { get; set; }
            public string Platform { get; set; }
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(FileManager));   // Logger
        /**
        * Save to file
        */
        public static byte[] GetSimulationRepresentation() {
            var simulationData = new FileManager.SimulationData {
                Metadata = new FileManager.Metadata {
                    Version = "0.1",
                    Date = DateTime.Now.ToString("yyyy-MM-dd"),
                    Time = DateTime.Now.ToString("HH:mm:ss"),
                    Platform = Environment.OSVersion.Platform.ToString()
                },
                Materials = MaterialManager.Materials
                    .Select(material => JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(material)))
                    .ToList(),

                Objects = Engine.Engine.EngineObjectsManager.GetObjects()
                    .Select(obj => JsonConvert.DeserializeObject<dynamic>(
                        obj.GetJsonRepresentation()
                    )).ToList()
            };

            string jsonOutput = JsonConvert.SerializeObject(simulationData, Formatting.Indented);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonOutput);


            if (GlobalVariables.SaveSimulationHumanReadable) {
                return jsonBytes;
            }
            // Encode to Base64
            string base64String = Convert.ToBase64String(jsonBytes);
            byte[] base64Bytes = Encoding.UTF8.GetBytes(base64String);

            return base64Bytes;
        }
        
        /**
         * Load simulation from json content
         * \param inputString input string
         */
        public static void LoadFromContent(string inputString) {
            Engine.Engine.ClearSimulation();
            MaterialManager.Materials.Clear();
            log.Info("Info: Starting reading.");

            try {
                SimulationData? simulationData;
                
                // we do not know if the json is compressed or not, so we need to try both
                try {
                    // Decode from Base64
                    byte[] jsonBytes = Convert.FromBase64String(inputString);
                    string jsonOutput = Encoding.UTF8.GetString(jsonBytes);
                    simulationData = JsonConvert.DeserializeObject<SimulationData>(jsonOutput);
                }
                catch (Exception e) {
                    simulationData = JsonConvert.DeserializeObject<SimulationData>(inputString);
                }
                
                if(simulationData == null) {
                    log.Error("Error: Simulation data is null.");
                    return;
                }
                
                // Check metadata
                if (simulationData.Metadata.Platform != "Unix") {
                    log.Error("Only MacOS is supported.");
                    return;
                }
                if (simulationData.Metadata.Version != "0.1") {
                    log.Error("Only version 0.1 is supported.");
                    return;
                }

                // Load Materials
                foreach (var material in simulationData.Materials) {
                    MaterialManager.Materials.Add(JsonConvert.DeserializeObject<Material>(JsonConvert.SerializeObject(material)));
                }

                // Load Objects
                LoadJsonObjects(simulationData.Objects);
            }
            catch (Exception e) {
                log.Error("Error: " + e.Message);
            }
        }

        
        /**
         * \brief Load JSON objects
         * \param jsonObjects The JSON objects to load
         * We need a custom deserialization because the objects are written in a custom way to save space.
         */
        private static void LoadJsonObjects(List<dynamic> jsonObjects) {
            foreach (var obj in jsonObjects) {
                var type = obj.Type.ToString();
                EngineObject engineObject = null;

                if (type == ObjectType.GrainSquare.ToString()) {
                    engineObject = JsonConverters.GrainSquareFromJson(JsonConvert.SerializeObject(obj));
                } else if (type == ObjectType.StateGrainSquare.ToString()) {
                    engineObject = JsonConverters.StateGrainSquareFromJson(JsonConvert.SerializeObject(obj));
                } else if (type == ObjectType.Rectangle.ToString()) {
                    engineObject = JsonConverters.RectangleFromJson(JsonConvert.SerializeObject(obj));
                } else if (type == ObjectType.StateRectangle.ToString()) {
                    engineObject = JsonConverters.StateRectangleFromJson(JsonConvert.SerializeObject(obj));
                }

                if (engineObject != null) {
                    Engine.Engine.EngineObjectsManager.AddObject(engineObject);
                }
            }
        }


    }
}
