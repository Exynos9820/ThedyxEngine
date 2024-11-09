using Microsoft.Maui.Graphics;
using System;

namespace ThedyxEngine.UI
{
    /**
     * \class GridDrawer
     * \brief Represents the grid drawer.
     * 
     * The GridDrawer class provides methods for drawing the grid.
     */
    public class GridDrawer
    {
        /**
         * \brief Draws the grid on the specified canvas.
         * \param canvas The MAUI canvas to draw on.
         * \param manager The canvas manager providing the view context.
         */
        public void DrawGrid(ICanvas canvas, CanvasManager manager)
        {
            canvas.StrokeColor = Colors.LightGray;
            canvas.StrokeSize = 1;

            // Define the step based on the current zoom level or view size
            int step = 1;
            int deltaX = manager.CurrentRightXIndex - manager.CurrentLeftXIndex;
            if (deltaX > 1000) step = 100;
            else if (deltaX > 500) step = 50;
            else if (deltaX > 200) step = 20;
            else if (deltaX > 100) step = 10;
            else if (deltaX > 50) step = 5;
            else if (deltaX > 20) step = 2;
            else if (deltaX > 10) step = 1;

            // Calculate width and height of the visible area
            float width = (float)manager.ViewportWidth;
            float height = (float)manager.ViewportHeight;

            // Draw vertical lines
            float x = 0;
            for (int i = manager.CurrentLeftXIndex; i <= manager.CurrentRightXIndex; i += step)
            {
                x = (i - manager.CurrentLeftXIndex) * width / deltaX;
                canvas.DrawLine(x, 0, x, height);
            }

            // Draw horizontal lines
            float y = 0;
            int deltaY = manager.CurrentTopYIndex - manager.CurrentBottomYIndex;
            for (int i = manager.CurrentBottomYIndex; i <= manager.CurrentTopYIndex; i += step)
            {
                y = height - (i - manager.CurrentBottomYIndex) * height / deltaY;
                canvas.DrawLine(0, y, width, y);
            }

            // Draw labels along the axes
            canvas.FontColor = Colors.Black;
            canvas.FontSize = 10;

            // Draw X-axis labels
            x = 0;
            for (int i = manager.CurrentLeftXIndex; i <= manager.CurrentRightXIndex; i += step)
            {
                x = (i - manager.CurrentLeftXIndex) * width / deltaX;
                canvas.DrawString(i.ToString(), x, height - 10, HorizontalAlignment.Center);
            }

            // Draw Y-axis labels
            y = 0;
            for (int i = manager.CurrentBottomYIndex; i <= manager.CurrentTopYIndex; i += step)
            {
                y = height - (i - manager.CurrentBottomYIndex) * height / deltaY;
                canvas.DrawString(i.ToString(), 0, y, HorizontalAlignment.Left);
            }
        }
    }
}
