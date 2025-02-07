using Newtonsoft.Json;
using ThedyxEngine.Engine;
using ThedyxEngine.Engine.Managers;

namespace ThedyxEngine.Util;

public static class EngineObjectsFactory {
    
    /**
     * Deserializes a JSON string to a GrainSquare object.
     * \param json The JSON string to deserialize.
     * \return A new GrainSquare object deserialized from the JSON string.
     */
    public static GrainSquare GrainSquareFromJson(string json)
    {
        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        var jObject = JsonConvert.DeserializeObject<dynamic>(json, settings);

        string type = jObject.Type;
        if (type != "GrainSquare")
            throw new InvalidOperationException("JSON is not of type Grainsquare.");
        Point Position = Util.Parsers.ParsePoint(jObject.Position.ToString());

        string name = jObject.Name;
        double simulationTemperature = (double)jObject.SimulationTemperature;
        Material Material = MaterialManager.GetMaterialByName((string)jObject.MaterialName);
        return new GrainSquare(name, Position)
        {
            SimulationTemperature = simulationTemperature,
            CurrentTemperature = simulationTemperature,
            Material = Material
        };
    }
    
    
    /**
     * \brief Creates and object from JSON representation.
     * \param json The JSON representation of the object.
     * \returns The object created from JSON representation.
     */
    public static EngineRectangle EngineRectangleFromJson(string json) {
        var settings = new JsonSerializerSettings {
            NullValueHandling = NullValueHandling.Ignore
        };

        var jObject = JsonConvert.DeserializeObject<dynamic>(json, settings);

        string type = jObject.Type;

        if(type != "Rectangle") {
            throw new ArgumentException("JSON is not of type Rectangle");
        }
        string name = jObject.Name;
        double simulationTemperature = (double)jObject.SimulationTemperature;
        Point position = Util.Parsers.ParsePoint(jObject.Position.ToString());
        Point Position = Util.Parsers.ParsePoint(jObject.Position.ToString());
        Point Size = Util.Parsers.ParsePoint(jObject.Size.ToString());
        Material Material = MaterialManager.GetMaterialByName((string)jObject.MaterialName);

        return new EngineRectangle(name, (int)(Position.X + Size.X), (int)(Position.Y + Size.Y)) {
            SimulationTemperature = simulationTemperature,
            Position = Position,
            Size = Size,
        };
    }
    
    /**
     * \brief Creates and object from JSON representation.
     * \param json The JSON representation of the object.
     * \returns The object created from JSON representation.
     */
    public static EngineLiquid EngineStateObjectFromJson(string json) {
        var settings = new JsonSerializerSettings {
            NullValueHandling = NullValueHandling.Ignore
        };

        var jObject = JsonConvert.DeserializeObject<dynamic>(json, settings);

        string type = jObject.Type;

        if(type != "Rectangle") {
            throw new ArgumentException("JSON is not of type Rectangle");
        }
        string name = jObject.Name;
        double simulationTemperature = (double)jObject.SimulationTemperature;
        Point position = Util.Parsers.ParsePoint(jObject.Position.ToString());
        Point Position = Util.Parsers.ParsePoint(jObject.Position.ToString());
        Point Size = Util.Parsers.ParsePoint(jObject.Size.ToString());
        Material Material = MaterialManager.GetMaterialByName((string)jObject.MaterialName);

        return new EngineLiquid(name, (int)(Position.X + Size.X), (int)(Position.Y + Size.Y)) {
            SimulationTemperature = simulationTemperature,
            Position = Position,
            Size = Size,
        };
    }
}