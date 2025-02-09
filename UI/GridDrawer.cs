using Microsoft.Maui.Graphics;
using System;
using ThedyxEngine.UI;

namespace TempoEngine.UI
{
    public class GridDrawer {
        public void DrawGrid(ICanvas canvas, CanvasManager manager, RectF viewport) {
            // Set static properties outside the loop
            canvas.StrokeColor = Colors.LightGray;
            canvas.StrokeSize = 1;
            canvas.FontColor = Colors.Black;
            canvas.FontSize = 13;

            int step = 1;
            int deltaX = manager.CurrentRightXIndex - manager.CurrentLeftXIndex;

            if (deltaX > 1000) step = 100;
            else if (deltaX > 500) step = 50;
            else if (deltaX > 200) step = 20;
            else if (deltaX > 100) step = 10;
            else if (deltaX > 50) step = 5;
            else if (deltaX > 20) step = 2;
            else step = 1;

            float width = viewport.Width;
            float height = viewport.Height;
            float stepWidth = width / deltaX * step;  // Precompute

            int leftX = manager.CurrentLeftXIndex - manager.CurrentLeftXIndex % step;
            float x = (leftX - manager.CurrentLeftXIndex) * width / deltaX;
            if (x < 0) x += stepWidth;

            // Vertical grid lines
            while (x < width) {
                canvas.DrawLine(x, 0, x, height);
                canvas.DrawString(leftX.ToString(), x + 2, height - 15, 100, 100, 
                    HorizontalAlignment.Left, VerticalAlignment.Top);
                leftX += step;
                x += stepWidth;
            }

            // Horizontal grid lines
            float yStepHeight = height / (manager.CurrentTopYIndex - manager.CurrentBottomYIndex) * step;
            float yScreen = 0;
            int currentYIndex = manager.CurrentTopYIndex;

            while (yScreen < height) {
                canvas.DrawLine(0, yScreen, width, yScreen);
                canvas.DrawString(currentYIndex.ToString(), 5, yScreen + 2, 100, 100,
                        HorizontalAlignment.Left, VerticalAlignment.Top);
                yScreen += yStepHeight;
                currentYIndex -= step;
            }
        }

    }
}
