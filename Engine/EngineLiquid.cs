using System.Numerics;
using Microsoft.Maui.Controls.Shapes;
using ThedyxEngine.Engine.Managers;
using ThedyxEngine.UI;

namespace ThedyxEngine.Engine;

public class EngineLiquid : EngineObject {
    private List<EngineGrainLiquid> _grainSquares;
    private List<EngineGrainLiquid> _externalSquares;

    public EngineLiquid(string name, int width, int height) : base(name) {
        _size = new(width, height);
        SetSquaresForShape();
        SetTemperatureForAllSquares();
    }

    /**
     * \brief Create squares for the shape
     */
    private void SetSquaresForShape() {
        _externalSquares = [];
        _grainSquares = [];
        for (int i = 0; i < Size.X; i++) {
            for (int j = 0; j < Size.Y; j++) {
                EngineGrainLiquid square = new($"{Name} square {i} {j}", new Point(i, j), Material);
                square.Position = new Point(_position.X + i, _position.Y + j);
                square.CurrentTemperature = _simulationTemperature;
                square.SimulationTemperature = _simulationTemperature;
                square.Material = _material;
                _grainSquares.Add(square);
                if (i == 0 || j == 0 || i == Size.X - 1 || j == Size.Y - 1) {
                    _externalSquares.Add(square);
                }
            }
        }
    }
    
    /**
     * \brief Sets the temperature for all squares.
     */
    private void SetTemperatureForAllSquares() {
        foreach (var square in _grainSquares) {
            square.SimulationTemperature = _simulationTemperature;
            square.CurrentTemperature = _simulationTemperature;
            square.SetStateFromTemperature();
        }
        CurrentTemperature = _simulationTemperature;
        SimulationTemperature = _simulationTemperature;
    }

    
    /**
     * \brief Sets the material properties of the object.
     */
    protected override void SetMaterialProperties() {
        if (_grainSquares == null) return;
        foreach (var square in _grainSquares) {
            square.Material = _material;
        }
    }
    
    /**
     * \brief OnPropertyChanged
     * Based on which property has been changed, set the parameters for the squares
     */
    protected override void OnPropertyChanged(string propertyName) {

        if(propertyName == "Size")      SetSquaresForShape();
            
        if (propertyName == "Position") SetSquaresForShape();

        // call base method
        base.OnPropertyChanged(propertyName);
    }

    public override void GetPolygons(CanvasManager canvasManager, out List<Polygon> polygons, out List<double> temperatures) {
            polygons = [];
            temperatures = [];
            //  understand how far are we from canvas
            // we will check it by width
            var canvasWidth = canvasManager.CurrentRightXIndex - canvasManager.CurrentLeftXIndex;
            var groupBy = canvasWidth switch {
                >= 50 and < 200 => 2,
                >= 200 and < 500 => 5,
                >= 1000 and < 2000 => 20,
                >= 2000 => 50,
                _ => 1
            };
            
            for(var i = 0; i < Size.X; i+= groupBy) {
                for(var j = 0; j < Size.Y; j+= groupBy) {
                    // iterate through all squares in this group, take average temperature and create polygon
                    var points = new PointCollection();
                    var temperature = 0.0;
                    var opacity = 0.0;
                    // then we take the position of the first square and create a polygon
                    for(var x = i; x < i + groupBy && x < Size.X; x++) {
                        for(var y = j; y < j + groupBy && y < Size.Y; y++) {
                            var square = _grainSquares[x * (int)Size.Y + y];
                            temperature += square.CurrentTemperature;
                            opacity += square.CurrentState switch {
                                EngineGrainLiquid.CurrentSta.Solid => 1,
                                EngineGrainLiquid.CurrentSta.Liquid => 0.5,
                                EngineGrainLiquid.CurrentSta.Gas => 0.1,
                                _ => 0
                            };
                        }
                    }
                    // check if the last group was smaller than groupBy
                    var groupByX = Math.Min(groupBy, Size.X - i);
                    var groupByY = Math.Min(groupBy, Size.Y - j);
                    temperature /= groupByX * groupByY;
                    // get the position of the first square
                    var firstSquare = _grainSquares[i * (int)Size.Y + j];
                    points.Add(new Point(firstSquare.Position.X, firstSquare.Position.Y));
                    // get the size of the group and create a bigger square
                    points.Add(new Point(firstSquare.Position.X + groupByX, firstSquare.Position.Y));
                    points.Add(new Point(firstSquare.Position.X + groupByX, firstSquare.Position.Y + groupByY));
                    points.Add(new Point(firstSquare.Position.X, firstSquare.Position.Y + groupByY));
                    // add the polygon to the list
                    var polygon = new Polygon(points);
                    if (!IsSelected)
                        polygon.Fill = ColorManager.GetColorFromTemperature(temperature);
                    else
                    {
                        polygon.Stroke = SolidColorBrush.Black;
                        polygon.StrokeThickness = 3;
                        polygon.Fill = ColorManager.GetColorFromTemperature(temperature);
                        if (Engine.Mode != Engine.EngineMode.Running)
                            polygon.Opacity = 0.5;
                    }
                    polygon.Opacity = opacity / (groupByX * groupByY);
                    polygons.Add(polygon);
                    temperatures.Add(temperature);
                }
            }
    }

    public override List<GrainSquare> GetSquares() {
        return [.._grainSquares];
    }

    public override bool IsVisible(CanvasManager canvasManager) {
        // we need to check coordinates of the canvas manager and check if there is any square in the visible area
        Vector2 topLeft, bottomRight;
        GetObjectVisibleArea(out topLeft, out bottomRight);
        // check for intersecting with canvas manager
        if (topLeft.X > canvasManager.CurrentRightXIndex || bottomRight.X < canvasManager.CurrentLeftXIndex ||
            topLeft.Y > canvasManager.CurrentTopYIndex || bottomRight.Y < canvasManager.CurrentBottomYIndex) {
            return false;
        }
        return true;
    }

    public override void GetObjectVisibleArea(out Vector2 topLeft, out Vector2 bottomRight) {
        topLeft = new Vector2((float)_position.X, (float)_position.Y);
        bottomRight = new Vector2((float)(_position.X + _size.X), (float)(_position.Y + _size.Y));
    }

    public override void SetStartTemperature() {
        _currentTemperature = _simulationTemperature;
        OnPropertyChanged(nameof(CurrentTemperature));
        SetTemperatureForAllSquares();
    }

    public override string GetObjectTypeString() {
       return "Liquid";
    }

    public override ObjectType GetObjectType() {
        return ObjectType.Liquid;
    }

    public override string GetJsonRepresentation() {
        throw new NotImplementedException();
    }

    public override bool IsIntersecting(EngineObject obj) {
        return false;
    }

    public override List<GrainSquare> GetExternalSquares() {
        return new List<GrainSquare>(_externalSquares);
    }

    public override void ApplyEnergyDelta() {
        foreach (var square in _grainSquares) {
            square.ApplyEnergyDelta();
        }
    }
}