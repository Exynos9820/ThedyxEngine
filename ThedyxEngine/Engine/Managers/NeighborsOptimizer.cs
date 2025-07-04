namespace ThedyxEngine.Engine.Managers;


/**
 * \class NeighborsOptimizer
 * \brief Builds a map of neighbors for objects
 */
public class NeighborsOptimizer : IOptimizer {
    
    /**
     * Fill adjacent squares for two lists of squares
     * \param squares1 first list of squares
     * \param squares2 second list of squares
     */
    private static void FillAdjacentSquares(List<GrainSquare> squares1, List<GrainSquare> squares2) {
        foreach(var square1 in squares1) {
            foreach(var square2 in squares2) {
                if (square1.AreTouching(square2)) {
                    square1.AddAdjacentSquare(square2);
                    square1.AddRadiationExchangeSquare(square2);
                    square2.AddAdjacentSquare(square1);
                    square2.AddRadiationExchangeSquare(square1);
                }
            }
        }
    }

    /**
     * Optimize touching objects, by setting adjacent squares for every square of an object
     * \param objects list of objects
     */
    public void Optimize(List<EngineObject> objects, int startPosition, int endPosition) {
        for (int i = startPosition; i < endPosition; i++) {
            List<GrainSquare> firstExternal = objects[i].GetExternalSquares();
            FillAdjacentSquares(objects[i].GetSquares(), objects[i].GetSquares());

            for (int j = 0; j < objects.Count; j++) {
                if (i == j) continue;
                List<GrainSquare> secondExternal = objects[j].GetExternalSquares();
                FillAdjacentSquares(firstExternal, secondExternal);
            }
        }
    }
    
    /**
     * Returns a name of optimizer
     */
    public string GetName() {
        return "Neighbors";
    }
}