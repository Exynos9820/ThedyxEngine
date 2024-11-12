using Microsoft.Maui.Graphics;
using System;
using ThedyxEngine.UI;

namespace TempoEngine.UI
{
    public class GridDrawer
    {
        public void DrawGrid(ICanvas canvas, CanvasManager manager, RectF viewport)
        {
            // Set colors and line thickness
            canvas.StrokeColor = Colors.LightGray;
            canvas.StrokeSize = 1;
            

            int step = 1;
            int deltaX = manager.CurrentRightXIndex - manager.CurrentLeftXIndex;

            // Calculate step sizes based on deltaX
            if (deltaX > 1000) step = 100;
            else if (deltaX > 500) step = 50;
            else if (deltaX > 200) step = 20;
            else if (deltaX > 100) step = 10;
            else if (deltaX > 50) step = 5;
            else if (deltaX > 20) step = 2;
            else if (deltaX > 10) step = 1;

            float width = viewport.Width;
            float height = viewport.Height;

            // Calculate initial left X position
            int leftX = manager.CurrentLeftXIndex - manager.CurrentLeftXIndex % step;
            float x = (leftX - manager.CurrentLeftXIndex) * width / deltaX;

            if (x < 0) x += width / deltaX * step;

            // Draw vertical grid lines and labels
            while (x < width)
            {
                canvas.StrokeColor = Colors.LightGray;
                canvas.DrawLine(x, 0, x, height);
                canvas.FontColor = Colors.Black;
                canvas.FontSize = 13;
                canvas.DrawString(leftX.ToString(), x + 2, height - 15,  100, 100, HorizontalAlignment.Left, VerticalAlignment.Top);
                leftX += step;
                x += width / deltaX * step;
            }

            float yScreen = 0;
            int currentYIndex = manager.CurrentTopYIndex;
            canvas.FontColor = Colors.Black;
            
            // Draw horizontal grid lines and labels
            while (yScreen < height)
            {
                canvas.StrokeColor = Colors.LightGray;
                canvas.DrawLine(0, yScreen, width, yScreen);
                canvas.FontColor = Colors.Black;
                canvas.FontSize = 13;
                canvas.DrawString(currentYIndex.ToString(), 5, yScreen + 2, 100, 100, HorizontalAlignment.Left, VerticalAlignment.Top);
                yScreen += height / (manager.CurrentTopYIndex - manager.CurrentBottomYIndex) * step;
                currentYIndex -= step;
            }
        }
    }
}
