namespace ThedyxEngine.Util;

public static class GlobalVariables {
    /** Grid step in meters*/
    public static double GridStep = 0.01;
    /** Depth of looking for radiation squares */
    public static int RadiationDepth = 10; 
    /** Air temperature in Kelvin*/
    public static double AirTemperature = 293;
    /** Air thermal conductivity */
    public static double AirThermalConductivity = 0.025;
    /** Stefan-Boltzmann constant */
    public static readonly double StefanBoltzmannConst = 5.67 * Math.Pow(10, -8);
    /** Engine update interval */
    public static double  EngineIntervalUpdatePerSecond = 60; 
    /** Interval between UI updates */
    public static int WindowRefreshRate = 60;
    /** Program major version */
    public static readonly int MajorVersion = 0;
    /** Program minor version */
    public static readonly int MinorVersion = 1;
    /** Program patch version */
    public static bool SaveSimulationHumanReadable = false;
    /** If engine is going to wait to be in time with real time */
    public static bool WaitToBeInTime = false;
    /** If objects should loose heat to air */
    public static bool ObjectsLooseHeatToAir = true;
    /** The lower limit of the temperature color scale */
    public static int MinTemperatureColor = 290;
    /** The upper limit of the temperature color scale */
    public static int MaxTemperatureColor = 400;
}