using Microsoft.Maui.Graphics;
using System.Diagnostics;
using System.Numerics;
using System.Collections.Generic;
using Microsoft.Maui.Controls.Shapes;
using ThedyxEngine.Engine;
using ThedyxEngine.UI;

public class EngineCanvasDrawable : IDrawable
{
    private CanvasManager _canvasManager;
    private GridDrawer _gridDrawer;
    private bool drawGrid = true;
    private float zoomLevel = 1.0f;
    private PointF panOffset = new PointF(0, 0);
    private Stopwatch stopwatch = new Stopwatch();

    public EngineCanvasDrawable()
    {
        _canvasManager = new CanvasManager();
        _gridDrawer = new GridDrawer();
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.SaveState();
        
        // Apply zoom and pan transformations
        canvas.Scale(zoomLevel, zoomLevel);
        canvas.Translate(panOffset.X, panOffset.Y);

        // Draw grid if enabled
        if (drawGrid)
        {
            stopwatch.Restart();
            _gridDrawer.DrawGrid(canvas, _canvasManager);
            stopwatch.Stop();
            Debug.WriteLine($"Time to draw grid: {stopwatch.ElapsedMilliseconds} ms");
        }

        // Adjust for aspect ratio
        _canvasManager.AdjustForAspectRatio(dirtyRect.Width, dirtyRect.Height);

        // Retrieve and draw polygons
        var objects = Engine.EngineObjectsManager.GetVisibleObjects(_canvasManager);
        stopwatch.Restart();
        foreach (var obj in objects)
        {
            List<Polygon> polygons = obj.GetPolygons(_canvasManager);
            foreach (var polygon in polygons)
            {
                // Convert polygon points to screen coordinates and draw
                for (int i = 0; i < polygon.Points.Count; i++)
                {
                    polygon.Points[i] = ConvertToScreenCoordinates(polygon.Points[i]);
                }
                DrawPolygon(canvas, polygon);
            }
        }
        stopwatch.Stop();
        Debug.WriteLine($"Time to draw polygons: {stopwatch.ElapsedMilliseconds} ms");

        canvas.RestoreState();
    }

    private void DrawPolygon(ICanvas canvas, Polygon polygon)
    {
        // Set stroke and fill for polygons
        canvas.StrokeColor = Colors.Black;
        canvas.StrokeSize = 2;
        canvas.FillColor = Colors.LightBlue;
        canvas.FillPolygon(polygon.Points.ToArray());
        canvas.DrawPolygon(polygon.Points.ToArray());
    }

    public PointF ConvertToScreenCoordinates(PointF point)
    {
        double width = _canvasManager.ViewportWidth;
        double height = _canvasManager.ViewportHeight;
        int leftX = _canvasManager.CurrentLeftXIndex;
        int rightX = _canvasManager.CurrentRightXIndex;
        int topY = _canvasManager.CurrentTopYIndex;
        int bottomY = _canvasManager.CurrentBottomYIndex;

        double x = (point.X - leftX) * width / (rightX - leftX);
        double y = height - (point.Y - bottomY) * height / (topY - bottomY);
        return new PointF((float)x, (float)y);
    }

    public void ZoomIn(float factor) => zoomLevel *= factor;
    public void ZoomOut(float factor) => zoomLevel /= factor;
    public void Pan(float dx, float dy) => panOffset = new PointF(panOffset.X + dx, panOffset.Y + dy);
}
