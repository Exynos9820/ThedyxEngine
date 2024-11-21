namespace ThedyxEngine.Engine.Managers {
    /**
     * \class OptimizationManager
     * \brief Manages the optimization of the engine
     *
     * The OptimizationManager class provides methods for optimizing the engine by setting adjacent squares to be touching
     */
    public static class ObjectsOptimizationManager {

        /**
         * Optimize objects
         * \param objects list of objects
         */
        public static void Optimize(List<EngineObject> objects) {
            ClearOptimization(objects);
            OptimizeTouching(objects);
            RadiationOptimization.Optimize(objects);
        }

        /**
         * Fill adjacent squares for two lists of squares
         * \param squares1 first list of squares
         * \param squares2 second list of squares
         */
        private static void FillExternalSquares(List<GrainSquare> squares1, List<GrainSquare> squares2) {
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
        private static void OptimizeTouching(List<EngineObject> objects) {
            for (int i = 0; i < objects.Count; i++) {
                List<GrainSquare> firstExternal = objects[i].GetExternalSquares();

                for (int j = i + 1; j < objects.Count; j++) {
                    List<GrainSquare> secondExternal = objects[j].GetExternalSquares();
                    FillExternalSquares(firstExternal, secondExternal);
                }
            }
        }

        /**
         * Clear optimization
         * \param objects list of objects
         */
        private static void ClearOptimization(List<EngineObject> objects) {
            foreach(var obj in objects) {
                List<GrainSquare> squares = obj.GetSquares();
                // clear adjacent squares
                foreach(var square in squares) {
                    square.ClearOptimizationSquares();
                }
            }
        }
    }
}
