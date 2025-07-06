using ThedyxEngine.Engine.Managers;
using ThedyxEngine.Engine.Objects;

/**
 * \namespace ThedyxEngine.Engine.Dev
 * \brief Contains developer tools
 */
namespace ThedyxEngine.Engine.Dev {
    /**
     * \class SimpleExamples
     * \brief Allows to test different scenarios
     */
    public static class SimpleExamples
    {
        public static void SetThreesquares() {
            // add 3 objects to the engine
            var obj1 = new GrainSquare("Square", new Point(0, 0)) {
                SimulationTemperature = 200
            };
            Engine.EngineObjectsManager?.AddObject(obj1);
            var obj2 = new GrainSquare("square2", new Point(1, 0)) {
                SimulationTemperature = 50
            };
            Engine.EngineObjectsManager?.AddObject(obj2);
            var obj3 = new GrainSquare("square3", new Point(2, 2));
            obj1.SimulationTemperature = 0;
            Engine.EngineObjectsManager?.AddObject(obj3);
        }
        


        public static void CreateGridOf10x10Rectangles(int columns, int rows) {
            const int w = 10, h = 10;

            // 1) grid dimensions in world coordinates
            int gridWidth  = columns * w;
            int gridHeight = rows    * h;

            // 2) find the geometric centre
            var centre = new Point(gridWidth / 2.0, gridHeight / 2.0);

            // 3) choose a temperature profile
            const double Tmin = 100;   // edge temperature
            const double Tmax = 500;   // exact centre
            double maxRadius  = Math.Sqrt(centre.X * centre.X + centre.Y * centre.Y);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    var rectName = $"Rect_{r}_{c}";
                    var position = new Point(c * w, r * h);

                    // Euclidean distance from the rectangle’s centre to grid centre
                    double dx   = position.X + w / 2.0 - centre.X;
                    double dy   = position.Y + h / 2.0 - centre.Y;
                    double dist = Math.Sqrt(dx * dx + dy * dy);

                    // 4) map distance to temperature (linear fall-off)
                    double t = Tmax - (Tmax - Tmin) * (dist / maxRadius);

                    var rect = new EngineRectangle(rectName, w, h)
                    {
                        Position            = position,
                        SimulationTemperature = t
                    };

                    Engine.EngineObjectsManager?.AddObject(rect);
                }
            }
        }


    public static void TwoEngineRectangles() {
            var e1 = new EngineRectangle("Rectangle1", 30, 30) {
                Position = new Point(0, 0)
            };
            Engine.EngineObjectsManager?.AddObject(e1);
            var e2 = new EngineRectangle("Rectangle2", 20, 20) {
                SimulationTemperature = 1000,
                Position = new Point(30, 0)
            };
            Engine.EngineObjectsManager?.AddObject(e2);
        }
        
        public static void IceMeltingFromHotAluminium() {
            // Create a hot aluminium object
            var aluminium = new EngineRectangle("Bottom wall", 70, 20) {
                Position = new Point(-20, 10),
                SimulationTemperature = 5000
            };
            Engine.EngineObjectsManager?.AddObject(aluminium);
            
            // add left and right walls
            var leftWall = new EngineRectangle("LeftWall", 20, 30) {
                Position = new Point(-20, 30),
                SimulationTemperature = 5000
            };
            Engine.EngineObjectsManager?.AddObject(leftWall);
            var rightWall = new EngineRectangle("RightWall", 20, 30) {
                Position = new Point(30, 30),
                SimulationTemperature = 5000
            };
            Engine.EngineObjectsManager?.AddObject(rightWall);
            
            // add top wall
            var topWall = new EngineRectangle("TopWall", 70, 20) {
                Position = new Point(-20, 60),
                SimulationTemperature = 5000
            };
            Engine.EngineObjectsManager?.AddObject(topWall);
            
            // Create an ice object
            EngineStateRectangle ice = new EngineStateRectangle("Ice", 30, 30) {
                Position = new Point(0, 30),
                SimulationTemperature = 220
            };
            Engine.EngineObjectsManager?.AddObject(ice);
            ice.Material = MaterialManager.GetMaterialByName("Water");
            
        }

        public static void RectangleWithTempDifference(int width, int height) {
            var maxTemperature = 1000.0;

            // Calculate the center position
            var centerX = width / 2.0;
            var centerY = height / 2.0;

            // Create each square in the grid
            for (var x = 0; x < width; x++) {
                for (var y = 0; y < height; y++) {
                    // Calculate the centroid of the square
                    var centroid = new Point(x + 0.5, y + 0.5);

                    // Calculate distance from the center
                    var distance = Math.Sqrt(Math.Pow(centroid.X - centerX, 2) + Math.Pow(centroid.Y - centerY, 2));
                    var maxDistance = Math.Sqrt(Math.Pow(centerX, 2) + Math.Pow(centerY, 2));

                    // Assign temperature inversely proportional to the distance
                    var temperature = maxTemperature * (1 - (distance / maxDistance));

                    // Define points for the square (assuming each cell is a right square)
                    var p1 = new Point(x, y);

                    // Create square
                    var square = new GrainSquare($"Square_{x}_{y}", p1) {
                        SimulationTemperature = temperature
                    };


                    // Add to the engine
                    Engine.EngineObjectsManager?.AddObject(square);
                }
            }
        }
    }
}
