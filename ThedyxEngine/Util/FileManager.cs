using System.Diagnostics;
using log4net;
using Newtonsoft.Json;
using System.Text;
using ThedyxEngine.Engine;
using ThedyxEngine.Engine.Managers;
using ThedyxEngine.Engine.Objects;

namespace ThedyxEngine.Util
{
    /**
     * \class FileManager
     * \brief Manages the file operations.
     *
     * The FileManager class provides methods for managing the file operations.
     */
    public static class FileManager {
        
        /**
         * \class SimulationData
         * \brief Structure of the simulation data
         */
        public class SimulationData {
            /** Metadata */
            public required Metadata Metadata { get; set; }
            /** List of materials */
            public required List<dynamic?> Materials { get; set; }
            /** List of objects */
            public required List<dynamic?>? Objects { get; set; }
        }
        
        /**
         * \class Metadata
         * \brief Metadata of the simulation data
         */
        public class Metadata {
            /** Version of the program */
            public required string Version { get; set; }
            /** Date of the simulation */
            public required string Date { get; set; }
            /** Time of the simulation */
            public required string Time { get; set; }
            /** Platform of the simulation */
            public required string Platform { get; set; }
            /** Min temperature color */
            public required int MinTemperatureColor { get; set; }
            /** Max temperature color */
            public required int MaxTemperatureColor { get; set; }
        }
        
        /** Logger */
        private static readonly ILog Log = LogManager.GetLogger(typeof(FileManager));
        /**
        * Save to file
        */
        public static byte[] GetSimulationRepresentation() {
            var simulationData = new SimulationData {
                Metadata = new Metadata {
                    Version = GlobalVariables.MajorVersion + "." + GlobalVariables.MinorVersion,
                    Date = DateTime.Now.ToString("yyyy-MM-dd"),
                    Time = DateTime.Now.ToString("HH:mm:ss"),
                    Platform = Environment.OSVersion.Platform.ToString(),
                    MinTemperatureColor = GlobalVariables.MinTemperatureColor,
                    MaxTemperatureColor = GlobalVariables.MaxTemperatureColor
                },
                Materials = MaterialManager.Materials
                    .Select(material => JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(material)))
                    .ToList(),

                Objects = Engine.Engine.EngineObjectsManager?.GetObjects()
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
            Log.Info("Info: Starting reading.");

            try {
                SimulationData? simulationData;
                
                // we do not know if the json is compressed or not, so we need to try both
                try {
                    // Decode from Base64
                    byte[] jsonBytes = Convert.FromBase64String(inputString);
                    string jsonOutput = Encoding.UTF8.GetString(jsonBytes);
                    simulationData = JsonConvert.DeserializeObject<SimulationData>(jsonOutput);
                    Debug.Assert(simulationData != null, nameof(simulationData) + " != null");
                    GlobalVariables.MinTemperatureColor = simulationData.Metadata.MinTemperatureColor;
                    GlobalVariables.MaxTemperatureColor = simulationData.Metadata.MaxTemperatureColor;
                }
                catch (Exception) {
                    simulationData = JsonConvert.DeserializeObject<SimulationData>(inputString);
                }
                
                if(simulationData == null) {
                    Log.Error("Error: Simulation data is null.");
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
                Log.Error("Error: " + e.Message);
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
