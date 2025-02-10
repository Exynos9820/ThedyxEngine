using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using ThedyxEngine.Engine;
using ThedyxEngine.Engine.Managers;

namespace ThedyxEngine.UI;

public partial class MaterialsPopup : Popup {
    private Material? _currentSelectedMaterial;
    public MaterialsPopup() {
        InitializeComponent();
        
        // we need to update the list of materials
        Materials.Update(MaterialManager.GetMaterials());
        Materials.OnSelectedMaterialChanged += OnSelectedMaterialChanged;
    }
    
    private void OnSelectedMaterialChanged(Material material) {
        _currentSelectedMaterial = material;
        // we need to update the properties of the selected material
        Name.Text = material.Name;
    }
    
    
    private void OnNameCompleted(object sender, EventArgs e) {
        // if name is the same, we don't need to update
        if (_currentSelectedMaterial == null || _currentSelectedMaterial.Name == Name.Text) return;
        
    }

}