using System.Numerics;
using ThedyxEngine.Util;

namespace ThedyxEngine.UI;

/**
 * \class CanvasManager
 * \brief Manages the Canvas
 */
public class CanvasManager{
    /** Gets the current left X index. */
    public int CurrentLeftXIndex { get; private set; }
    /** Gets the current right X index. */
    public int CurrentRightXIndex { get; private set; }
    /** Gets the current top Y index. */
    public int CurrentTopYIndex { get; private set; }
    /** Gets the current bottom Y index. */
    public int CurrentBottomYIndex { get; private set; }

    /**
     * \brief Initializes a new instance of the CanvasManager class.
     * Always resets the zoom to the default values.
     */
    public CanvasManager() {
        ResetZoom();
    }

    /**
     * \brief Resets the zoom to the default values.
     * Always a square 200 x 200.
     */
    public void ResetZoom() {
        CurrentLeftXIndex = -100;
        CurrentRightXIndex = 100;
        CurrentTopYIndex = 100;
        CurrentBottomYIndex = -100;
    }

    /**
     * \brief Gets the step for moving the view.
     * \return The step for moving the view.
     */
    private int getStep() {
        int deltaX = CurrentRightXIndex - CurrentLeftXIndex;
        int step = 1;
        if (deltaX > 1000) step = 100;
        else if (deltaX > 500) step = 50;
        else if (deltaX > 200) step = 20;
        else if (deltaX > 100) step = 10;
        else if (deltaX > 50) step = 5;
        else if (deltaX > 20) step = 2;
        else if (deltaX > 10) step = 1;
        return step;
    }

    /**
     * \brief Moves the view.
     * \param key The key pressed.
     */
    public void Move(PanUpdatedEventArgs args) {
        int width = CurrentRightXIndex - CurrentLeftXIndex;
        int height = CurrentTopYIndex - CurrentBottomYIndex;
        int step = getStep();
        CurrentLeftXIndex += (int)args.TotalX / 100;
        CurrentRightXIndex += (int)args.TotalX / 100;
        CurrentTopYIndex += (int)args.TotalY / 100;
        CurrentBottomYIndex += (int)args.TotalY / 100;
        // check if we are out of bounds
        if (CurrentLeftXIndex < CanvasHelper.MinLeftXIndex) {
            CurrentLeftXIndex = CanvasHelper.MinLeftXIndex;
            CurrentRightXIndex = CurrentLeftXIndex + width;
        }
        if (CurrentRightXIndex > CanvasHelper.MaxRightXIndex) {
            CurrentRightXIndex = CanvasHelper.MaxRightXIndex;
            CurrentLeftXIndex = CurrentRightXIndex - width;
        }
        if (CurrentTopYIndex > CanvasHelper.MaxYTopIndex) {
            CurrentTopYIndex = CanvasHelper.MaxYTopIndex;
            CurrentBottomYIndex = CurrentTopYIndex - height;
        }
        if (CurrentBottomYIndex < CanvasHelper.MinYBottomIndex) {
            CurrentBottomYIndex = CanvasHelper.MinYBottomIndex;
            CurrentTopYIndex = CurrentBottomYIndex + height;
        }
    }

    /**
     * \brief Zooms to a given area.
     * \param topLeft The top left corner of the area.
     * \param bottomRight The bottom right corner of the area.
     */
    public void ZoomToArea(Vector2 topLeft, Vector2 bottomRight) {
        CurrentLeftXIndex = (int)topLeft.X;
        CurrentRightXIndex = (int)bottomRight.X;
        CurrentTopYIndex = (int)bottomRight.Y;
        CurrentBottomYIndex = (int)topLeft.Y;
        // check if we are out of bounds
        if (CurrentLeftXIndex < CanvasHelper.MinLeftXIndex) {
            CurrentLeftXIndex = CanvasHelper.MinLeftXIndex;
        }
        if (CurrentRightXIndex > CanvasHelper.MaxRightXIndex) {
            CurrentRightXIndex = CanvasHelper.MaxRightXIndex;
        }
        if (CurrentTopYIndex > CanvasHelper.MaxYTopIndex) {
            CurrentTopYIndex = CanvasHelper.MaxYTopIndex;
        }
        if (CurrentBottomYIndex < CanvasHelper.MinYBottomIndex) {
            CurrentBottomYIndex = CanvasHelper.MinYBottomIndex;
        }
        // check for distance between indexes, if it is smaller than 10, we need to adjust indexes
        if (CurrentRightXIndex - CurrentLeftXIndex < 10) {
            int delta = 10 - (CurrentRightXIndex - CurrentLeftXIndex);
            CurrentRightXIndex += delta / 2;
            CurrentLeftXIndex -= delta / 2;
        }
        if (CurrentTopYIndex - CurrentBottomYIndex < 10) {
            int delta = 10 - (CurrentTopYIndex - CurrentBottomYIndex);
            CurrentTopYIndex += delta / 2;
            CurrentBottomYIndex -= delta / 2;
        }
    }

