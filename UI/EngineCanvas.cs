using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using log4net;
using Microsoft.Maui.Controls.Shapes;
using TempoEngine.UI;
using ThedyxEngine.Engine;
using ThedyxEngine.Engine.Managers;

namespace ThedyxEngine.UI {
    public class EngineCanvas : IDrawable {
        private readonly CanvasManager _canvasManager;   // Canvas manager
        private readonly GridDrawer _gridDrawer;
        private static readonly ILog log = LogManager.GetLogger(typeof(EngineCanvas)); // Logger
        private MainPage _mainPage;
        public EngineCanvas(MainPage mainPage) : base() {
            _canvasManager = new CanvasManager();
            _gridDrawer = new GridDrawer();
            _mainPage = mainPage;
            var graphicsView = new GraphicsView
            {
                WidthRequest = 800,
                HeightRequest = 600,
                BackgroundColor = Colors.White
            };
        }
            
        public void Draw(ICanvas canvas, RectF dirtyRect) {
            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 2;
            canvas.FillColor = Colors.LightBlue;
            
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            _canvasManager.AdjustForAspectRatio(dirtyRect.Width, dirtyRect.Height);

            // log time
            stopwatch.Stop();
            log.Info("Time to clear canvas: " + stopwatch.ElapsedMilliseconds + " ms");
            stopwatch.Restart();

            List<EngineObject> objects = Engine.Engine.EngineObjectsManager.GetVisibleObjects(_canvasManager);
            stopwatch.Stop();
            log.Info("Time to get visible objects: " + stopwatch.ElapsedMilliseconds + " ms");
            stopwatch.Restart();


            if (Engine.Engine.ShowGrid) {
                _gridDrawer.DrawGrid(canvas, _canvasManager, dirtyRect);
                stopwatch.Stop();
                log.Info("Time to draw grid: " + stopwatch.ElapsedMilliseconds + " ms");
                stopwatch.Restart();
            }

            // get polygons
            foreach (var obj in objects) {
                obj.GetPolygons(_canvasManager, out var polygons, out var temperatures, out var opacities);
                // convert polygon points to screen coordinates
                for (int j = 0; j < polygons.Count; j++) {
                    RectF polygon = polygons[j];
                    double temp = temperatures[j];
                    float opacity = opacities[j];
                    if (!Engine.Engine.ShowColor) {
                        canvas.FillColor = ColorManager.GetColorFromTemperature(temp);
                        canvas.StrokeColor = ColorManager.GetColorFromTemperature(temp);
                        canvas.StrokeSize = 2;
                        canvas.Alpha = opacity;
                    }
                    else {
                        canvas.FillColor = obj.Material.MaterialColor;
                        canvas.StrokeColor = obj.Material.MaterialColor;
                        canvas.StrokeSize = 2;
                        canvas.Alpha = opacity;
                    }
                    
                    
                    // create new rectangle from converted points and draw it
                    var startPoint = ConvertToScreenCoordinates(new Point(polygon.X, polygon.Y), dirtyRect.Width, dirtyRect.Height);
                    var endPoint = ConvertToScreenCoordinates(new Point(polygon.X + polygon.Width, polygon.Y + polygon.Height), dirtyRect.Width, dirtyRect.Height);
                    RectF rect = new RectF((float)startPoint.X, (float)startPoint.Y, (float)(endPoint.X - startPoint.X), (float)(endPoint.Y - startPoint.Y));
                    
                    canvas.FillRectangle(rect);
                    canvas.DrawRectangle(rect);
                    
                    // if we need to show temperature, draw a label in the center of the polygon
                    if (Engine.Engine.ShowTemperature) {
                        // get the center of the polygon
                        double x = 0;
                        double y = 0;

                        x = (rect.X + rect.Width / 2);
                        y = (rect.Y + rect.Height / 2);
                        // get the temperature
                        // draw the label
                        canvas.FillColor = Colors.Black;
                        canvas.FontSize = 10;
                        StringBuilder sb = new StringBuilder();
                        sb.Append((int)temp).Append("°");
                        canvas.DrawString(sb.ToString(), (float)x-10, (float)y-10, 100, 100, HorizontalAlignment.Left, VerticalAlignment.Top);
                    }
                }
            }
            stopwatch.Stop();
            log.Info("Time to draw polygons: " + stopwatch.ElapsedMilliseconds + " ms");
            stopwatch.Restart();
                
            log.Info("Time to set clip geometry: " + stopwatch.ElapsedMilliseconds + " ms");
            
        }

        
        public void Zoom(double delta) {
            if (delta > 1)
                _canvasManager.ZoomIn(delta);
            else
                _canvasManager.ZoomOut(delta);
        }

        public void Move(PanUpdatedEventArgs args) {
            _canvasManager.Move(args);
        }
        
        public void ZoomToObject(EngineObject obj) {
            // get object data
            Vector2 topLeft, bottomRight;
            obj.GetObjectVisibleArea(out topLeft, out bottomRight);
            _canvasManager.ZoomToArea(topLeft, bottomRight);
            _mainPage.Update();
        }

        private Point ConvertToScreenCoordinates(Point point, double actualWidth, double actualHeight) {
            // get ActualWidth and Height
            double width = actualWidth;
            double height = actualHeight;
            // get manager indexes
            int leftX = _canvasManager.CurrentLeftXIndex;
            int rightX = _canvasManager.CurrentRightXIndex;
            int topY = _canvasManager.CurrentTopYIndex;
            int bottomY = _canvasManager.CurrentBottomYIndex;
            // convert point to screen coordinates
            double x = (point.X - leftX) * width / (rightX - leftX);
            double y = height - (point.Y - bottomY) * height / (topY - bottomY);
            return new Point(x, y);
        }
    }
}
