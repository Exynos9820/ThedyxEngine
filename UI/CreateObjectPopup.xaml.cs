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

public partial class CreateObjectPopup : Popup {
    private ObjectType _objectType;
    private EngineObject _object;
    public Action OnObjectCreated;
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
            case ObjectType.StateChangeRectangle:
                TitleLabel.Text = "Create State Change Rectangle";
                _object = new EngineLiquid("", 10, 10);
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
    private async void ShowErrorMessageBox(string text) {
        await Application.Current.MainPage.DisplayAlert("Error", text, "OK");
    }

    private void UpdateAll() {
        OnNameCompleted(null, null);
        OnTemperatureCompleted(null, null);
        OnXPositionCompleted(null, null);
        OnYPositionCompleted(null, null);
        OnHeightCompleted(null, null);
        OnWidthCompleted(null, null);
        OnMaterialChanged(null, null);
    }

    
    private void OnNameCompleted(object sender, EventArgs e) {
        if (_object.Name != NameEntry.Text && !Engine.Engine.EngineObjectsManager.IsNameAvailable(NameEntry.Text)) {
            ShowErrorMessageBox("Name is not available");
            NameEntry.BackgroundColor = Colors.Red;
            return;
        }
        _object.Name = NameEntry.Text;
        NameEntry.BackgroundColor = Colors.White;
    }
    
    private void OnTemperatureCompleted(object sender, EventArgs e) {
        if (double.TryParse(Temperature.Text, out double temperature)) {
            _object.SimulationTemperature = temperature;
            Temperature.BackgroundColor = Colors.White;
        }
        else {
            Temperature.BackgroundColor = Colors.Red;
        }
    }
    private void OnXPositionCompleted(object sender, EventArgs e) {
        if (double.TryParse(XPosition.Text, out double x)) {
            _object.Position = new Microsoft.Maui.Graphics.Point(x, _object.Position.Y);
        }
    }

    private void OnYPositionCompleted(object sender, EventArgs e) {
        if (double.TryParse(YPosition.Text, out double y)) {
            _object.Position = new Microsoft.Maui.Graphics.Point(_object.Position.X, y);
        }
    }

    private void OnHeightCompleted(object sender, EventArgs e) {
        if (_object.GetObjectType() == ObjectType.GrainSquare) {
            Height.Text = "1";
        }

        if (double.TryParse(Height.Text, out double height)) {
            _object.Size = new Microsoft.Maui.Graphics.Point(_object.Size.X, height);
        }
    }
    
    private void OnWidthCompleted(object sender, EventArgs e) {
        if (_object.GetObjectType() == ObjectType.GrainSquare) {
            Height.Text = "1";
        }

        if (double.TryParse(Width.Text, out double width)) {
            _object.Size = new Microsoft.Maui.Graphics.Point(width, _object.Size.Y);
        }
    }
    
    private void OnMaterialChanged(object sender, EventArgs e) {
        if (_object == null) return;
        if (Material.SelectedItem != null)
            _object.Material = (Material)Material.SelectedItem;
    }
    private void MakeEntriesWhite() {
        NameEntry.BackgroundColor = Colors.White;
    }
    
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

    private void OnCloseButtonClicked(object sender, EventArgs e) {
        Close();
    }
}