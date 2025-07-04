using System.Diagnostics;
using System.Numerics;
using log4net;
using TempoEngine.UI;
using ThedyxEngine.Engine;
using ThedyxEngine.Engine.Managers;

namespace ThedyxEngine.UI {
    /**
     * Canvas is a canvas that displays the simulation.
     */
    public class Canvas : IDrawable {
        /** The canvas manager. It makes some math */
        private readonly CanvasManager _canvasManager;
        /** The grid drawer. It draws the grid */
        private readonly GridDrawer _gridDrawer;
        /** The logger */
        private static readonly ILog log = LogManager.GetLogger(typeof(Canvas));
        /** The main page of the whole program*/
        private MainPage _mainPage;
        /** Last dirty rectangle */
        private RectF _lastDirtyRect;
        /**
         * Constructor for the Canvas class.
         * \param mainPage The main page of the program.
         */
        public Canvas(MainPage mainPage) : base() {
            _canvasManager = new CanvasManager();
            _gridDrawer = new GridDrawer();
            _mainPage = mainPage;
        }
        

        
        /**
         * Draw draws the simulation on the canvas.
         * Draws every polygon of every object that is visible on the canvas.
         * Draws the grid if it is enabled.
         * \param canvas The canvas to draw on.
         * \param dirtyRect The dirty rectangle.
         */
        public void Draw(ICanvas canvas, RectF dirtyRect) {
            _lastDirtyRect = dirtyRect;
            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 2;
            canvas.FillColor = Colors.LightBlue;
            canvas.FontSize = 10;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            _canvasManager.AdjustForAspectRatio(dirtyRect.Width, dirtyRect.Height);

            // log time
            stopwatch.Stop();
            stopwatch.Restart();

            List<EngineObject> objects = Engine.Engine.EngineObjectsManager.GetVisibleObjects(_canvasManager);
            stopwatch.Stop();
            stopwatch.Restart();
            

            // get polygons
            foreach (var obj in objects) {
                obj.GetPolygons(_canvasManager, out var polygons, out var temperatures, out var opacities);
                // convert polygon points to screen coordinates
                for (int j = 0; j < polygons.Count; j++) {
                    RectF startRect = polygons[j];
                    var startPoint = ConvertToScreenCoordinates(new Point(startRect.X, startRect.Y), dirtyRect.Width, dirtyRect.Height);
                    var endPoint = ConvertToScreenCoordinates(new Point(startRect.X + startRect.Width, startRect.Y + startRect.Height), dirtyRect.Width, dirtyRect.Height);
                    // сheck if at least one point is inside the dirty rect
                    if((startPoint.X < 0 && endPoint.X < 0) || (startPoint.X > dirtyRect.Width && endPoint.X > dirtyRect.Width) || (startPoint.Y < 0 && endPoint.Y < 0) || (startPoint.Y > dirtyRect.Height && endPoint.Y > dirtyRect.Height)) {
                        continue;
                    }
                    double temp = temperatures[j];
                    float opacity = opacities[j];
                    Color fillColor = Colors.Black;
                    if (!Engine.Engine.ShowColor) {
                        fillColor = ColorManager.GetColorFromTemperature(temp);
                        canvas.StrokeSize = 2;
                        canvas.Alpha = opacity;
                    }
                    else {
                        fillColor = obj.Material.MaterialColor;
                        canvas.StrokeSize = 2;
                        canvas.Alpha = opacity;
                    }
                    canvas.FillColor = fillColor;
                    canvas.StrokeColor = fillColor;
                    
                    // create new rectangle from converted points and draw it
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
                        // draw the label
                        //  check if color is too dart, set color of text to white
                        double brightness = 0.299 * fillColor.Red + 0.587 * fillColor.Green + 0.114 * fillColor.Blue;
                        canvas.FontColor = brightness < 0.5 ? Colors.White : Colors.Black;
                        canvas.Alpha = 1;
                        string text = (int)temp + "°";
                        canvas.DrawString(text, (float)x-10, (float)y-10, 100, 100, HorizontalAlignment.Left, VerticalAlignment.Top);
                    }
                }
            }
            
            stopwatch.Stop();
            stopwatch.Restart();
            
            if (Engine.Engine.ShowGrid) {
                canvas.Alpha = 1;
                _gridDrawer.DrawGrid(canvas, _canvasManager, dirtyRect);
                stopwatch.Stop();
                stopwatch.Restart();
            }
            
        }

        
        /**
         * Zoom zooms in or out.
         * \param delta The delta.
         */
        public void Zoom(double delta) {
            if (delta > 1)
                _canvasManager.ZoomIn(delta);
            else
                _canvasManager.ZoomOut(delta);
        }
        
        /**
         * Move moves the canvas.
         * \param args The pan updated event arguments.
         */
        public void Move(PanUpdatedEventArgs args) {
            _canvasManager.Move(args);
        }
        
        /**
         * ZoomToObject zooms to an object.
         * \param obj The object to zoom to.
         */
        public void ZoomToObject(EngineObject obj) {
            // get object data
            Vector2 topLeft, bottomRight;
            obj.GetObjectVisibleArea(out topLeft, out bottomRight);
            _canvasManager.ZoomToArea(topLeft, bottomRight);
            _mainPage.Update();
        }
        
        /**
         * ConvertToScreenCoordinates converts a point in simulation to screen coordinates.
         * \param point The point to convert.
         * \param actualWidth The actual width of the canvas.
         * \param actualHeight The actual height of the canvas.
         * \return The point in screen coordinates.
         */
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

        /**
         * ConvertToCanvasCoordinates converts a point in screen coordinates to simulation coordinates.
         * \param point The point to convert.
         * \return The point in simulation coordinates.
         */
        public Point ConvertToCanvasCoordinates(PointF point) {
            // get ActualWidth and Height
            double width = _lastDirtyRect.Width;
            double height = _lastDirtyRect.Height;
            // get manager indexes
            int leftX = _canvasManager.CurrentLeftXIndex;
            int rightX = _canvasManager.CurrentRightXIndex;
            int topY = _canvasManager.CurrentTopYIndex;
            int bottomY = _canvasManager.CurrentBottomYIndex;
            // convert point to screen coordinates
            double x = leftX + point.X * (rightX - leftX) / width;
            double y = bottomY + (height - point.Y) * (topY - bottomY) / height;
            return new Point(x, y);
        }
    }
}
