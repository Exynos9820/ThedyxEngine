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

public partial class SettingsPopup : Popup {
    public SettingsPopup() {
        InitializeComponent();
        FillEntriesValues();
    }
    
    private async void ShowErrorMessageBox(string text) {
        await Application.Current.MainPage.DisplayAlert("Error", text, "OK");
    }
    private void FillEntriesValues() {
        RoomTemperature.Text = Engine.Engine.AirTemperature.ToString();
    }
    
    private void OnTemperatureCompleted(object sender, EventArgs e) {
        if (double.TryParse(RoomTemperature.Text, out double temp)) {
            if (temp >= 0 && temp < 1000) {
                Engine.Engine.AirTemperature = temp;
            }
            else {
                ShowErrorMessageBox("Temperature must be between 0 and 1000");
            }
        }else {
            ShowErrorMessageBox("Temperature must be a number");   
        }
        RoomTemperature.Text = Engine.Engine.AirTemperature.ToString();
    }
}