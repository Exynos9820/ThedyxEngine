namespace ThedyxEngine.Util{
    /**
     * \class CanvasData
     * \brief Provides data for the canvas.
     * 
     * The CanvasData class provides data for the canvas.
     */
    static class CanvasHelper {
        // X > 0 parameters
        /** Maximum left X index */
        public static int MinLeftXIndex = -1000;
        /** Maximum left X index */
        public static int MaxLeftXIndex = -10;
        
        // X < 0 parameters
        /** Maximum right X index */
        public static int MaxRightXIndex = 1000; 
        /** Minumum right X index */
        public static int MinRightXIndex = 10;

        // Y > 0 parameters
        /** Maximum top Y index */
        public static int MaxYTopIndex = 1000;
        /** Minimum top Y index */
        public static int MinYTopIndex = 10;

        // Y < 0 parameters
        /** Minimum bottom Y index */
        public static int MinYBottomIndex = -1000;
        /** Maximum bottom Y index */
        public static int MaxYBottomIndex = -10;

        
        /**
         * Get the group by value for the current canvas width
         * \param canvasWidth Width of the canvas
         * \param Size Size of the object
         * \return Group by value
         */
        public static int GetGroupByValue(int canvasWidth, Point size) {
            int sx = (int)Math.Round(size.X);
            int groupBy = 1;
            if (canvasWidth is >= 50 and < 200) {
                groupBy = PickDivisor(sx, new[] { 3, 4, 5 }, 5);
            }
            else if (canvasWidth is >= 200 and < 500) {
                groupBy = PickDivisor(sx, new[] { 8, 9, 10, 11, 12 }, 10);
            }
            else if (canvasWidth is >= 500 and < 1000) {
                groupBy = PickDivisor(sx, new[] { 20, 25, 30, 35, 40 }, 30);
            }
            else if (canvasWidth is >= 1000 and < 2000) {
                groupBy = PickDivisor(sx, new[] { 50, 60, 70, 80, 90 }, 50);
            }
            else if (canvasWidth >= 2000) {
                groupBy = PickDivisor(sx, new[] { 100, 200, 300, 400, 500 }, 100);
            }

            return groupBy;
        }

        /**
         * Returns the first divisor that evenly divides value
         * \param value - to ve divided
         * \param divisors - list of divisors
         * \param fallback - default value
         */
        private static int PickDivisor(int value, int[] divisors, int fallback)
        {
            foreach (var d in divisors)
                if (value % d == 0)
                    return d;

            return fallback;
        }

    }
}
