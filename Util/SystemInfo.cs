using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace ThedyxEngine.Util {
    /**
     * \class SystemInfo    
     * \brief Provides methods for getting system information.
     * 
     * The SystemInfo class provides methods for getting system information.
     */
    internal partial class SystemInfo {
        /*private static readonly ILog log = LogManager.GetLogger(typeof(SystemInfo)); /// Logger
        /**
         * \brief Gets the refresh rate of the primary screen.
         * \return The refresh rate of the primary screen.
         */
        public static int GetRefreshRate() {
             // Get the handle to the device context (DC) for the primary screen
             /*IntPtr hdc = GetDC(IntPtr.Zero); // Passing IntPtr.Zero gets the DC for the entire screen
             if (hdc == IntPtr.Zero) {
                 return 0; // Return 0 if we fail to get the DC
             }

             // Get the vertical refresh rate
             int refreshRate = GetDeviceCaps(hdc, VERTREFRESH);

             // Release the device context
             ReleaseDC(IntPtr.Zero, hdc);

             log.Info("Current refresh rate: " + refreshRate + " Hz");

             return refreshRate;*/
             return 120;
        }

    /*


        const int VERTREFRESH = 116;        /// Vertical refresh rate

        [LibraryImport("user32.dll")]
        private static partial IntPtr GetDC(IntPtr hWnd);   /// Gets the device context (DC) for the entire screen.

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool ReleaseDC(IntPtr hWnd, IntPtr hDC); /// Releases the device context (DC) for the entire screen.

        [LibraryImport("gdi32.dll")]
        private static partial int GetDeviceCaps(IntPtr hdc, int nIndex); /// Retrieves device-specific information for the specified device context (DC).
        */
    }
}
