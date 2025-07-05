using ThedyxEngine.Engine;

namespace ThedyxEngine.UI {
    /**
     * \class ObjectsView
     * \brief A list of EngineObject that the user can select from.
     */
    public partial class ObjectsView : ContentView {
        /** The currently selected object. */
        private EngineObject _currentSelectedEngineObject;
        /** The event that is called when the selected object changes. */
        public Action<EngineObject>? OnSelectedObjectChanged;
        /** The event that is called when the user wants to zoom to an object. */
        public Action<EngineObject>? OnZoomToObject;
        /** The event that is called when the user wants to delete an object. */
        public Action? OnDeleteObject;

        /**
         * Constructor for the ObjectsView class.
         */
        public ObjectsView() {
            InitializeComponent();
        }

        /**
         * SelectObject selects an object in the list.
         * \param obj The object to select.
         */
        public void Update(List<EngineObject> objects) {
            EngineCollectionView.ItemsSource = null;
            EngineCollectionView.ItemsSource = objects;
            if (!objects.Contains(_currentSelectedEngineObject)) {
                _currentSelectedEngineObject = null;
            }
        }

        /**
         * Enable enables or disables the list.
         * \param isEnabled Whether the list is enabled.
         */
        public void Enable(bool isEnabled) {
            EngineCollectionView.IsEnabled = isEnabled;
        }



        /**
         * OnSelectionChanged is called when the user selects an object.
         * \param sender The object that sent the event.
         * \param e The event arguments.
         */
        private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e) {
            if (e.CurrentSelection.Count > 0) {
                var selectedObject = e.CurrentSelection[0] as EngineObject;
                if (selectedObject != null) {
                    _currentSelectedEngineObject?.Deselect();
                    _currentSelectedEngineObject = selectedObject;
                    _currentSelectedEngineObject.Select();
                    OnSelectedObjectChanged?.Invoke(_currentSelectedEngineObject);
                }
            }
        }
        
    }
}