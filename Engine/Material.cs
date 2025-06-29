using Newtonsoft.Json;

namespace ThedyxEngine.Engine {
    /** 
     * \class Material
     * \brief Represents a material in the simulation.
     * 
     * The Material class provides methods for managing the materials in the simulation.
     */
    public class Material {
        public string Name { get; set; }  /// Gets or sets the name of the material.
        
        public double SolidSpecificHeatCapacity { get; set; } /// Gets or sets the specific heat capacity.
        public double LiquidSpecificHeatCapacity { get; set; } /// Gets or sets the specific heat capacity.
        public double GasSpecificHeatCapacity { get; set; } /// Gets or sets the specific heat capacity.

        public double SolidDensity { get; set; } /// Gets or sets the density.
        public double LiquidDensity { get; set; } /// Gets or sets the density.
        public double GasDensity { get; set; } /// Gets or sets the density.

        public double SolidEmissivity { get; set; } /// Gets or sets the emissivity.
        public double LiquidEmissivity { get; set; } /// Gets or sets the emissivity.
        public double GasEmissivity { get; set; } /// Gets or sets the emissivity.
        public double SolidThermalConductivity { get; set; } /// Gets or sets the thermal conductivity.
        public double LiquidThermalConductivity { get; set; } /// Gets or sets the thermal conductivity.
        public double GasThermalConductivity { get; set; } /// Gets or sets the thermal conductivity.
        public double MeltingTemperature { get; set; } /// Gets or sets the solidification temperature.
        
        public double BoilingTemperature { get; set; } /// Gets or sets the boiling temperature.
        
        public double MeltingEnergy { get; set; } /// Gets or sets the solidification energy.
        public double BoilingEnergy { get; set; } /// Gets or sets the boiling energy.
        public double LiquidConvectiveHeatTransferCoefficient { get; set; } /// Gets or sets the liquid convective heat transfer coefficient.
        public double GasConvectiveHeatTransferCoefficient { get; set; } /// Gets or sets the gas convective heat transfer coefficient.
        public Color MaterialColor { get; set; }  /// Gets or sets the material color.

        /**
         * \brief Creates a new material.
         */
        public Material() {
            Name = "Unknown";
            MaterialColor = Colors.Black;
        }
        /**
         * \brief Returns the name of the material.
         * \return The name of the material.
         */
        public override string ToString() {
            return Name;
        }
        
        /**
         * \brief Converts the material to a JSON string.
         * \return The JSON string.
         */
        public String ToJson() {
            // format the material to json
            var settings = new JsonSerializerSettings {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
            
            // we need to convert the color to string of an array of integers
            string color = $"[{MaterialColor.Red}, {MaterialColor.Green}, {MaterialColor.Blue}, {MaterialColor.Alpha}]";

            return JsonConvert.SerializeObject(new {
                Type = "Material",
                Name,
                SolidSpecificHeatCapacity,
                LiquidSpecificHeatCapacity,
                GasSpecificHeatCapacity,
                SolidDensity,
                LiquidDensity,
                GasDensity,
                SolidEmissivity,
                LiquidEmissivity,
                GasEmissivity,
                SolidThermalConductivity,
                LiquidThermalConductivity,
                GasThermalConductivity,
                MeltingTemperature,
                BoilingTemperature,
                MeltingEnergy,
                BoilingEnergy,
                MaterialColor = color
            }, settings);
        }
    }
}
