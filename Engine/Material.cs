using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThedyxEngine.Engine {
    /** 
     * \class Material
     * \brief Represents a material in the simulation.
     * 
     * The Material class provides methods for managing the materials in the simulation.
     */
    public class Material {
        public string Name { get; set; }  /// Gets or sets the name of the material.

        public bool isBaseMaterial { get; set; } /// Gets or sets the base material.

        public double SolidSpecificHeatCapacity { get; set; } /// Gets or sets the specific heat capacity.
        public double LiquidSpecificHeatCapacity { get; set; } /// Gets or sets the specific heat capacity.
        public double GasSpecificHeatCapacity { get; set; } /// Gets or sets the specific heat capacity.

        public double SolidDensity { get; set; } /// Gets or sets the density.
        public double LiquidDensity { get; set; } /// Gets or sets the density.
        public double GasDensity { get; set; } /// Gets or sets the density.

        public double SolidEmmisivity { get; set; } /// Gets or sets the emissivity.
        
        public double SolidThermalConductivity { get; set; } /// Gets or sets the thermal conductivity.
        
        public double MeltingTemperature { get; set; } /// Gets or sets the solidification temperature.
        
        public double BoilingTemperature { get; set; } /// Gets or sets the boiling temperature.
        
        public double MeltingEnergy { get; set; } /// Gets or sets the solidification energy.
        public double BoilingEnergy { get; set; } /// Gets or sets the boiling energy.
        public Color MaterialColor { get; set; } /// Gets or sets the material color.
        /**
         * \brief Returns the name of the material.
         * \return The name of the material.
         */
        public override string ToString() {
            return Name;
        }
    }
}