    /**
     * \brief Zooms in.
     * \param delta The delta.
     */
    public void ZoomIn(double delta) {
        int xSize = CurrentRightXIndex - CurrentLeftXIndex;
        int ySize = CurrentTopYIndex - CurrentBottomYIndex;

        if (xSize == CanvasHelper.MinRightXIndex - CanvasHelper.MaxLeftXIndex || ySize == CanvasHelper.MinYTopIndex - CanvasHelper.MaxYBottomIndex) return;
        int xZoomDelta = (int) (xSize * delta / 100 - xSize) / 5;
        int yZoomDelta = (int) (ySize * delta / 100 - ySize) / 5;

        if (xZoomDelta >= 0 || yZoomDelta >= 0) return;

        CurrentRightXIndex = Math.Max(CurrentRightXIndex + xZoomDelta, CanvasHelper.MinRightXIndex);
        CurrentLeftXIndex = Math.Min(CurrentLeftXIndex - xZoomDelta, CanvasHelper.MaxLeftXIndex);
        CurrentTopYIndex = Math.Max(CurrentTopYIndex + yZoomDelta, CanvasHelper.MinYTopIndex);
        CurrentBottomYIndex = Math.Min(CurrentBottomYIndex - yZoomDelta, CanvasHelper.MaxYBottomIndex);
    }

    /**
     * \brief Zooms out.
     * \param delta The delta.
     */
    public void ZoomOut(double delta) {
        // calculate new indexes depending on delta, but not more than max indexes
        int xSize = CurrentRightXIndex - CurrentLeftXIndex;
        int ySize = CurrentTopYIndex - CurrentBottomYIndex;

        if (xSize == CanvasHelper.MaxRightXIndex - CanvasHelper.MinLeftXIndex || ySize == CanvasHelper.MaxYTopIndex - CanvasHelper.MinYBottomIndex)
            return;

        int xZoomDelta = (int) (xSize * delta / 100 + xSize) / 5;
        int yZoomDelta = (int) (ySize * delta / 100 + ySize) / 5;

        if (xZoomDelta <= 0 || yZoomDelta <= 0){
            // then make it 10 and adjust indexes
            xZoomDelta = 3;
            // adjust y zoom delta to keep aspect ratio
            yZoomDelta = (int)(xZoomDelta * (CurrentTopYIndex - CurrentBottomYIndex) / (CurrentRightXIndex - CurrentLeftXIndex));
        }
        CurrentRightXIndex = Math.Min(CurrentRightXIndex + xZoomDelta, CanvasHelper.MaxRightXIndex);
        CurrentLeftXIndex = Math.Max(CurrentLeftXIndex - xZoomDelta, CanvasHelper.MinLeftXIndex);
        CurrentTopYIndex = Math.Min(CurrentTopYIndex + yZoomDelta, CanvasHelper.MaxYTopIndex);
        CurrentBottomYIndex = Math.Max(CurrentBottomYIndex - yZoomDelta, CanvasHelper.MinYBottomIndex);
    }

    /**
     * \brief Adjusts the indexes for the aspect ratio.
     * \param width The width.
     * \param height The height.
     */
    public void AdjustForAspectRatio(double width, double height) {
        // Adjust indexes based on aspect ratio
        // if canvas is not square, e need to adjust indexes for Y axis
        double ratio = width / height;

        // get needed distance for Y axis
        int yDistance = (int)((CurrentRightXIndex - CurrentLeftXIndex) / ratio);
        // get current distance for Y axis
        int currentYDistance = CurrentTopYIndex - CurrentBottomYIndex;
        // calculate new indexes for Y axis
        int yDelta = (yDistance - currentYDistance) / 2;
        CurrentTopYIndex += yDelta;
        CurrentBottomYIndex -= yDelta;
    }


    /**
     * Checks if a given point is visible within the current canvas manager's view.
     * \param point The point to check for visibility.
     * \param canvasManager The canvas manager providing the current view context.
     * \return True if the point is visible, otherwise false.
     */

    public static bool isPointVisible(Point point, CanvasManager canvasManager){
        return point.X >= canvasManager.CurrentLeftXIndex && point.X <= canvasManager.CurrentRightXIndex &&
               point.Y >= canvasManager.CurrentBottomYIndex && point.Y <= canvasManager.CurrentTopYIndex;
    }
}