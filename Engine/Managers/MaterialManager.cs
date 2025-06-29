using ThedyxEngine.Engine.Objects;

namespace ThedyxEngine.Engine.Managers {
    /** 
     * \class MaterialManager
     * \brief Manages the materials in the simulation.
     * 
     * The MaterialManager class provides methods for managing the materials in the simulation.
     */
    internal class MaterialManager {

        public static List<Material> Materials = []; /// List of materials

        /**
         * Initialize the materials
         */
        public static void Init() {
            PopulateWithBaseMaterials();
        }

        /**
         * Populate the materials with base materials
         */
        /* for emissivity
        https://www.thermoworks.com/emissivity-table/?srsltid=AfmBOopr9aZ1nes5l5xnvluHzu6TTiJtLAQOXSMWJnuCAe5oc_aQNfxm
        for density
        https://en.wikipedia.org/wiki/Density
        for thermal conductivity
        https://www.engineeringtoolbox.com/
        */
        private static void PopulateWithBaseMaterials() {
            var m1 = new Material {
                Name = "Aluminium",
                SolidSpecificHeatCapacity = 887,
                LiquidSpecificHeatCapacity = 1170,
                GasSpecificHeatCapacity = 0.9,
                SolidDensity = 2700,
                LiquidDensity = 2385,
                GasDensity = 1.29,
                SolidEmissivity = 0.2,
                LiquidEmissivity = 0.35,
                GasEmissivity = 0.01,
                MeltingTemperature = 933.47,
                BoilingTemperature = 2792,
                MeltingEnergy = 397000,
                BoilingEnergy = 10500000,
                SolidThermalConductivity = 205,
                LiquidThermalConductivity = 80,
                GasThermalConductivity = 0.03,
                GasConvectiveHeatTransferCoefficient = 10000,
                LiquidConvectiveHeatTransferCoefficient = 3000,
                MaterialColor = Colors.Gray
            };
            Materials.Add(m1);
            var m2 = new Material {
                Name = "Solid White Plastic",
                SolidSpecificHeatCapacity = 1300,
                SolidDensity = 1000,
                SolidEmissivity = 0.84,
                SolidThermalConductivity = 0.33,
                MaterialColor = Colors.LightGray
            };
            Materials.Add(m2);
            
            var m3 = new Material {
                Name = "Glass",
                SolidSpecificHeatCapacity = 792,
                SolidDensity = 2500,
                SolidEmissivity = 0.92,
                MaterialColor = Colors.LightBlue
            };
            Materials.Add(m3);
            
            var m4 = new Material {
                Name = "Copper: oxidized",
                SolidSpecificHeatCapacity = 385,
                SolidDensity = 8940,
                SolidEmissivity = 0.65,
                SolidThermalConductivity = 401,
                MaterialColor = Colors.Coral
            };
            Materials.Add(m4);

            var m5 = new Material {
                Name = "Water",
                SolidSpecificHeatCapacity = 2090,
                LiquidSpecificHeatCapacity = 4186,
                GasSpecificHeatCapacity = 1.996,
                SolidDensity = 1000,
                LiquidDensity = 1000,
                GasDensity = 1000,
                SolidEmissivity = 0.98,
                LiquidEmissivity = 0.96,
                GasEmissivity = 0.1,
                MeltingTemperature = 273.15,
                BoilingTemperature = 373.15,
                MeltingEnergy = 334000,
                BoilingEnergy = 2257000,
                SolidThermalConductivity = 2.22,
                LiquidThermalConductivity = 0.606,
                GasThermalConductivity = 0.016,
                GasConvectiveHeatTransferCoefficient = 60000,
                LiquidConvectiveHeatTransferCoefficient = 1500,
                MaterialColor = Colors.DodgerBlue
            };
            Materials.Add(m5);
        }


        /**
         * Get the base material
         * \return The base material
         */
        public static Material GetBaseMaterial() {
            if(Materials.Count == 0) throw new Exception("Materials are not initialized");
            return Materials[0];
        }

        /** 
         * Get material by name
         * \param name Name of the material
         * \return The material
         */
        public static Material GetMaterialByName(string name) {
            return Materials.Find(x => x.Name == name);
        }

        /** 
         * Get all materials
         * \return List<Material> of the materials
         */
        public static List<Material> GetMaterials() {
           return Materials;
        }
        
        
        /**
         * Check if the name is available
         * \param name Name of the material
         */
        public static bool IsNameAvailable(string name) {
            return Materials.Find(x => x.Name == name) == null;
        }

        
        /**
         * Get the coefficient between two objects
         */
        public static double GetCoeficientFromMaterial(GrainSquare obj1, GrainSquare obj2) {
            double thermalConductivity1 = obj1.Material.SolidThermalConductivity;
            double thermalConductivity2 = obj2.Material.SolidThermalConductivity;
            if(thermalConductivity1 == 0 || thermalConductivity2 == 0) return 0;
            if(obj1 is StateGrainSquare state1) {
                thermalConductivity1 = state1.GetMaterialThermalConductivity();
            }
            // the same for the second object
            if (obj2 is StateGrainSquare state2) {
                thermalConductivity2 = state2.GetMaterialThermalConductivity();
            }
            return 2 * thermalConductivity1 * thermalConductivity2 / (thermalConductivity1 + thermalConductivity2);
        }



        // calculated as e'=(e1*e2)/(e1+e2 - e1*e2)
        // 03 is the default value for the emissivity of the object for aluminium  
        /**
         * Get the emissivity between two objects
         * Calculated as e'=(e1*e2)/(e1+e2 - e1*e2)
         */
        public static double GetEmmisivityBetweenTwoObjects(GrainSquare obj1, GrainSquare obj2) {
            return 0.3;
        }

    }
}
