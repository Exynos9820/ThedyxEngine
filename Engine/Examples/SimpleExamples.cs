using ThedyxEngine.Engine.Managers;
using ThedyxEngine.Engine.Objects;

namespace ThedyxEngine.Engine.Examples {
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

        public static void SetManyRectangles(int numberHeight, int numberWidth, int height, int width) {
            // add 3 objects to the engine
            for (int i = 0; i < numberHeight; i++) {
                for (int j = 0; j < numberWidth; j++) {
                    var obj = new EngineRectangle($"Rectangle {i} {j}", height, width) {
                        Position = new Point(i * height, j * width),
                        SimulationTemperature = 200
                    };
                    Engine.EngineObjectsManager?.AddObject(obj);
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
