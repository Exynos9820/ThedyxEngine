namespace ThedyxEngine.Util;

public static class Const {
    public const double GridStep = 0.001; // grid step in meters 
    public const int RadiationDepth = 10; // depth of looking for radiation squares
    /// Stefan-Boltzmann constant
    public static readonly double StefanBoltzmannConst = 5.67 * Math.Pow(10, -8);
    /// Interval between engine updates
    public static  readonly double  EngineIntervalUpdate = 1000/60; 
}