using System.Globalization;
using CommunityToolkit.Maui.Views;
using ThedyxEngine.Util;

namespace ThedyxEngine.UI;

/**
 * SettingsPopup is a popup that allows the user to change the settings of the simulation.
 */
public partial class SettingsPopup : Popup {
    /**
     * Constructor for the SettingsPopup class.
     */
    public SettingsPopup() {
        InitializeComponent();
        FillEntriesValues();
    }
    
    /**
     * ShowErrorMessageBox shows a message box with an error message.
     * \param text The text to display in the message box.
     */
    private async void ShowErrorMessageBox(string text) {
        await Application.Current.MainPage.DisplayAlert("Error", text, "OK");
    }
    
    /**
     * FillEntriesValues fills the entries with the current values of the settings.
     */
    private void FillEntriesValues() {
        RoomTemperature.Text        = GlobalVariables.AirTemperature.ToString(CultureInfo.InvariantCulture);
        RadiationDepth.Text         = GlobalVariables.RadiationDepth.ToString();
        EngineUpdatesPerSecond.Text = GlobalVariables.EngineIntervalUpdatePerSecond.ToString(CultureInfo.InvariantCulture);
        UIUpdatesPerSecond.Text     = GlobalVariables.WindowRefreshRate.ToString();
        IsHumanReadable.IsChecked   = GlobalVariables.SaveSimulationHumanReadable;
        IsObjectLooseHeatToAir.IsChecked = GlobalVariables.ObjectsLooseHeatToAir;
        IsWaitToBeInTime.IsChecked = GlobalVariables.WaitToBeInTime;
        MinTemperatureColor.Text = GlobalVariables.MinTemperatureColor.ToString();
        MaxTemperatureColor.Text = GlobalVariables.MaxTemperatureColor.ToString();
    }
    
    /**
     * OnTemperatureCompleted is called when the user has finished editing the temperature.
     * \param sender The object that sent the event.
     * \param e The event arguments.
     */
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
    
    /**
     * OnRadiationCompleted is called when the user has finished editing the radiation depth.
     * \param sender The object that sent the event.
     * \param e The event arguments.
     */
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
    
    /**
     * OnEngineUpdatesCompleted is called when the user has finished editing the engine updates per second.
     * \param sender The object that sent the event.
     * \param e The event arguments.
     */
    private void OnEngineUpdatesCompleted(object sender, EventArgs e) {
        if (int.TryParse(EngineUpdatesPerSecond.Text, out var updates)) {
            if (updates >= 15 && updates <= 1200) {
                GlobalVariables.EngineIntervalUpdatePerSecond = updates;
            }
            else {
                ShowErrorMessageBox("Engine updates per second must be between 15 and 1200");
            }
        }else {
            ShowErrorMessageBox("Engine updates per second must be a number");   
        }
        EngineUpdatesPerSecond.Text = GlobalVariables.EngineIntervalUpdatePerSecond.ToString(CultureInfo.InvariantCulture);
    }
    
    /**
     * OnUIUpdatesCompleted is called when the user has finished editing the UI updates per second.
     * \param sender The object that sent the event.
     * \param e The event arguments.
     */
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
    
    /**
     * OnFileCheckBoxChanged is called when the user has changed the human readable checkbox.
     * \param sender The object that sent the event.
     * \param e The event arguments.
     */
    private void OnFileCheckBoxChanged(object sender, EventArgs e) {
        GlobalVariables.SaveSimulationHumanReadable = IsHumanReadable.IsChecked;
    }

    
    /**
     * OnMinTemperatureColorChanged is called when the user has finished editing the min temperature color.
     * \param sender The object that sent the event.
     * \param e The event arguments.
     */
    private void OnMinTemperatureColorChanged(object sender, EventArgs e) {
        if (int.TryParse(MinTemperatureColor.Text, out var minTemp)) {
            if (minTemp >= 0 && minTemp < GlobalVariables.MaxTemperatureColor) {
                GlobalVariables.MinTemperatureColor = minTemp;
            }
            else {
                ShowErrorMessageBox(
                    "Min temperature color must be between 0 and " + GlobalVariables.MaxTemperatureColor);
            }
        }
    }
    
    /**
     * OnMaxTemperatureColorChanged is called when the user has finished editing the max temperature color.
     * \param sender The object that sent the event.
     * \param e The event arguments.
     */
    private void OnMaxTemperatureColorChanged(object sender, EventArgs e) {
        if (int.TryParse(MaxTemperatureColor.Text, out var maxTemp)) {
            if (maxTemp > GlobalVariables.MinTemperatureColor && maxTemp < 1000) {
                GlobalVariables.MaxTemperatureColor = maxTemp;
            }
            else {
                ShowErrorMessageBox(
                    "Max temperature color must be between " + GlobalVariables.MinTemperatureColor + " and 1000");
            }
        }
    }



    /**
     * IsObjectsLooseHeatToAirChanged is called when the user has changed the objects loose heat to air checkbox.
     * \param sender The object that sent the event.
     * \param e The event arguments.
     */
    private void IsObjectsLooseHeatToAirChanged(object sender, EventArgs e) {
        GlobalVariables.ObjectsLooseHeatToAir = IsObjectLooseHeatToAir.IsChecked;
    }
    
    
    /**
     * OnWaitToBeInTimeChanged is called when the user has changed the wait to be in time checkbox.
     * \param sender The object that sent the event.
     * \param e The event arguments.
     */
    private void OnWaitToBeInTimeChanged(object sender, EventArgs e) {
        GlobalVariables.WaitToBeInTime = IsWaitToBeInTime.IsChecked;
    }
}