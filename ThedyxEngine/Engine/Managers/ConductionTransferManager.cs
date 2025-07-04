using ThedyxEngine.Util;

namespace ThedyxEngine.Engine.Managers {

    /**
     * \class ConductionTransferManager
     * \brief Manages the transfer of conduction heat between objects in the simulation.
     *
     * The ConductionTransferManager class provides methods for calculating and transferring
     * conduction heat between objects in the simulation. 
     */
    public class ConductionTransferManager : IHeatTransferManager {

        /**
         * Transfer conduction heat for specified objects
         * \param objects List of objects
         */
        public void TransferHeat(List<EngineObject> objects) {
            foreach (var obj in objects) {
                TransferHeatForObject(obj);
            }
        }

        /**
        * Transfer heat for every GrainSquare with its adjacent squares
        * \param obj object
        */
        private void TransferHeatForObject(EngineObject obj) {
            List<GrainSquare> objsquares = obj.GetSquares();
            for (int i = 0; i < objsquares.Count; i++) {
                List<GrainSquare> adjSquares = objsquares[i].GetAdjacentSquares();
                for (int j = 0; j < adjSquares.Count; j++) {
                    TranferHeatBetweenTwoSquares(objsquares[i], adjSquares[j]);
                }
                TransferHeatBetweenSquareAndAir(objsquares[i]);;
            }
        }

        /**
         * Transfer heat between two squares, details in documentation, Fourier's law
         * \param sq1 first square
         * \param sq2 second square
         */
        private void TranferHeatBetweenTwoSquares(GrainSquare sq1, GrainSquare sq2) {
            double temperatureDifference = sq1.CurrentTemperature - sq2.CurrentTemperature;
            double coeficient = MaterialManager.GetCoeficientFromMaterial(sq1, sq2);
            double timeTransfer = 1 / GlobalVariables.EngineIntervalUpdatePerSecond;
            double heatTransfer = coeficient  * temperatureDifference * timeTransfer;
            sq1.AddEnergyDelta(-heatTransfer);
            // removed apply heat to the second square, because this will be called for the second square
        }
    
        /**
         * Transfer heat between square and air
         * \param sq square
         */
        private void TransferHeatBetweenSquareAndAir(GrainSquare sq) {
            if(!GlobalVariables.ObjectsLooseHeatToAir) return;
            double temperatureDifference = sq.CurrentTemperature - GlobalVariables.AirTemperature;
            double thermalConductivity1 = sq.Material.SolidThermalConductivity;
            if (sq is StateGrainSquare statesq) {
                thermalConductivity1 = statesq.GetMaterialThermalConductivity();
            }
            double thermalConductivity2 = GlobalVariables.AirThermalConductivity;
            double coeficient = 2 * thermalConductivity1 * thermalConductivity2 / (thermalConductivity1 + thermalConductivity2);
            double timeTransfer = 1 / GlobalVariables.EngineIntervalUpdatePerSecond;
            int areaRadiationLoss = Math.Max((4 - sq.GetAdjacentSquares().Count),0);
            double heatTransfer = coeficient  * temperatureDifference * timeTransfer * areaRadiationLoss;
            sq.AddEnergyDelta(-heatTransfer);
        }

    }
}
