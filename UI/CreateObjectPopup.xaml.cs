using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using ThedyxEngine.Engine;
using ThedyxEngine.Engine.Managers;

namespace ThedyxEngine.UI;

/**
 * CreateObjectPopup is a popup that allows the user to create an object.
 */
public partial class CreateObjectPopup : Popup {
    /** The object type. */
    private ObjectType _objectType;
    /** The object being created. */
    private EngineObject _object;
    /** The event that is called when the object is created. */
    public Action OnObjectCreated;
    
    /**
     * Constructor for the CreateObjectPopup class.
     * \param objectType The object type.
     */
    public CreateObjectPopup(ObjectType objectType) {
        InitializeComponent();
        _objectType = objectType;
        MakeEntriesWhite();
        switch (objectType) {
            case ObjectType.Rectangle:
                TitleLabel.Text = "Create Rectangle";
                _object = new EngineRectangle("", 10, 10);
                break;
            case ObjectType.GrainSquare:
                TitleLabel.Text = "Create Grain Square";
                _object = new GrainSquare("", new Point(0,0));
                break;
            case ObjectType.StateRectangle:
                TitleLabel.Text = "Create State Change Rectangle";
                _object = new EngineStateRectangle("", 10, 10);
                break;
            default:
                Debug.Assert(false, "Invalid object type");
                throw new Exception("Invalid object type");
                break;
        }

        _object.Material = MaterialManager.GetBaseMaterial();
        Material.ItemsSource = MaterialManager.GetMaterials();
        Material.SelectedItem = _object.Material;
        _object.Position = new Microsoft.Maui.Graphics.Point(0, 0);
        XPosition.Text = "0";
        YPosition.Text = "0";
        _object.Size = new Microsoft.Maui.Graphics.Point(1, 1);
        Width.Text = "1";
        Height.Text = "1";
        _object.SimulationTemperature = 200;
        Temperature.Text = "200";
    }
    
    /**
     * Close closes the popup.
     */
    private async void ShowErrorMessageBox(string text) {
        await Application.Current.MainPage.DisplayAlert("Error", text, "OK");
    }
    
    /**
     * UpdateAll updates all the fields.
     * Before closing the popup, this function is called to update all the fields of the created object.
     */
    private void UpdateAll() {
        OnNameCompleted(null, null);
        OnTemperatureCompleted(null, null);
        OnXPositionCompleted(null, null);
        OnYPositionCompleted(null, null);
        OnHeightCompleted(null, null);
        OnWidthCompleted(null, null);
        OnMaterialChanged(null, null);
    }

    /**
     * OnNameCompleted is called when the user has finished editing the name.
     * \param sender The object that sent the event.
     * \param e The event arguments.
     */
    private void OnNameCompleted(object sender, EventArgs e) {
        if (_object.Name != NameEntry.Text && !Engine.Engine.EngineObjectsManager.IsNameAvailable(NameEntry.Text)) {
            ShowErrorMessageBox("Name is not available");
            NameEntry.BackgroundColor = Colors.Red;
            return;
        }
        _object.Name = NameEntry.Text;
        NameEntry.BackgroundColor = Colors.White;
    }
    
    
    /**
     * OnTemperatureCompleted is called when the user has finished editing the temperature.
     * \param sender The object that sent the event.
     * \param e The event arguments.
     */
    private void OnTemperatureCompleted(object sender, EventArgs e) {
        if (double.TryParse(Temperature.Text, out double temperature)) {
            _object.SimulationTemperature = temperature;
            Temperature.BackgroundColor = Colors.White;
        }
        else {
            Temperature.BackgroundColor = Colors.Red;
        }
    }
    
    /**
     * OnXPositionCompleted is called when the user has finished editing the x position.
     * \param sender The object that sent the event.
     * \param e The event arguments.
     */
    private void OnXPositionCompleted(object sender, EventArgs e) {
        if (double.TryParse(XPosition.Text, out double x)) {
            _object.Position = new Microsoft.Maui.Graphics.Point(x, _object.Position.Y);
        }
    }
    
    /**
     * OnYPositionCompleted is called when the user has finished editing the y position.
     * \param sender The object that sent the event.
     * \param e The event arguments.
     */
    private void OnYPositionCompleted(object sender, EventArgs e) {
        if (double.TryParse(YPosition.Text, out double y)) {
            _object.Position = new Microsoft.Maui.Graphics.Point(_object.Position.X, y);
        }
    }
    
    /**
     * OnHeightCompleted is called when the user has finished editing the height.
     * \param sender The object that sent the event.
     * \param e The event arguments.
     */
    private void OnHeightCompleted(object sender, EventArgs e) {
        if (_object.GetObjectType() == ObjectType.GrainSquare) {
            Height.Text = "1";
        }

        if (double.TryParse(Height.Text, out double height)) {
            _object.Size = new Microsoft.Maui.Graphics.Point(_object.Size.X, height);
        }
    }
    
    /**
     * OnWidthCompleted is called when the user has finished editing the width.
     * \param sender The object that sent the event.
     * \param e The event arguments.
     */
    private void OnWidthCompleted(object sender, EventArgs e) {
        if (_object.GetObjectType() == ObjectType.GrainSquare) {
            Height.Text = "1";
        }

        if (double.TryParse(Width.Text, out double width)) {
            _object.Size = new Microsoft.Maui.Graphics.Point(width, _object.Size.Y);
        }
    }
    
    /**
     * OnMaterialChanged is called when the user selects a material.
     * \param sender The object that sent the event.
     * \param e The event arguments.
     */
    private void OnMaterialChanged(object sender, EventArgs e) {
        if (_object == null) return;
        if (Material.SelectedItem != null)
            _object.Material = (Material)Material.SelectedItem;
    }
    
    /**
     * MakeEntriesWhite makes all the entries white.
     */
    private void MakeEntriesWhite() {
        NameEntry.BackgroundColor = Colors.White;
    }
    
    /**
     * OnCreateButtonClicked is called when the user clicks the create button.
     * \param sender The object that sent the event.
     * \param e The event arguments.
     */
    private void OnCreateButtonClicked(object sender, EventArgs e) {
        // make checks for object name
        Debug.Assert(Engine.Engine.EngineObjectsManager != null, "Engine.Engine.EngineObjectsManager != null");
        if (string.IsNullOrWhiteSpace(NameEntry.Text) || !Engine.Engine.EngineObjectsManager.IsNameAvailable(NameEntry.Text)) {
            // make entry red
            NameEntry.BackgroundColor = Colors.Red;
            return;
        }
        UpdateAll();
        Engine.Engine.EngineObjectsManager.AddObject(_object);
        OnObjectCreated?.Invoke();
        Close();
    }

    /**
     * OnCloseButtonClicked is called when the user clicks the close button.
     * \param sender The object that sent the event.
     * \param e The event arguments.
     */
    private void OnCloseButtonClicked(object sender, EventArgs e) {
        Close();
    }
}