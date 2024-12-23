using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Maui.Controls.Shapes;
using ThedyxEngine.Engine;
using ThedyxEngine.Engine.Managers;
using ThedyxEngine.UI;
using ThedyxEngine.Util;

namespace ThedyxEngine.Engine
{
    /**
     * \class GrainSquare
     * \brief Represents a square grain object within the simulation engine.
     *
     * The GrainSquare class extends \ref EngineObject and encapsulates the properties
     * and behavior of a square-shaped grain in the simulation, including thermal properties,
     * position, and selection state. It includes methods for rendering, visibility checks, and serialization.
     *
     * \see EngineObject
     * \see CanvasManager
     */
    public class GrainSquare : EngineObject
    {
        protected double EnergyDelta = 0; // current energy delta
        public event PropertyChangedEventHandler? PositionChanged; // event triggered when position changes
        private List<GrainSquare> _adjacentSquares = []; // list of adjacent squares
        private HashSet<GrainSquare> _radiationExchangeSquares = [];
        // lock for applying energy delta
        protected readonly object EnergyLock = new();

        private static readonly ILog log = LogManager.GetLogger(typeof(GrainSquare));

        /**
         * Constructs a Grainsquare with specified vertices and name.
         * \param name The name of the grain square.
         * \param p_a Vertex A of the square.
         * \param p_b Vertex B of the square.
         * \param p_c Vertex C of the square.
         */
        public GrainSquare(string name, Point position) : base(name)
        {
            _position = position;
            SetCachedPoints();
            Material = MaterialManager.GetBaseMaterial();
        }


        // Cached points of square to not to allocate anything during the runtime
        protected Point _cachedPointB = new(0, 0); // right top corner
        protected Point _cachedPointC = new(0, 0); // left bottom corner
        protected Point _cachedPointD = new(0, 0); // right bottom corner

        /**
         * Generates the polygons that visually represent the square.
         * This method overrides the abstract method defined in \ref EngineObject.
         * \return List of polygons constituting the square's visual representation.
         */
        public override void GetPolygons(CanvasManager canvasManager, out List<Polygon> polygons, out List<double> temperatures)
        {
            polygons = [];
            temperatures = [];
            Polygon polygon = new();
            polygon.Points.Add(_position);
            polygon.Points.Add(_cachedPointB);
            polygon.Points.Add(_cachedPointD);
            polygon.Points.Add(_cachedPointC);

            if (!IsSelected)
                polygon.Fill = ColorManager.GetColorFromTemperature(_currentTemperature);
            else
            {
                polygon.Stroke = SolidColorBrush.Black;
                polygon.StrokeThickness = 3;
                polygon.Fill = ColorManager.GetColorFromTemperature(_currentTemperature);
                if (Engine.Mode != Engine.EngineMode.Running)
                    polygon.Opacity = 0.5;
            }

            polygons.Add(polygon);
            temperatures.Add(_currentTemperature);
        }

        /**
         * \brief Sets caeched point of the GrainSquare
         */
        private void SetCachedPoints()
        {
            _cachedPointB = new(Position.X + 1, Position.Y);
            _cachedPointC = new(Position.X, Position.Y - 1);
            _cachedPointD = new(Position.X + 1, Position.Y - 1);
        }
        
        /**
         * Sets the material of the grain square. Does nothing for grain squares.
         */
        protected override void SetMaterialProperties() { }
        
        /**
         * Gets or sets the position of the grain square.
         */
        public override Point Position
        {
            get => _position;
            set
            {
                _position = value;
                SetCachedPoints();
                OnPositionChanged(nameof(Position));
                OnPropertyChanged(nameof(Position));
            }
        }


        /**
         * Determines whether any of the square's vertices are visible in the current view.
         * \param canvasManager The canvas manager providing the current view context.
         * \return True if any vertex is visible, otherwise false.
         */
        public override bool IsVisible(CanvasManager canvasManager)
        {
            //return CanvasManager.isPointVisible(Position, canvasManager);
            return true;
        }

        /**
         * Calculates the bounding box that encompasses the square.
         * \param[out] topLeft The top-left corner of the bounding box.
         * \param[out] bottomRight The bottom-right corner of the bounding box.
         */
        public override void GetObjectVisibleArea(out Vector2 topLeft, out Vector2 bottomRight)
        {
            topLeft = new Vector2((float)_position.X, (float)_position.Y);
            bottomRight = new Vector2((float)_cachedPointD.X, (float)_cachedPointD.Y);
        }

        /**
         * Add energy to the grain square that was calculated in one simulation step
         * \param energyDelta The energy to add to the grain square.
         */
        public void AddEnergyDelta(double energyDelta)
        {
            lock (EnergyLock)
                EnergyDelta += energyDelta;
        }

