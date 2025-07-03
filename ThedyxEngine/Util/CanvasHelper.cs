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
        public static int GetGroubByValue(int canvasWidth, Point size) {
            var groupBy = 1;
            
            // we need to try to make groups that are divisible by the size of the object
            if (canvasWidth >= 50 && canvasWidth < 200) {
                // try divisibility by 3,4,5, if no success, use 5
                if (size.X % 3 == 0) groupBy = 3;
                else if (size.X % 4 == 0) groupBy = 4;
                else if (size.X % 5 == 0) groupBy = 5;
                else groupBy = 5;
            }else if (canvasWidth >= 200 && canvasWidth < 500) {
                // try divisibility by 8, 9, 10, 11, 12, if no success, use 10
                if (size.X % 8 == 0) groupBy = 8;
                else if (size.X % 9 == 0) groupBy = 9;
                else if (size.X % 10 == 0) groupBy = 10;
                else if (size.X % 11 == 0) groupBy = 11;
                else if (size.X % 12 == 0) groupBy = 12;
                else groupBy = 10;
            }else if (canvasWidth >= 500 && canvasWidth < 1000) {
                // try divisibility by 20, 25, 30, 35, 40, if no success, use 30
                if (size.X % 20 == 0) groupBy = 20;
                else if (size.X % 25 == 0) groupBy = 25;
                else if (size.X % 30 == 0) groupBy = 30;
                else if (size.X % 35 == 0) groupBy = 35;
                else if (size.X % 40 == 0) groupBy = 40;
                else groupBy = 30;
            }else if (canvasWidth >= 1000 && canvasWidth < 2000) {
                // try divisibility by 50, 60, 70, 80, 90, if no success, use 50
                if (size.X % 50 == 0) groupBy = 50;
                else if (size.X % 60 == 0) groupBy = 60;
                else if (size.X % 70 == 0) groupBy = 70;
                else if (size.X % 80 == 0) groupBy = 80;
                else if (size.X % 90 == 0) groupBy = 90;
                else groupBy = 50;
            }else if (canvasWidth >= 2000) {
                // try divisibility by 100, 200, 300, 400, 500, if no success, use 100
                if (size.X % 100 == 0) groupBy = 100;
                else if (size.X % 200 == 0) groupBy = 200;
                else if (size.X % 300 == 0) groupBy = 300;
                else if (size.X % 400 == 0) groupBy = 400;
                else if (size.X % 500 == 0) groupBy = 500;
                else groupBy = 100;
            }
            return groupBy;
        }
    }
}
