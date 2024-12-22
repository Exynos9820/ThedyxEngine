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
                obj.GetPolygons(_canvasManager, out var polygons, out var temperatures);
                // convert polygon points to screen coordinates
                for (int j = 0; j < polygons.Count; j++) {
                    Polygon polygon = polygons[j];
                    double temp = temperatures[j];
                    if (polygon.Fill is SolidColorBrush solidColorBrush && !Engine.Engine.ShowColor) {
                        canvas.FillColor = solidColorBrush.Color;
                        canvas.StrokeColor = solidColorBrush.Color;
                        canvas.StrokeSize = 2;
                    }
                    else {
                        canvas.FillColor = obj.Material.MaterialColor;
                        canvas.StrokeColor = obj.Material.MaterialColor;
                        canvas.StrokeSize = 2;
                    }
                    
                    for (int i = 0; i < polygon.Points.Count; i++) {
                        polygon.Points[i] = ConvertToScreenCoordinates(polygon.Points[i], dirtyRect.Width, dirtyRect.Height);
                    }
                    // add polygon to canvas
                    var path = new PathF();
                    path.MoveTo((float)polygon.Points[0].X, (float)polygon.Points[0].Y);
                    for (int i = 1; i < polygon.Points.Count; i++)
                    {
                        path.LineTo((float)polygon.Points[i].X, (float)polygon.Points[i].Y);
                    }
                    path.Close();

                    canvas.FillPath(path);
                    canvas.FillPath(path);
                    
                    // if we need to show temperature, draw a label in the center of the polygon
                    if (Engine.Engine.ShowTemperature) {
                        // get the center of the polygon
                        double x = 0;
                        double y = 0;
                        foreach (var point in polygon.Points) {
                            x += point.X;
                            y += point.Y;
                        }
                        x /= polygon.Points.Count;
                        y /= polygon.Points.Count;
                        // get the temperature
                        // draw the label
                        canvas.FillColor = Colors.Black;
                        canvas.FontSize = 8;
                        StringBuilder sb = new StringBuilder();
                        sb.Append((int)temp).Append("°K");
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
