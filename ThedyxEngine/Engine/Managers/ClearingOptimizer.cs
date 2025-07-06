using ThedyxEngine.Engine.Objects;

namespace ThedyxEngine.Engine.Managers;

/**
 * \class ClearingOptimizer
 * \brief Implements IOptimizer. Clears data before optimizations.
 */
public class ClearingOptimizer : IOptimizer {
    /**
     * Clear optimization
     * \param objects list of objects
     */
    public void Optimize(List<EngineObject> objects, int startPosition, int endPosition) {
        for (int i = startPosition; i < endPosition; i++) {
            List<GrainSquare> squares = objects[i].GetSquares();
            foreach (var square in squares) {
                // clear adjacent and radiation squares
                square.ClearOptimizationSquares();
            }
        }
    }
    
    /**
     * Returns a name of optimizer
     */
    public string GetName() {
        return "Clearing";
    }
}