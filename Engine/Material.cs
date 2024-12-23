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

        public double SpecificHeatCapacity { get; set; } /// Gets or sets the specific heat capacity.

        public double Density { get; set; } /// Gets or sets the density.

        public double Emmisivity { get; set; } /// Gets or sets the emissivity.
        
        public double ThermalConductivity { get; set; } /// Gets or sets the thermal conductivity.

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
