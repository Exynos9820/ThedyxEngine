using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Collections.Generic;
using ThedyxEngine.Engine;

namespace ThedyxEngine.UI
{
    /**
     * \class EngineObjectsList
     * \brief EngineObjectsList class
     * List of all objects of the simulation
     */
    public class EngineObjectsList : CollectionView
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
            BackgroundColor = Colors.White;

            // Define item template with black text color
            ItemTemplate = new DataTemplate(() =>
            {
                var label = new Label
                {
                    TextColor = Colors.Black,
                    FontSize = 16,
                    Padding = new Thickness(5, 3, 0, 0)
                };
                label.SetBinding(Label.TextProperty, "Name"); // Bind to Name property

                var layout = new StackLayout
                {
                    Padding = new Thickness(10, 0, 0, 0), // Small left padding
                    Children = { label }
                };

                // Add tap gesture recognizer for zooming
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) => ZoomToObject();

                layout.GestureRecognizers.Add(tapGesture);
                return layout;
            });

            SelectionMode = SelectionMode.Single;
            SelectionChanged += OnSelectionChanged;
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
            if (_currentSelectedEngineObject != null)
            {
                OnZoomToObject?.Invoke(_currentSelectedEngineObject);
            }
        }

        /**
         * Handle item selection
         */
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count > 0)
            {
                var selectedObject = e.CurrentSelection[0] as EngineObject;
                if (selectedObject != null)
                {
                    _currentSelectedEngineObject?.Deselect();
                    _currentSelectedEngineObject = selectedObject;
                    _currentSelectedEngineObject.Select();
                    OnSelectedObjectChanged?.Invoke(_currentSelectedEngineObject);
                }
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
}
