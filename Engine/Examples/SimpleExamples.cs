using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThedyxEngine.Engine.Managers;

namespace ThedyxEngine.Engine.Examples {
    public static class SimpleExamples
    {
        public static void SetThreesquares() {
            // add 3 objects to the engine
            GrainSquare obj1 = new GrainSquare("Square", new Point(0, 0));
            obj1.SimulationTemperature = 200;
            Engine.EngineObjectsManager.AddObject(obj1);
            GrainSquare obj2 = new GrainSquare("square2", new Point(1, 0));
            obj2.SimulationTemperature = 50;
            Engine.EngineObjectsManager.AddObject(obj2);
            GrainSquare obj3 = new GrainSquare("square3", new Point(2, 2));
            obj1.SimulationTemperature = 0;
            Engine.EngineObjectsManager.AddObject(obj3);
        }

        public static void SetManyRectangles(int numberHeight, int numberWidth, int height, int width) {
            // add 3 objects to the engine
            for (int i = 0; i < numberHeight; i++) {
                for (int j = 0; j < numberWidth; j++) {
                    EngineRectangle obj = new EngineRectangle($"Rectangle {i} {j}", height, width);
                    obj.Position = new Point(i * height, j * width);
                    obj.SimulationTemperature = 200;
                    Engine.EngineObjectsManager.AddObject(obj);
                }
            }
        }

    public static void TwoEngineRectangles() {
            EngineRectangle e1 = new EngineRectangle("Rectangle1", 30, 30);
            e1.Position = new Point(0, 0);
            Engine.EngineObjectsManager.AddObject(e1);
            EngineRectangle e2 = new EngineRectangle("Rectangle2", 20, 20);
            e2.SimulationTemperature = 1000;
            e2.Position = new Point(30, 0);
            Engine.EngineObjectsManager.AddObject(e2);
        }
        
        public static void IceMeltingFromHotAluminium() {
            // Create a hot aluminium object
            EngineRectangle aluminium = new EngineRectangle("Bottom wall", 70, 20);
            aluminium.Position = new Point(-20, 10);
            aluminium.SimulationTemperature = 5000;
            Engine.EngineObjectsManager.AddObject(aluminium);
            
            // add left and right walls
            EngineRectangle leftWall = new EngineRectangle("LeftWall", 20, 30);
            leftWall.Position = new Point(-20, 30);
            leftWall.SimulationTemperature = 5000;
            Engine.EngineObjectsManager.AddObject(leftWall);
            EngineRectangle rightWall = new EngineRectangle("RightWall", 20, 30);
            rightWall.Position = new Point(30, 30);
            rightWall.SimulationTemperature = 5000;
            Engine.EngineObjectsManager.AddObject(rightWall);
            
            // add top wall
            EngineRectangle topWall = new EngineRectangle("TopWall", 70, 20);
            topWall.Position = new Point(-20, 60);
            topWall.SimulationTemperature = 5000;
            Engine.EngineObjectsManager.AddObject(topWall);
            
            // Create an ice object
            EngineStateRectangle ice = new EngineStateRectangle("Ice", 30, 30);
            ice.Position = new Point(0, 30);
            ice.SimulationTemperature = 220;
            Engine.EngineObjectsManager.AddObject(ice);
            ice.Material = MaterialManager.GetMaterialByName("Water");
            
        }

        public static void RectangleWithTempDifference(int width, int height) {
            double maxTemperature = 1000.0;

            // Calculate the center position
            double centerX = width / 2.0;
            double centerY = height / 2.0;

            // Create each square in the grid
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    // Calculate the centroid of the square
                    Point centroid = new Point(x + 0.5, y + 0.5);

                    // Calculate distance from the center
                    double distance = Math.Sqrt(Math.Pow(centroid.X - centerX, 2) + Math.Pow(centroid.Y - centerY, 2));
                    double maxDistance = Math.Sqrt(Math.Pow(centerX, 2) + Math.Pow(centerY, 2));

                    // Assign temperature inversely proportional to the distance
                    double temperature = maxTemperature * (1 - (distance / maxDistance));

                    // Define points for the square (assuming each cell is a right square)
                    Point p1 = new Point(x, y);

                    // Create square
                    GrainSquare square = new GrainSquare($"Square_{x}_{y}", p1);
                    square.SimulationTemperature = temperature;


                    // Add to the engine
                    Engine.EngineObjectsManager.AddObject(square);
                }
            }
        }
    }
}
