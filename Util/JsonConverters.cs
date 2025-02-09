using Newtonsoft.Json;
using ThedyxEngine.Engine;
using ThedyxEngine.Engine.Managers;

namespace ThedyxEngine.Util;

public static class JsonConverters {
    
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
        Material Material = MaterialManager.GetMaterialByName((string)jObject.Material);

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

    public static Material MaterialFromJson(string json) {
        var settings = new JsonSerializerSettings {
            NullValueHandling = NullValueHandling.Ignore
        };

        var jObject = JsonConvert.DeserializeObject<dynamic>(json, settings);
        // we need to convert the color to string of an array of integers
        string color = jObject?.MaterialColor;
        color = color.Replace("[", "").Replace("]", "");
        var colorValues = color.Split(",");
        var materialColor = new Color(float.Parse(colorValues[0]), float.Parse(colorValues[1]), float.Parse(colorValues[2]), float.Parse(colorValues[3]));
        return new Material() {
            Name = jObject.Name,
            SolidSpecificHeatCapacity = jObject.SolidSpecificHeatCapacity,
            LiquidSpecificHeatCapacity = jObject.LiquidSpecificHeatCapacity,
            GasSpecificHeatCapacity = jObject.GasSpecificHeatCapacity,
            SolidDensity = jObject.SolidDensity,
            LiquidDensity = jObject.LiquidDensity,
            GasDensity = jObject.GasDensity,
            SolidEmissivity = jObject.SolidEmissivity,
            LiquidEmissivity = jObject.LiquidEmissivity,
            GasEmissivity = jObject.GasEmissivity,
            SolidThermalConductivity = jObject.SolidThermalConductivity,
            LiquidThermalConductivity = jObject.LiquidThermalConductivity,
            GasThermalConductivity = jObject.GasThermalConductivity,
            MeltingTemperature = jObject.MeltingTemperature,
            BoilingTemperature = jObject.BoilingTemperature,
            MeltingEnergy = jObject.MeltingEnergy,
            BoilingEnergy = jObject.BoilingEnergy,
            MaterialColor = materialColor
        };
    }
}