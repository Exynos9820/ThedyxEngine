using ThedyxEngine.Engine.Objects;

namespace ThedyxEngine.Engine.Managers;

/**
 * \interface IHeatTransferManager
 * \brief Provides an interface for heat transfer managers.
 */
public interface IHeatTransferManager {
    /**
     *
     * Method to transfer heat for all objects
     * \param objects list of objects
     */
    public void TransferHeat(List<EngineObject> objects);
}