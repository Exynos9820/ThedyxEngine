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
        MatCollectionView.ItemsSource  = MaterialManager.Materials;
    }
    
    public void SelectMaterial(Material material) {
        _currentSelectedMaterial = material;
        MatCollectionView.SelectedItem = material;
    }

    public void Update() {
        MatCollectionView.ItemsSource = null;
        if (_currentSelectedMaterial != null && !MaterialManager.Materials.Contains(_currentSelectedMaterial)) {
            _currentSelectedMaterial = null;
        }
    }
    
    public void Update(List<Material> materials) {
        MatCollectionView.ItemsSource = null;
        MatCollectionView.ItemsSource = materials;
        if (!materials.Contains(_currentSelectedMaterial)) {
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