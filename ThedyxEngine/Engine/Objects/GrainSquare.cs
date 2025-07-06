﻿using System.ComponentModel;
using System.Numerics;
using log4net;
using Newtonsoft.Json;
using ThedyxEngine.Engine.Managers;
using ThedyxEngine.UI;
using ThedyxEngine.Util;

namespace ThedyxEngine.Engine.Objects {
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
    public class GrainSquare : EngineObject {
        /**
         * Current energy delta
         */
        protected double EnergyDelta = 0;
        
        /**
         * event triggered when position changes
         */
        public event PropertyChangedEventHandler? PositionChanged;
        
        /**
         * List of neighboring squares to exchanges heat created by @ref ThedyxEngine.Engine.Managers.NeighboursOptimizer "NeighboursOptimizer"
         */
        private List<GrainSquare> _adjacentSquares = [];
        
        /**
         * List of neighboring squares to exchange radiation created by @ref ThedyxEngine.Engine.Managers.RadiationOptimizer "RadiationOptimizer"
         */
        private HashSet<GrainSquare> _radiationExchangeSquares = [];
            
        /**
         * lock for applying energy delta
         */
        protected readonly object EnergyLock = new();
        
        /**
         * Logger
         */
        private static readonly ILog log = LogManager.GetLogger(typeof(GrainSquare));

        /**
         * Constructs a Grainsquare with specified vertices and name.
         * \param name The name of the grain square.
         * \param p_a Vertex A of the square.
         * \param p_b Vertex B of the square.
         * \param p_c Vertex C of the square.
         */
        public GrainSquare(string? name, Point position) : base(name) {
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
        public override void GetPolygons(CanvasManager canvasManager, out List<RectF> rects, out List<double> temperatures, out List<float> opacities) {
            rects = [];
            temperatures = [];
            opacities = [];

            var rect = new RectF((float)_position.X, (float)_position.Y, 1, 1);
            rects.Add(rect);
            opacities.Add(1);
            temperatures.Add(_currentTemperature);
        }

        /**
         * \brief Sets caeched point of the GrainSquare
         */
        protected void SetCachedPoints() {
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
        public override Point Position {
            get => _position;
            set {
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
        public override bool IsVisible(CanvasManager canvasManager) {
            //return CanvasManager.isPointVisible(Position, canvasManager);
            return true;
        }

        /**
         * Calculates the bounding box that encompasses the square.
         * \param[out] topLeft The top-left corner of the bounding box.
         * \param[out] bottomRight The bottom-right corner of the bounding box.
         */
        public override void GetObjectVisibleArea(out Vector2 topLeft, out Vector2 bottomRight) {
            topLeft = new Vector2((float)_position.X, (float)_position.Y);
            bottomRight = new Vector2((float)_cachedPointD.X, (float)_cachedPointD.Y);
        }

        /**
         * Add energy to the grain square that was calculated in one simulation step
         * \param energyDelta The energy to add to the grain square.
         */
        public void AddEnergyDelta(double energyDelta) {
            lock (EnergyLock)
                EnergyDelta += energyDelta;
        }

        /**
         * Applies the energy delta to the grain square, updating the temperature.
         */
        public override void ApplyEnergyDelta() {
            if(_isTemperatureFixed) return;
            // lock to be accessed by one thread at a time
            lock (EnergyLock) {
                double tempDelta = EnergyDelta / GlobalVariables.GridStep / GlobalVariables.GridStep /
                                   _material.SolidSpecificHeatCapacity / _material.SolidDensity;
                CurrentTemperature = _currentTemperature + tempDelta;
                CurrentTemperature = Math.Max(0, CurrentTemperature);
                EnergyDelta = 0;
            }
        }

        /**
         * Sets the initial temperature of the grain to the simulation temperature.
         */
        public override void SetStartTemperature() {
            _currentTemperature = _simulationTemperature;
            OnPropertyChanged(nameof(CurrentTemperature));
        }


        /**
         * Serializes the grain square to a JSON representation.
         * \return A JSON string representing the grain square.
         */
        public override string GetJsonRepresentation() {
            var settings = new JsonSerializerSettings {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };

            return JsonConvert.SerializeObject(new {
                Type = ObjectType.GrainSquare.ToString(),
                Name,
                Position = _position,
                SimulationTemperature = _simulationTemperature,
                MaterialName = _material.Name,
                IsTemperatureFixed = _isTemperatureFixed,
                IsGasStateAllowed = _isGasStateAllowed
            }, settings);
        }

        /**
         * \brief Determines whether the grain square is intersecting with another object.
         * \returns true if the grain square is intersecting with the object, otherwise false.
         */
        public override bool IsIntersecting(EngineObject obj) {
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
        public bool AreTouching(GrainSquare other) {
            // Check the object is not null and that the two squares are not the same
            if (other == null || this.Name == other.Name) {
                return false;
            }

            bool xTouch = Math.Abs(this.Position.X - other.Position.X) == 1 && this.Position.Y == other.Position.Y;
            bool yTouch = Math.Abs(this.Position.Y - other.Position.Y) == 1 && this.Position.X == other.Position.X;

            // Ensure that they are not touching at the corners
            return (xTouch || yTouch) && Math.Abs(this.Position.X - other.Position.X) < 2 && Math.Abs(this.Position.Y - other.Position.Y) < 2;
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
            // check if there is no squre with the same name
            if (_adjacentSquares.Any(s => s.Name == square.Name)) {
                return;
            }
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