using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ThedyxEngine.Engine.Managers{
    /**
     * \class RadiationTransferManager
     * \brief Manages the transfer of radiation heat between objects in the simulation.
     *
     * The RadiationTransferManager class provides methods for calculating and transferring
     * radiation heat between objects in the simulation. It includes methods for transferring
     * radiation heat between two objects and radiation heat loss to air.
     */
    public static class RadiationTransferManager {

        /// Stefan-Boltzmann constant
        public static readonly double StefanBoltzmannConst = 5.67 * Math.Pow(10, -8);
        /**
         * Transfer radiation heat between two objects
         * Q = σ*A*e`*(T1^4 - T2^4)*t. 
         * \param obj1 first object
         * \param obj2 second object
         */
        private static void TransferRadiationBetweenTwoObjects(EngineObject obj1, EngineObject obj2) {
            List<GrainSquare> obj1squares = obj1.GetSquares();
            List<GrainSquare> obj2squares = obj2.GetSquares();
            foreach (var square1 in obj1squares) {
                foreach (var square2 in obj2squares) {
                    double emmisivityBetweenTwoObjects = (square1.Material.Emmisivity * square2.Material.Emmisivity) / (1/square1.Material.Emmisivity + 1/square2.Material.Emmisivity - 1);
                    double viewFactor = 1;
                    double energyRadiationLoss = emmisivityBetweenTwoObjects * StefanBoltzmannConst * Engine.GridStep * viewFactor * (Math.Pow(square1.CurrentTemperature, 4) - Math.Pow(square2.CurrentTemperature, 4)) * Engine.EngineIntervalUpdate;
                    square1.AddEnergyDelta(-energyRadiationLoss);
                    square2.AddEnergyDelta(energyRadiationLoss);
                }
            }
        }

        /**
         * Transfer radiation heat loss to air
         * Q = σ*A*e`*(T1^4 - T2^4)*t.
         * \param obj object
         */
        private static void TransferRadiationHeatLooseToAir(EngineObject obj) {
            List<GrainSquare> objsquares = obj.GetSquares();
            foreach(var square in objsquares) {
                // calculated by Stefan-Boltzmann law of radiation and multiplied by the engine update interval
                // we need to find the area of the square that is not touching other squares to calculate the radiation loss to air
                double areaRadiationLoss = 4 - square.GetAdjacentSquares().Count * Engine.GridStep;
                double energyRadiationLoss = square.Material.Emmisivity * StefanBoltzmannConst * areaRadiationLoss * (Math.Pow(square.CurrentTemperature, 4) - Math.Pow(Engine.AirTemperature, 4)) * Engine.EngineIntervalUpdate;
                square.AddEnergyDelta(-energyRadiationLoss);
            }
        }

        /**
         * 
         * Transfer radiation heat for all objects
         * First, transfer radiation heat loss to air
         * Second, transfer radiation heat between objects
         * Simplified logic for now, details are in the documentation
         * \param objects list of objects
         */
        public static void TransferRadiationHeat(List<EngineObject> objects){
            foreach(var obj in objects){
                TransferRadiationHeatLooseToAir(obj);
            }

            for (int i = 0; i < objects.Count; i++) {
                for (int j = i + 1; j < objects.Count; j++) {
                    TransferRadiationBetweenTwoObjects(objects[i], objects[j]);
                }
            }
        }

    }
}
