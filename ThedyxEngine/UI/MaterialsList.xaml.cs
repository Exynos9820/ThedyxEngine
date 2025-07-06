using ThedyxEngine.Engine;
using ThedyxEngine.Engine.Managers;

namespace ThedyxEngine.UI;

/**
 * \class MaterialsList
 * \brief A list of @ref ThedyxEngine.Engine.Material "Materials" that the user can select from.
 */
public partial class MaterialsList : ContentView {
    /** The currently selected material. */
    private Material? _currentSelectedMaterial;
    /** The event that is called when the selected material changes. */
    public Action<Material>? OnSelectedMaterialChanged;

    /**
     * Constructor for the MaterialsList class.
     */
    public MaterialsList() {
        InitializeComponent();
        MatCollectionView.ItemsSource  = MaterialManager.MaterialsView;
    }
    
    /**
     * SelectMaterial selects a material in the list.
     * \param material The material to select.
     */
    public void SelectMaterial(Material material) {
        _currentSelectedMaterial = material;
        MatCollectionView.SelectedItem = material;
    }

    /**
     * Update updates the list of materials.
     * If the currently selected material is not in the list, it is set to null.
     */
    public void Update() {
        MatCollectionView.ItemsSource = MaterialManager.MaterialsView;
    }

    /**
     * OnSelectionChanged is called when the user selects a material.
     * \param sender The object that sent the event.
     * \param e The event arguments.
     */
    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e) {
        if (e.CurrentSelection.Count <= 0) return;
        if (e.CurrentSelection[0] is not Material selectedObject) return;
        _currentSelectedMaterial = selectedObject;
        OnSelectedMaterialChanged?.Invoke(_currentSelectedMaterial);
    }
}