        /**
         * Applies the energy delta to the grain square, updating the temperature.
         */
        public void ApplyEnergyDelta()
        {
            // lock to be accessed by one thread at a time
            lock (EnergyLock) {
                double tempDelta = EnergyDelta / Const.GridStep / Const.GridStep /
                                   _material.SolidSpecificHeatCapacity / _material.SolidDensity;
                CurrentTemperature = _currentTemperature + tempDelta;
                CurrentTemperature = Math.Max(0, CurrentTemperature);
                EnergyDelta = 0;
            }
        }

        /**
         * Sets the initial temperature of the grain to the simulation temperature.
         */
        public override void SetStartTemperature()
        {
            _currentTemperature = _simulationTemperature;
            OnPropertyChanged(nameof(CurrentTemperature));
        }

        /**
         * Provides the type identifier for Grainsquare objects.
         * \return A string identifier for the type.
         */
        public override string GetObjectTypeString()
        {
            return "GrainSquare";
        }


        /**
         * Deserializes a JSON string to a GrainSquare object.
         * \param json The JSON string to deserialize.
         * \return A new GrainSquare object deserialized from the JSON string.
         */
        public static GrainSquare FromJson(string json)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var jObject = JsonConvert.DeserializeObject<dynamic>(json, settings);

            string type = jObject.Type;
            if (type != "GrainSquare")
                throw new InvalidOperationException("JSON is not of type Grainsquare.");
            Point Position = Util.Parsers.ParsePoint(jObject.Position.ToString());

            string name = jObject.Name;
            double simulationTemperature = (double)jObject.SimulationTemperature;
            Material Material = MaterialManager.GetMaterialByName((string)jObject.MaterialName);
            return new GrainSquare(name, Position)
            {
                _simulationTemperature = simulationTemperature,
                _currentTemperature = simulationTemperature,
                _material = Material
            };
        }


        /**
         * Serializes the grain square to a JSON representation.
         * \return A JSON string representing the grain square.
         */
        public override string GetJsonRepresentation()
        {
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

        /**
         * \brief Determines whether the grain square is intersecting with another object.
         * \returns true if the grain square is intersecting with the object, otherwise false.
         */
        public override bool IsIntersecting(EngineObject obj)
        {
            throw new NotImplementedException();
        }

        /**
         * \brief Gets the type of the object.
         * \returns The type of the object.
         */
        public override ObjectType GetObjectType()
        {
            return ObjectType.GrainSquare;
        }

        /**
         *
         * \brief Event handler for when the position of the grain square changes.
         * \param propertyName The name of the property that changed.
         */
        protected void OnPositionChanged(string propertyName)
        {
            if (Engine.Mode != Engine.EngineMode.Running)
            {
                SetCachedPoints();
                PositionChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /**
         * \brief Determines whether the grain square is touching another grain square.
         * \param other The other grain square to check for touching.
         * \returns true if the grain square is touching the other grain square, otherwise false.
         */
        public bool AreTouching(GrainSquare other)
        {
            // check the object is not null and that the two squares are not the same
            if (other == null || this.Name == other.Name)
            {
                return false;
            }

            bool xTouch = Math.Abs(this.Position.X - other.Position.X) == 1 && this.Position.Y == other.Position.Y;
            bool yTouch = Math.Abs(this.Position.Y - other.Position.Y) == 1 && this.Position.X == other.Position.X;

            return xTouch || yTouch;
        }

        /*
         * \brief Gets the squares that are adjacent to the grain square.
         * \returns A list of adjacent squares.
         */
        public override List<GrainSquare> GetSquares()
        {
            List<GrainSquare> grainsquares = [this];
            return grainsquares;
        }

        /**
         * \brief Adds an adjacent square to the grain square.
         * \param square The adjacent square to add.
         */
        public void AddAdjacentSquare(GrainSquare square) {
            _adjacentSquares.Add(square);
        }

        /**
         * \brief Adds a square to the squares to exchange with radiation heat.
         * \param square The square to add.
         */
        public void AddRadiationExchangeSquare(GrainSquare square) {
            _radiationExchangeSquares.Add(square);
        }

        /**
         * \brief Clears the list of squares to optimize performance
         */
        public void ClearOptimizationSquares() {
            _adjacentSquares.Clear();
            _radiationExchangeSquares.Clear();
        }

        /**
         * \brief Gets the external squares of the grain square.
         * \returns A list of external squares.
         */
        public override List<GrainSquare> GetExternalSquares() {
            return new List<GrainSquare> { this };
        }

        /**
         * \brief Gets the adjacent squares of the grain square.
         * \returns A list of adjacent squares.
         */
        public List<GrainSquare> GetAdjacentSquares() {
            return _adjacentSquares;
        }
        
        /**
         * \brief Gets the adjacent squares of the grain square.
         * \returns A list of adjacent squares.
         */
        public HashSet<GrainSquare> GetRadiationSquares() {
            return _radiationExchangeSquares;
        }
        
    }
}