using ThedyxEngine.Engine;

namespace ThedyxEngine.UI;

/**
 * \class EngineObjectsList
 * \brief EngineObjectsList class
 * List of all objects of the simulation
 */
public class EngineObjectsList : ListView
{
    private EngineObject? _currentSelectedEngineObject;  // Current selected object
    public Action<EngineObject>? OnSelectedObjectChanged; // Event for selected object changed
    public Action<EngineObject>? OnZoomToObject;          // Event for zoom to object
    public Action? OnDeleteObject;                        // Event for delete object
    private bool _isEnabled = true;                       // Is enabled

    /**
     * Constructor
     */
    public EngineObjectsList()
    {
        // Add gesture recognizer for double-tap
        var tapGesture = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
        tapGesture.Tapped += (s, e) => ZoomToObject();
        this.GestureRecognizers.Add(tapGesture);

        // Selection change handling
        this.ItemSelected += OnSelectionChanged;
    }

    /**
     * Update the list of objects
     * \param objects List of objects
     */
    public void Update(List<EngineObject> objects)
    {
        this.ItemsSource = objects;
    }

    public void Enable(bool isEnabled)
    {
        _isEnabled = isEnabled;
    }

    /**
     * Zoom to object
     */
    private void ZoomToObject()
    {
        var item = this.SelectedItem as EngineObject;
        if (item != null)
        {
            _currentSelectedEngineObject?.Deselect();
            _currentSelectedEngineObject = item;
            _currentSelectedEngineObject.Select();
            OnZoomToObject?.Invoke(_currentSelectedEngineObject);
        }
    }

    /**
     * Handle delete or navigation keys
     */
    public void HandleKeyDown(string key)
    {
        if (key == "Delete" && _isEnabled)
        {
            var item = SelectedItem as EngineObject;
            if (item != null)
            {
                Engine.Engine.EngineObjectsManager.RemoveObject(item);
                OnDeleteObject?.Invoke();
            }
        }
        else if (key == "Down")
        {
            if (SelectedIndex < Items.Count - 1)
            {
                SelectedIndex++;
                _currentSelectedEngineObject = Engine.Engine.EngineObjectsManager.GetObjectByIndex(SelectedIndex);
            }
        }
        else if (key == "Up")
        {
            if (SelectedIndex > 0)
            {
                SelectedIndex--;
                _currentSelectedEngineObject = Engine.Engine.EngineObjectsManager.GetObjectByIndex(SelectedIndex);
            }
        }
    }

    /**
     * OnSelectionChanged method
     */
    private void OnSelectionChanged(object sender, SelectedItemChangedEventArgs e)
    {
        var selectedObject = e.SelectedItem as EngineObject;
        if (selectedObject != null && (_currentSelectedEngineObject == null || selectedObject.Name != _currentSelectedEngineObject.Name))
        {
            _currentSelectedEngineObject?.Deselect();
            _currentSelectedEngineObject = selectedObject;
            _currentSelectedEngineObject.Select();
            OnSelectedObjectChanged?.Invoke(_currentSelectedEngineObject);
        }
    }

    /**
     * Clear the list
     */
    public void Clear()
    {
        this.ItemsSource = null;
    }
}