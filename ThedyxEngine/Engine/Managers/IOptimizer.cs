/**
 * \namespace ThedyxEngine.Engine.Managers
 * \brief Contains managers for energy transfers and optimization
 */
namespace ThedyxEngine.Engine.Managers;

/**
 * \interface IOptimizer
 * \brief Provides an interface for heat optimizing managers.
 */
public interface IOptimizer {
    /**
     *
     * Method to optimize internals for all objects
     * \param objects list of objects
     * \param startPosition Starting index in list
     * \param endPosition Last index in list
     */
    public void Optimize(List<EngineObject> objects, int startPosition, int endPosition);
    
    /**
     * Method to get a name of optimize manager
     */
    public String GetName();
}