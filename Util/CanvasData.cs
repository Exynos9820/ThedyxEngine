using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThedyxEngine.Util{
    /**
     * \class CanvasData
     * \brief Provides data for the canvas.
     * 
     * The CanvasData class provides data for the canvas.
     */
    static class CanvasData {
        // X > 0 parameters
        public static int MinLeftXIndex = -1000;  // Minimum left X index
        public static int MaxLeftXIndex = -10;    // Maximum left X index
        
        // X < 0 parameters
        public static int MaxRightXIndex = 1000;  // Maximum right X index
        public static int MinRightXIndex = 10;    // Minumum right X index

        // Y > 0 parameters
        public static int MaxYTopIndex = 1000;    // Maximum top Y index
        public static int MinYTopIndex = 10;      // Minimum top Y index

        // Y < 0 parameters
        public static int MinYBottomIndex = -1000;// Minimum bottom Y index
        public static int MaxYBottomIndex = -10;  // Maximum bottom Y index

    }
}
