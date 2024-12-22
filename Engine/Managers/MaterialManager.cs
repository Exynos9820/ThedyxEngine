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
                SpecificHeatCapacity = 887,
                Density = 2700,
                Emmisivity = 0.05,
                MaterialColor = Colors.Gray
            };
            Materials.Add(m1);
            var m2 = new Material {
                Name = "Solid White Plastic",
                isBaseMaterial = true,
                SpecificHeatCapacity = 1300,
                Density = 1000,
                Emmisivity = 0.84,
                MaterialColor = Colors.LightGray
            };
            Materials.Add(m2);
            
            var m3 = new Material {
                Name = "Glass",
                isBaseMaterial = true,
                SpecificHeatCapacity = 792,
                Density = 2500,
                Emmisivity = 0.92,
                MaterialColor = Colors.LightBlue
            };
            Materials.Add(m3);
            
            var m4 = new Material {
                Name = "Copper: oxidized",
                isBaseMaterial = true,
                SpecificHeatCapacity = 385,
                Density = 8940,
                Emmisivity = 0.65,
                MaterialColor = Colors.Coral
            };
            Materials.Add(m4);
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
            return 2.05;
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
