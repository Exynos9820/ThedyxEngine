using ThedyxEngine.Engine;

namespace ThedyxEngine.UI;

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