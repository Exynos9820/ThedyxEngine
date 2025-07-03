using ThedyxEngine.Util;

namespace ThedyxEngine.Engine.Managers;


/**
* \class ConvectionTransferManager
* \brief Manages the transfer of convection heat between objects in the simulation.
*
* The ConvectionTransferManager class provides methods for calculating and transferring
* convection heat between objects in the simulation. 
*/
public class ConvectionTransferManager {
    /**
     * Transfer convection heat for specified objects
     * \param objects List of objects
     */
    public static void TransferConvectionHeat(List<EngineObject> objects) {
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
     * Transfer heat between two squares, details in documentation, Newton's law of cooling
     * \param sq1 first square
     * \param sq2 second square
     * Link
     * https://www.engineeringtoolbox.com/convective-heat-transfer-d_430.html
     */
    private static void TranferHeatBetweenTwoSquares(GrainSquare sq1, GrainSquare sq2) {
        // First we need to check that both squared are StateGrainSquare
        if (!(sq1 is StateGrainSquare statesq1) || !(sq2 is StateGrainSquare statesq2)) {
            return;
        }
        // next we need to check if both squares are in the same state and it's liquid or gas
        if (statesq1.CurrentMaterialState != statesq2.CurrentMaterialState || statesq1.CurrentMaterialState == StateGrainSquare.MaterialState.Solid) {
            return;
        }
        // next we need to check if both squares are in the same material
        if (statesq1.Material != statesq2.Material) {
            return;
        }
        double tempDifference = statesq1.CurrentTemperature - statesq2.CurrentTemperature;
        // take depending on the state
        double coefficient = statesq1.CurrentMaterialState == StateGrainSquare.MaterialState.Gas
            ? statesq1.Material.GasConvectiveHeatTransferCoefficient
            : statesq1.Material.LiquidConvectiveHeatTransferCoefficient;
        
        double timeTransfer = 1 / GlobalVariables.EngineIntervalUpdatePerSecond;
        double area = GlobalVariables.GridStep;
        double heatTransfer = coefficient * tempDifference * timeTransfer * area;
        if(heatTransfer > 1000)
            heatTransfer = 1000;
        sq1.AddEnergyDelta(-heatTransfer);
    }
}