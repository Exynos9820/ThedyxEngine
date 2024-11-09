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
        private static void populateWithBaseMaterials() {
            Material m1 = new Material();
            m1.Name = "Aluminium";
            m1.isBaseMaterial = true;
            m1.SpecificHeatCapacity = 900;
            m1.Density = 2700;
            m1.Emmisivity = 0.03;
            Materials.Add(m1);
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
