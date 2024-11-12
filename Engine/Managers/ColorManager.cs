using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThedyxEngine.Engine.Managers {
    /**
    * \class ColorManager
    * \brief Manager used to work with color
    */
    public static class ColorManager {
        /**
         * \brief Returns color based on the objects temperature
         * 0 - 100 K is violet
         * 100 - 200 K is blue
         * 200 - 250 K is light blue
         * 250 - 320 K is green
         * 320 - 400 K is yellow
         * 400 - 800 K is orange
         * 800+ K is red
         * \param temperature Temperature of the object
         * \return Brush with the color
         */
        public static SolidColorBrush GetColorFromTemperature(double temperature) {
            if (temperature < 0) throw new ArgumentException("Temperature cannot be less than 0");
            // Define colors for each temperature range
            var colors = new[] {
                Color.FromRgb(143, 0, 255), // Violet, 0 K
                Color.FromRgb(0, 0, 255),   // Blue, 100 K
                Color.FromRgb(173, 216, 230), // Light Blue, 200 K
                Color.FromRgb(0, 128, 0),   // Green, 250 K
                Color.FromRgb(255, 255, 0), // Yellow, 320 K
                Color.FromRgb(255, 165, 0), // Orange, 400 K
                Color.FromRgb(255, 69, 0),  // Red, 800 K
            };

            // Define temperature thresholds
            var thresholds = new[] { 0, 100, 200, 250, 320, 400, 800 };

            // Find the position in the gradient
            int index = 0;
            for (int i = 0; i < thresholds.Length - 1; i++) {
                if (temperature >= thresholds[i] && temperature < thresholds[i + 1]) {
                    index = i;
                    break;
                }
                if (temperature >= thresholds[thresholds.Length - 1]) {
                    return new SolidColorBrush(colors[colors.Length - 1]); // Return red for 800+ K
                }
            }

            // Calculate interpolation factor between the current and next color
            double range = thresholds[index + 1] - thresholds[index];
            double factor = (temperature - thresholds[index]) / range;

            // Interpolate between the current and next color
            Color startColor = colors[index];
            Color endColor = colors[index + 1];
            byte red = (byte)((startColor.Red + (endColor.Red - startColor.Red) * factor) * 256);  
            byte green = (byte)((startColor.Green + (endColor.Green - startColor.Green) * factor) * 256);
            byte blue = (byte)((startColor.Blue + (endColor.Blue - startColor.Blue) * factor) * 256);

            // Return the brush with the interpolated color
            return new SolidColorBrush(Color.FromRgb(red, green, blue));
        }
    }
}
