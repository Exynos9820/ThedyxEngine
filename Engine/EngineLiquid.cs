using System.Numerics;
using Microsoft.Maui.Controls.Shapes;
using ThedyxEngine.UI;

namespace ThedyxEngine.Engine;

public class EngineLiquid : EngineObject {
    private List<EngineGrainLiquid> _grainSquares;
    private List<EngineGrainLiquid> _externalSquares;

    public EngineLiquid(string name, int width, int height) : base(name) {
        
    }

    
    
    protected override void SetMaterialProperties() {
        throw new NotImplementedException();
    }

    public override void GetPolygons(CanvasManager canvasManager, out List<Polygon> polygons, out List<double> temperatures) {
        throw new NotImplementedException();
    }

    public override List<GrainSquare> GetSquares() {
        throw new NotImplementedException();
    }

    public override bool IsVisible(CanvasManager canvasManager) {
        throw new NotImplementedException();
    }

    public override void GetObjectVisibleArea(out Vector2 topLeft, out Vector2 bottomRight) {
        throw new NotImplementedException();
    }

    public override void SetStartTemperature() {
        throw new NotImplementedException();
    }

    public override string GetObjectTypeString() {
        throw new NotImplementedException();
    }

    public override ObjectType GetObjectType() {
        throw new NotImplementedException();
    }

    public override string GetJsonRepresentation() {
        throw new NotImplementedException();
    }

    public override bool IsIntersecting(EngineObject obj) {
        throw new NotImplementedException();
    }

    public override List<GrainSquare> GetExternalSquares() {
        throw new NotImplementedException();
    }
}