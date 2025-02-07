using System.Numerics;
using Microsoft.Maui.Controls.Shapes;
using Newtonsoft.Json;
using ThedyxEngine.Engine.Managers;
using ThedyxEngine.UI;
using ThedyxEngine.Util;

namespace ThedyxEngine.Engine;

public class EngineStateRectangle : EngineObject {
    private List<EngineStateGrainSquare> _grainSquares;
    private List<EngineStateGrainSquare> _externalSquares;

    public EngineStateRectangle(string name, int width, int height) : base(name) {
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
                EngineStateGrainSquare square = new($"{Name} square {i} {j}", new Point(i, j), Material);
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

    public override void GetPolygons(CanvasManager canvasManager, out List<RectF> rects, out List<double> temperatures, out List<float> opacities) {
            rects = [];
            temperatures = [];
            opacities = [];
            //  understand how far are we from canvas
            // we will check it by width
            var canvasWidth = canvasManager.CurrentRightXIndex - canvasManager.CurrentLeftXIndex;
            var groupBy = CanvasHelper.GetGroubByValue(canvasWidth, Size);


            for(var i = 0; i < Size.X; i+= groupBy) {
                for(var j = 0; j < Size.Y; j+= groupBy) {
                    // iterate through all squares in this group, take average temperature and create polygon
                    var points = new PointCollection();
                    var temperature = 0.0;
                    float opacity = 0.0f;
                    // then we take the position of the first square and create a polygon
                    for(var x = i; x < i + groupBy && x < Size.X; x++) {
                        for(var y = j; y < j + groupBy && y < Size.Y; y++) {
                            var square = _grainSquares[x * (int)Size.Y + y];
                            temperature += square.CurrentTemperature;
                            opacity += square.CurrentMaterialState switch {
                                EngineStateGrainSquare.MaterialState.Solid => 1f,
                                EngineStateGrainSquare.MaterialState.Liquid => 0.3f,
                                EngineStateGrainSquare.MaterialState.Gas => 0.1f,
                                _ => 0
                            };
                        }
                    }
                    // check if the last group was smaller than groupBy
                    int groupByX = (int)Math.Min(groupBy, Size.X - i);
                    int groupByY = (int)Math.Min(groupBy, Size.Y - j);
                    temperature /= groupByX * groupByY;
                    // get the position of the first square
                    var firstSquare = _grainSquares[i * (int)Size.Y + j];
                    var rect = new RectF((float)firstSquare.Position.X, (float)firstSquare.Position.Y, groupByX, groupByY);
                    
                    opacities.Add(opacity / (groupByX * groupByY));
                    rects.Add(rect);
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
       return "StateObject";
    }

    public override ObjectType GetObjectType() {
        return ObjectType.StateChangeRectangle;
    }
    
    

    public override string GetJsonRepresentation() {
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };

        return JsonConvert.SerializeObject(new
        {
            Type = GetObjectTypeString(),
            Name,
            Position = _position,
            SimulationTemperature = _simulationTemperature,
            MaterialName = _material.Name
        }, settings);    
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