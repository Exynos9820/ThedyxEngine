using ThedyxEngine.Util;

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
        public static Color GetColorFromTemperature(double temperature) {
            if (temperature < 0) throw new ArgumentException("Temperature cannot be less than 0");
            var colors = new[] {
                Color.FromRgb(0.0039215686274509665, 0.0, 0.996078431372549),
                Color.FromRgb(0.2196078431372549, 0.0, 0.7803921568627451),
                Color.FromRgb(0.4117647058823529, 0.0, 0.5882352941176471),
                Color.FromRgb(0.611764705882353, 0.0, 0.38823529411764707),
                Color.FromRgb(0.807843137254902, 0, 0.19215686274509802), 
                Color.FromRgb(0.996078431372549, 0.0, 0.003921),
            };
            
            // Define temperature thresholds
            // get minimum and maximum temperatures
            double min = GlobalVariables.MinTemperatureColor;
            double max = GlobalVariables.MaxTemperatureColor;
            
            if (temperature < min)
                return colors[0]; // Return violet for 0-130 K
            
            if (temperature > max)
                return colors[^1]; // Return red for 800+ K
            
            // make threadsholds from min to max, divide by 6
            var thresholds = new[] { min, min + (max - min) / 6, min + 2 * (max - min) / 6, min + 3 * (max - min) / 6, min + 4 * (max - min) / 6, min + 5 * (max - min) / 6 };
            
            // Find the position in the gradient
            int index = 0;
            for (int i = 0; i < thresholds.Length - 1; i++) {
                if (temperature >= thresholds[i] && temperature < thresholds[i + 1]) {
                    index = i;
                    break;
                }
                if (temperature >= thresholds[thresholds.Length - 1]) {
                    return  (colors[colors.Length - 1]); // Return red for 800+ K
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
            return Color.FromRgb(red, green, blue);
        }
    }
}
