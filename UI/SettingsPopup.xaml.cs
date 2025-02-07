using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using ThedyxEngine.Engine;
using ThedyxEngine.Engine.Managers;
using ThedyxEngine.Util;

namespace ThedyxEngine.UI;

public partial class SettingsPopup : Popup {
    public SettingsPopup() {
        InitializeComponent();
        FillEntriesValues();
    }
    
    private async void ShowErrorMessageBox(string text) {
        await Application.Current.MainPage.DisplayAlert("Error", text, "OK");
    }
    private void FillEntriesValues() {
        RoomTemperature.Text        = GlobalVariables.AirTemperature.ToString();
        RadiationDepth.Text         = GlobalVariables.RadiationDepth.ToString();
        EngineUpdatesPerSecond.Text = GlobalVariables.EngineIntervalUpdatePerSecond.ToString();
    }
    
    private void OnTemperatureCompleted(object sender, EventArgs e) {
        if (double.TryParse(RoomTemperature.Text, out var temp)) {
            if (temp >= 0 && temp < 1000) {
                GlobalVariables.AirTemperature = temp;
            }
            else {
                ShowErrorMessageBox("Temperature must be between 0 and 1000");
            }
        }else {
            ShowErrorMessageBox("Temperature must be a number");   
        }
        RoomTemperature.Text = GlobalVariables.AirTemperature.ToString();
    }
    
    private void OnRadiationCompleted(object sender, EventArgs e) {
        if (int.TryParse(RadiationDepth.Text, out var depth)) {
            if (depth >= 0 && depth < 30) {
                GlobalVariables.RadiationDepth = depth;
            }
            else {
                ShowErrorMessageBox("Radiation depth for calculations must be between 0 and 30");
            }
        }else {
            ShowErrorMessageBox("Radiation depth must be a number");   
        }
        RadiationDepth.Text = GlobalVariables.RadiationDepth.ToString();
    }
    
    private void OnEngineUpdatesCompleted(object sender, EventArgs e) {
        if (int.TryParse(EngineUpdatesPerSecond.Text, out var updates)) {
            if (updates >= 15 && updates < 240) {
                GlobalVariables.EngineIntervalUpdatePerSecond = updates;
            }
            else {
                ShowErrorMessageBox("Engine updates per second must be between 15 and 240");
            }
        }else {
            ShowErrorMessageBox("Engine updates per second must be a number");   
        }
        EngineUpdatesPerSecond.Text = GlobalVariables.EngineIntervalUpdatePerSecond.ToString();
    }
    
    private void OnUIUpdatesCompleted(object sender, EventArgs e) {
        if (int.TryParse(UIUpdatesPerSecond.Text, out var updates)) {
            if (updates >= 15 && updates < 240) {
                GlobalVariables.WindowRefreshRate = updates;
            }
            else {
                ShowErrorMessageBox("UI updates per second must be between 15 and 240");
            }
        }else {
            ShowErrorMessageBox("UI updates per second must be a number");   
        }
        UIUpdatesPerSecond.Text = GlobalVariables.WindowRefreshRate.ToString();
    }
}