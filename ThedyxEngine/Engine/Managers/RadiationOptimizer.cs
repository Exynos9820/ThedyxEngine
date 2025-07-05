using ThedyxEngine.Util;

namespace ThedyxEngine.Engine.Managers;

/**
 * \class RadiationOptimizer
 * \brief Builds a visibility map for RadiationTransferManager
 */
public class RadiationOptimizer : IOptimizer{
    // List of all possible directions for line of sight
    private readonly List<(int X, int Y)> _directions =
    [
        (-1, 0), // Left
        (1, 0), // Right
        (0, -1), // Up
        (0, 1), // Down
        (-1, -1), // Top-left diagonal
        (-1, 1), // Bottom-left diagonal
        (1, -1), // Top-right diagonal
        (1, 1)
    ];

    /**
     * \brief Determines if a target square is visible from the start square.
     * 
     * This function checks whether there are any blocking squares along the path 
     * between the start square and the target square in a given direction. The 
     * visibility is determined only based on the squares found so far in that direction.
     * 
     * \param startSquare The square from which visibility is checked.
     * \param targetSquare The square whose visibility is being determined.
     * \param direction The direction in which the visibility is being checked.
     * \param visibleSquares The list of already found visible squares.
     * \return True if the target square is visible, false if the path is blocked.
     */
    private bool IsVisible(GrainSquare startSquare, GrainSquare targetSquare, (int X, int Y) direction, List<GrainSquare> visibleSquares) {
        int steps = Math.Max(
            Math.Abs((int)(targetSquare.Position.X - startSquare.Position.X)),
            Math.Abs((int)(targetSquare.Position.Y - startSquare.Position.Y))
        );

        for (int step = 1; step < steps; step++) {
            double intermediateX = startSquare.Position.X + direction.X * step;
            double intermediateY = startSquare.Position.Y + direction.Y * step;

            // Check if there is a square blocking the path
            if (visibleSquares.Any(s =>
                Math.Abs(s.Position.X - intermediateX) < 1e-6 &&
                Math.Abs(s.Position.Y - intermediateY) < 1e-6))
            {
                return false; // Path is blocked
            }
        }

        return true; // Path is clear
    }

    /**
     * \brief Searches for visible squares in all directions from the given square.
     * 
     * This method iterates over all possible directions and searches for squares 
     * within a specified depth that are visible from the given square. It stops 
     * searching in a direction once a visible square is found.
     * 
     * \param square The starting square from which the search is initiated.
     * \param allSquares A list of all squares in the grid.
     * \param maxDepth The maximum search depth (number of squares).
     */
    private void RetraceSquares(GrainSquare square, List<GrainSquare> allSquares, int maxDepth) {
        var visibleSquares = new List<GrainSquare>();
        foreach (var direction in _directions) {
            // Start looking in the current direction
            for (int depth = 1; depth <= maxDepth; depth++) {
                var targetX = square.Position.X + direction.X * depth;
                var targetY = square.Position.Y + direction.Y * depth;

                // Find a square at the target position
                var foundSquare = allSquares.FirstOrDefault(
                    s => Math.Abs(s.Position.X - targetX) < 1e-6 && Math.Abs(s.Position.Y - targetY) < 1e-6);

                if (foundSquare != null && IsVisible(square, foundSquare, direction, visibleSquares)) {
                    // Process the found square
                    square.AddRadiationExchangeSquare(foundSquare);

                    // Add the found square to the visible squares list
                    visibleSquares.Add(foundSquare);
                    break; // Stop looking further in this direction
                }
            }
        }
    }

   /**
    * \brief Optimizes radiation by retracing squares from each starting square.
    *
    * This method identifies visible squares for each square in the grid within
    * the maximum depth specified in `Const.RadiationDepth`. It combines all grain
    * squares from the provided engine objects and applies retracing logic to
    * determine visibility.
    *
    * \param objects A list of engine objects containing grain squares.
    */
    public void Optimize(List<EngineObject> objects, int startPosition, int endPosition) {
        // Create a list of all grain squares
        List<GrainSquare> grainSquares = [];
        foreach (var obj in objects) {
            grainSquares.AddRange(obj.GetExternalSquares());
        }
        
        
        List<GrainSquare> runSquares = [];
        // get the grain squares from the objects from the start position to the end position
        for (int i = startPosition; i < endPosition; i++) {
            runSquares.AddRange(objects[i].GetExternalSquares());
        }
        
        // Retrace squares for each grain square
        foreach (var square in runSquares) {
            RetraceSquares(square, grainSquares, GlobalVariables.RadiationDepth);
        }
    }

    /**
      * Returns a name of optimizer
      */
    public string GetName() {
        return "Radiation";
    }
}