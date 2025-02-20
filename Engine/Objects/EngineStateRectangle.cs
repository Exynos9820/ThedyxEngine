using System.Numerics;
using Microsoft.Maui.Controls.Shapes;
using Newtonsoft.Json;
using ThedyxEngine.Engine.Managers;
using ThedyxEngine.UI;
using ThedyxEngine.Util;

namespace ThedyxEngine.Engine;

/**
 * \class EngineStateRectangle
 * \brief Represents a rectangle of state squares in the simulation.
 *
 * The EngineStateRectangle class represents a rectangle of state squares in the simulation.
 * The difference with EngineRectangle that the aggregate state of the squares can be changed.
 */
public class EngineStateRectangle : EngineObject {
    /** all squares in the rectangle */
    private List<StateGrainSquare> _grainSquares;
    /** external squares of the rectangle */
    private List<StateGrainSquare> _externalSquares;

    /**
     * \brief Constructor for creating a new EngineStateRectangle.
     * \param name The name of the engine object.
     * \param width The width of the rectangle.
     * \param height The height of the rectangle.
     */
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
                StateGrainSquare square = new($"{Name} square {i} {j}", new Point(i, j), Material);
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
     * \brief Sets the fixed temperature for the object.
     */
    private void SetFixedTemperature() {
        foreach (var square in _grainSquares) {
            square.IsTemperatureFixed = _isTemperatureFixed;
        }
    }
    
    /**
    * \brief Sets the gas state allowed for the object.
    */
    private void SetIsGasStateAllowed() {
        foreach (var square in _grainSquares) {
            square.IsGasStateAllowed = _isGasStateAllowed;
        }
    }   
    
    /**
     * \brief OnPropertyChanged
     * Based on which property has been changed, set the parameters for the squares
     */
    protected override void OnPropertyChanged(string propertyName) {

        if(propertyName == "Size")      SetSquaresForShape();
            
        if (propertyName == "Position") SetSquaresForShape();
        
        if (propertyName == "SimulationTemperature") SetTemperatureForAllSquares();
        
        if (propertyName == "IsTemperatureFixed") SetFixedTemperature();
        
        if (propertyName == "IsGasStateAllowed") SetIsGasStateAllowed();

        // call base method
        base.OnPropertyChanged(propertyName);
    }
    
    /**
     * \brief Get the polygons for the canvas
     * \param canvasManager The canvas manager.
     * \param rects The list of rectangles.
     * \param temperatures The list of temperatures.
     * \param opacities The list of opacities.
     */
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
                                StateGrainSquare.MaterialState.Solid => 1f,
                                StateGrainSquare.MaterialState.Liquid => 0.3f,
                                StateGrainSquare.MaterialState.Gas => 0.1f,
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

    /**
     * \brief Get the squares of the object.
     * \return The list of squares.
     */
    public override List<GrainSquare> GetSquares() {
        return [.._grainSquares];
    }

    /**
     * \brief Determines if the object is visible on the given canvas.
     * \param canvasManager The canvas manager.
     * \return True if the object is visible, false otherwise.
     */
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
    
    /**
     * \brief Get the visible area of the object.
     * \param topLeft The top left corner of the object.
     * \param bottomRight The bottom right corner of the object.
     */
    public override void GetObjectVisibleArea(out Vector2 topLeft, out Vector2 bottomRight) {
        topLeft = new Vector2((float)_position.X, (float)_position.Y);
        bottomRight = new Vector2((float)(_position.X + _size.X), (float)(_position.Y + _size.Y));
    }

    /**
     * \brief Set the starting temperature for the simulation.
     */
    public override void SetStartTemperature() {
        _currentTemperature = _simulationTemperature;
        OnPropertyChanged(nameof(CurrentTemperature));
        SetTemperatureForAllSquares();
    }
    
    /**
     * \brief Get the object type.
     * \return The object type.
     */
    public override ObjectType GetObjectType() {
        return ObjectType.StateRectangle;
    }
    
    
    /**
     * \brief Get the JSON representation of the object.
     * \return The JSON representation.
     */
    public override string GetJsonRepresentation() {
        var settings = new JsonSerializerSettings {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };

        return JsonConvert.SerializeObject(new {
            Type = ObjectType.StateRectangle.ToString(),
            Name,
            Position = _position,
            Size = _size,
            SimulationTemperature = _simulationTemperature,
            MaterialName = _material.Name,
            IsTemperatureFixed = _isTemperatureFixed,
            IsGasStateAllowed = _isGasStateAllowed
        }, settings);    
    }
    
    /**
     * \brief Determines if the object is intersecting with another object.
     * \param obj The object to check for intersection.
     * \return True if the objects are intersecting, false otherwise.
     */
    public override bool IsIntersecting(EngineObject obj) {
        return false;
    }

    public override List<GrainSquare> GetExternalSquares() {
        return new List<GrainSquare>(_externalSquares);
    }
    
    /**
     * \brief Apply the energy delta to the squares.
     */
    public override void ApplyEnergyDelta() {
        if(_isTemperatureFixed) return;
        foreach (var square in _grainSquares) {
            square.ApplyEnergyDelta();
        }
    }
}