using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThedyxEngine.Engine;

namespace ThedyxEngine.UI;

public partial class MaterialsList : ContentView {
    public Material? _currentSelectedMaterial;
    public Action<Material>? OnSelectedMaterialChanged;

    public MaterialsList() {
        InitializeComponent();
    }

    public void Update(List<Material> materials) {
        EngineCollectionView.ItemsSource = null;
        EngineCollectionView.ItemsSource = materials;
        if (_currentSelectedMaterial != null && !materials.Contains(_currentSelectedMaterial)) {
            _currentSelectedMaterial = null;
        }
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e) {
        if (e.CurrentSelection.Count <= 0) return;
        if (e.CurrentSelection[0] is not Material selectedObject) return;
        _currentSelectedMaterial = selectedObject;
        OnSelectedMaterialChanged?.Invoke(_currentSelectedMaterial);
    }
}