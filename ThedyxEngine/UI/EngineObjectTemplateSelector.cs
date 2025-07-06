using ThedyxEngine.Engine;
using ThedyxEngine.Engine.Objects;

namespace ThedyxEngine.UI;

/**
 * \class EngineObjectTemplateSelector
 * \brief Template to display list of the @ref ThedyxEngine.Engine.Objects.EngineObject "EngineObjects" used by ObjectsList
 */
public class EngineObjectTemplateSelector : DataTemplateSelector {
    public DataTemplate? ObjectTemplate { get; set; }
    public DataTemplate? LiquidObjectTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container) {
        if (item is EngineRectangle or GrainSquare) {
            return ObjectTemplate!;
        }else if (item is EngineStateRectangle) {
            return LiquidObjectTemplate!;
        }
        throw new ArgumentException("Invalid item type");
    }
}