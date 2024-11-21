using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThedyxEngine.Util;

namespace ThedyxEngine.Engine.Managers {

    /**
     * \class ConductionTransferManager
     * \brief Manages the transfer of conduction heat between objects in the simulation.
     *
     * The ConductionTransferManager class provides methods for calculating and transferring
     * conduction heat between objects in the simulation. 
     */
    public class ConductionTransferManager {

        /**
         * Transfer heat between two squares
         * \param sq1 first square
         * \param sq2 second square
         */
        public static void TransferConductionHeat(List<EngineObject> objects) {
            foreach (var obj in objects) {
                TransferHeatForObject(obj);
            }
        }

        /**
        * Transfer heat for every GrainSquare with its adjacent squares
        * \param obj object
        */
        private static void TransferHeatForObject(EngineObject obj) {
            List<GrainSquare> objsquares = obj.GetSquares();
            for (int i = 0; i < objsquares.Count; i++) {
                List<GrainSquare> adjSquares = objsquares[i].GetAdjacentSquares();
                for (int j = 0; j < adjSquares.Count; j++) {
                    TranferHeatBetweenTwoSquares(objsquares[i], adjSquares[j]);
                }
            }
        }

        /**
         * Transfer heat between two squares, details in documentation, Fourier's law
         * \param sq1 first square
         * \param sq2 second square
         */
        private static void TranferHeatBetweenTwoSquares(GrainSquare sq1, GrainSquare sq2) {
            double temperatureDifference = sq1.CurrentTemperature - sq2.CurrentTemperature;
            double coeficient = MaterialManager.GetCoeficientFromMaterial(sq1, sq2);
            double timeTransfer = Const.EngineIntervalUpdate / 1000;
            double heatTransfer = coeficient  * temperatureDifference * timeTransfer;
            sq1.AddEnergyDelta(-heatTransfer);
            // removed apply heat to the second square, because this will be called for the second square
            //sq2.AddEnergyDelta(heatTransfer);
        }


    }
}
