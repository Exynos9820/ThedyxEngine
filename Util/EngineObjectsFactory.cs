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
        if (type != ObjectType.GrainSquare.ToString())
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
     * Deserializes a JSON string to a GrainSquare object.
     * \param json The JSON string to deserialize.
     * \return A new GrainSquare object deserialized from the JSON string.
     */
    public static StateGrainSquare StateGrainSquareFromJson(string json)
    {
        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        var jObject = JsonConvert.DeserializeObject<dynamic>(json, settings);

        string type = jObject.Type;
        if (type != ObjectType.StateGrainSquare.ToString())
            throw new InvalidOperationException("JSON is not of type Grainsquare.");
        Point Position = Util.Parsers.ParsePoint(jObject.Position.ToString());

        string name = jObject.Name;
        double simulationTemperature = (double)jObject.SimulationTemperature;
        Material Material = MaterialManager.GetMaterialByName((string)jObject.MaterialName);
        return new StateGrainSquare(name, Position, Material)
        {
            SimulationTemperature = simulationTemperature,
            CurrentTemperature = simulationTemperature,
        };
    }
    
    
    /**
     * \brief Creates and object from JSON representation.
     * \param json The JSON representation of the object.
     * \returns The object created from JSON representation.
     */
    public static EngineRectangle RectangleFromJson(string json) {
        var settings = new JsonSerializerSettings {
            NullValueHandling = NullValueHandling.Ignore
        };

        var jObject = JsonConvert.DeserializeObject<dynamic>(json, settings);

        string type = jObject.Type;

        if(type != ObjectType.Rectangle.ToString()) {
            throw new ArgumentException("JSON is not of type Rectangle");
        }
        string name = jObject.Name;
        double simulationTemperature = (double)jObject.SimulationTemperature;
        Point Position = Util.Parsers.ParsePoint(jObject.Position.ToString());
        Point Size = Util.Parsers.ParsePoint(jObject.Size.ToString());
        Material Material = MaterialManager.GetMaterialByName((string)jObject.MaterialName);

        return new EngineRectangle(name, (int)(Position.X + Size.X), (int)(Position.Y + Size.Y)) {
            SimulationTemperature = simulationTemperature,
            Position = Position,
            Size = Size,
            Material = Material
        };
    }
    
    /**
     * \brief Creates and object from JSON representation.
     * \param json The JSON representation of the object.
     * \returns The object created from JSON representation.
     */
    public static EngineStateRectangle StateRectangleFromJson(string json) {
        var settings = new JsonSerializerSettings {
            NullValueHandling = NullValueHandling.Ignore
        };

        var jObject = JsonConvert.DeserializeObject<dynamic>(json, settings);

        string type = jObject.Type;

        if(type != ObjectType.StateRectangle.ToString()) {
            throw new ArgumentException("JSON is not of type Rectangle");
        }
        string name = jObject.Name;
        double simulationTemperature = (double)jObject.SimulationTemperature;
        Point Position = Util.Parsers.ParsePoint(jObject.Position.ToString());
        Point Size = Util.Parsers.ParsePoint(jObject.Size.ToString());
        Material Material = MaterialManager.GetMaterialByName((string)jObject.MaterialName);

        return new EngineStateRectangle(name, (int)(Position.X + Size.X), (int)(Position.Y + Size.Y)) {
            SimulationTemperature = simulationTemperature,
            Position = Position,
            Size = Size,
            Material = Material
        };
    }
}