using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using ThedyxEngine.Engine;

namespace ThedyxEngine.UI
{
    public partial class EngineObjectsList : ContentView
    {
        private EngineObject? _currentSelectedEngineObject;
        public Action<EngineObject>? OnSelectedObjectChanged;
        public Action<EngineObject>? OnZoomToObject;
        public Action? OnDeleteObject;

        public EngineObjectsList()
        {
            InitializeComponent();
        }

        // Method to update the list of objects
        public void Update(List<EngineObject> objects) {
            EngineCollectionView.ItemsSource = null;
            EngineCollectionView.ItemsSource = objects;
            if (!objects.Contains(_currentSelectedEngineObject)) {
                _currentSelectedEngineObject = null;
            }
        }

        // Method to enable or disable the control
        public void Enable(bool isEnabled) {
            EngineCollectionView.IsEnabled = isEnabled;
        }



        // Method to handle item selection
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