using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            populateWithBaseMaterials();
        }

        /**
         * Populate the materials with base materials
         */
        // https://www.thermoworks.com/emissivity-table/?srsltid=AfmBOopr9aZ1nes5l5xnvluHzu6TTiJtLAQOXSMWJnuCAe5oc_aQNfxm
        // https://en.wikipedia.org/wiki/Density
        private static void populateWithBaseMaterials() {
            var m1 = new Material {
                Name = "Aluminium",
                isBaseMaterial = true,
                SolidSpecificHeatCapacity = 887,
                SolidDensity = 2700,
                SolidEmmisivity = 0.05,
                MaterialColor = Colors.Gray,
                SolidThermalConductivity = 205
            };
            Materials.Add(m1);
            var m2 = new Material {
                Name = "Solid White Plastic",
                isBaseMaterial = true,
                SolidSpecificHeatCapacity = 1300,
                SolidDensity = 1000,
                SolidEmmisivity = 0.84,
                MaterialColor = Colors.LightGray
            };
            Materials.Add(m2);
            
            var m3 = new Material {
                Name = "Glass",
                isBaseMaterial = true,
                SolidSpecificHeatCapacity = 792,
                SolidDensity = 2500,
                SolidEmmisivity = 0.92,
                MaterialColor = Colors.LightBlue
            };
            Materials.Add(m3);
            
            var m4 = new Material {
                Name = "Copper: oxidized",
                isBaseMaterial = true,
                SolidSpecificHeatCapacity = 385,
                SolidDensity = 8940,
                SolidEmmisivity = 0.65,
                MaterialColor = Colors.Coral
            };
            Materials.Add(m4);

            var m5 = new Material {
                Name = "Water",
                isBaseMaterial = true,
                SolidSpecificHeatCapacity = 2090,
                LiquidSpecificHeatCapacity = 4186,
                GasSpecificHeatCapacity = 1.996,
                SolidDensity = 1000,
                LiquidDensity = 1000,
                GasDensity = 1000,
                SolidEmmisivity = 0.95,
                MeltingTemperature = 273.15,
                BoilingTemperature = 373.15,
                MeltingEnergy = 334000,
                BoilingEnergy = 2257000,
                SolidThermalConductivity = 2.22,
                LiquidThermalConductivity = 0.606,
                GasThermalConductivity = 0.016,
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
         * Get the coefficient between two objects
         */
        public static double GetCoeficientFromMaterial(GrainSquare obj1, GrainSquare obj2) {
            double thermalConductivity1;
            double thermalConductivity2;
            if(obj1 is EngineStateGrainSquare) {
                // check state of the object and get the right thermal conductivity
                var obj = (EngineStateGrainSquare)obj1;
                if(obj.CurrentMaterialState == EngineStateGrainSquare.MaterialState.Solid) {
                    thermalConductivity1 = obj.Material.SolidThermalConductivity;
                } else if(obj.CurrentMaterialState == EngineStateGrainSquare.MaterialState.Liquid) {
                    thermalConductivity1 = obj.Material.LiquidThermalConductivity;
                } else {
                    thermalConductivity1 = obj.Material.GasThermalConductivity;
                }
            }else {
                thermalConductivity1 = obj1.Material.SolidThermalConductivity;
            }
            // the same for the second object
            if (obj2 is EngineStateGrainSquare liquid) {
                // check state of the object and get the right thermal conductivity
                if(liquid.CurrentMaterialState == EngineStateGrainSquare.MaterialState.Solid) {
                    thermalConductivity2 = liquid.Material.SolidThermalConductivity;
                } else if(liquid.CurrentMaterialState == EngineStateGrainSquare.MaterialState.Liquid) {
                    thermalConductivity2 = liquid.Material.LiquidThermalConductivity;
                } else {
                    thermalConductivity2 = liquid.Material.GasThermalConductivity;
                }
            }else {
                thermalConductivity2 = obj2.Material.SolidThermalConductivity;
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
