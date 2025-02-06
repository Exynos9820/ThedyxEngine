namespace ThedyxEngine.Util;

public static class GlobalVariables {
    /// Grid step
    public static double GridStep = 0.01;
    // Depth of looking for radiation squares
    public static int RadiationDepth = 10; 
    /// Air temperature in Kelvin
    public static double AirTemperature = 293;
    /// Stefan-Boltzmann constant
    public static readonly double StefanBoltzmannConst = 5.67 * Math.Pow(10, -8);
    /// Interval between engine updates
    public static double  EngineIntervalUpdatePerSecond = 60; 
}