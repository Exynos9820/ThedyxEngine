using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThedyxEngine.Engine;
using ThedyxEngine.Engine.Managers;

namespace ThedyxEngine.UI;

public partial class MaterialsList : ContentView {
    public Material? _currentSelectedMaterial;
    public Action<Material>? OnSelectedMaterialChanged;

    public MaterialsList() {
        InitializeComponent();
        EngineCollectionView.ItemsSource  = MaterialManager.Materials;
    }
    
    public void SelectMaterial(Material material) {
        _currentSelectedMaterial = material;
        EngineCollectionView.SelectedItem = material;
    }

    public void Update() {
        if (_currentSelectedMaterial != null && !MaterialManager.Materials.Contains(_currentSelectedMaterial)) {
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