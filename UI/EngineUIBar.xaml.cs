using Microsoft.Maui.Controls;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using ThedyxEngine.Engine;
using ThedyxEngine.Util;
using CommunityToolkit.Maui.Views;
using ThedyxEngine.Engine.Managers;
using CommunityToolkit.Maui.Storage;
using LukeMauiFilePicker;
using Newtonsoft.Json;

namespace ThedyxEngine.UI {
    /**
     * EngineUIBar is a class that represents the UI bar at the top of the screen.
     * It contains buttons for starting, stopping, and pausing the simulation, as well as buttons for adding, saving, and opening objects.
     */
    public partial class EngineUIBar : ContentView {
        /** The event that is called when the UI needs to be updated. */
        public Action? UpdateUI;
        /** The event that is called when the selected object is deleted. */
        public Action<EngineObject>? DeleteSelected;
        /** The event that is called when the engine mode is changed. */
        public Action? EngineModeChanged;
        /** The main page of the app. */
        public MainPage? MainPage;
        
        /**
         * Constructor for the EngineUIBar class.
         */
        public EngineUIBar() {
            InitializeComponent();
            Loaded += (sender, args) => {
                SetStoppedMode();
            };
        }

        /**
         * SetStoppedMode sets the UI to the stopped mode.
         */
        public void SetStoppedMode() {
            // Set engine controls
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            PauseButton.IsEnabled = false;

            // Set UI controls
            AddButton.IsEnabled = true;
            SaveButton.IsEnabled = true;
            OpenButton.IsEnabled = true;
            ClearButton.IsEnabled = true;
            ResetButton.IsEnabled = true;
            MaterialsButton.IsEnabled = true;
            SettingsButton.IsEnabled = true;
        }

        /**
         * Update updates the time label on the UI.
         */
        public void Update() {
            long currentTime = Engine.Engine.GetSimulationTime();
            TimeSpan time = TimeSpan.FromMilliseconds(currentTime);

            if (time.Days > 0) {
                // More than one day: show days, hours, and minutes
                TimeLabel.Text = time.ToString(@"dd\:hh\:mm");
            } 
            else if (time.Hours > 0) {
                // More than one hour but less than a day: show hours, minutes, and seconds
                TimeLabel.Text = time.ToString(@"hh\:mm\:ss");
            } 
            else {
                // Less than one hour: show minutes, seconds, and centiseconds
                TimeLabel.Text = time.ToString(@"mm\:ss\:ff");
            }
        }

        /**
         * SetRunningMode sets the UI to the running mode.
         */
        private void SetRunningMode() {
            // Set engine controls
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            PauseButton.IsEnabled = true;

            // Set UI controls
            AddButton.IsEnabled = false;
            SaveButton.IsEnabled = false;
            OpenButton.IsEnabled = false;
            ClearButton.IsEnabled = false;
            ResetButton.IsEnabled = false;
            MaterialsButton.IsEnabled = false;
            SettingsButton.IsEnabled = false;
        }

        /**
         * SetPausedMode sets the UI to the paused mode.
         */
        private void SetPausedMode() {
            // Set engine controls
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = true;
            PauseButton.IsEnabled = false;

            // Set UI controls
            AddButton.IsEnabled = false;
            SaveButton.IsEnabled = false;
            OpenButton.IsEnabled = false;
            ClearButton.IsEnabled = false;
            MaterialsButton.IsEnabled = false;
            SettingsButton.IsEnabled = false;
            ResetButton.IsEnabled = true;
        }
        
        /**
         * OnSaveButtonClicked is called when the user clicks the save button.
         * \param sender The object that sent the event.
         * \param e The event arguments.
         */
        private async void OnSaveButtonClicked(object sender, EventArgs e) {
            using var memoryStream = new MemoryStream(FileManager.GetSimulationRepresentation());
            var result = await FileSaver.Default.SaveAsync("simulation.tdx", memoryStream);
        }


        /**
         * OnOpenButtonClicked is called when the user clicks the open button.
         * \param sender The object that sent the event.
         * \param e The event arguments.
         */
        private async void OnOpenButtonClicked(object sender, EventArgs e) {
            var customFileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>> {
                { DevicePlatform.iOS, new[] { ".tdx" } },
                { DevicePlatform.MacCatalyst, new[] { "public.data" } },
                { DevicePlatform.WinUI, new[] { ".tdx" } },
                { DevicePlatform.Android, new[] { ".tdx" } }
            });

            var options = new PickOptions {
                PickerTitle = "Select a .tdx File",
                FileTypes = customFileTypes
            };

            var file = await FilePicker.Default.PickAsync(options);  // Await the task

            if (file != null) {
                try {
                    using (var stream = await file.OpenReadAsync())  // Read the file as a stream
                    using (var reader = new StreamReader(stream)) {
                        string fileContent = await reader.ReadToEndAsync();  // Read content as a string
                        FileManager.LoadFromContent(fileContent);  // Pass content to LoadFromContent
                    }
                }
                catch (Exception ex) {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Failed to read the file: {ex.Message}", "OK");
                }
            }
            Engine.Engine.ResetSimulation();
        }

        /**
         * OnClearButtonClicked is called when the user clicks the clear button.
         * \param sender The object that sent the event.
         * \param e The event arguments.
         */
        private async void OnClearButtonClicked(object sender, EventArgs e) {
            bool result = await Application.Current.MainPage.DisplayAlert("Confirm Clear", "Are you sure you want to clear all data?", "Yes", "No");
            if (result) {
                Engine.Engine.ClearSimulation();
                DeleteSelected?.Invoke(null);
            }
        }

        /**
         * OnAddButtonClicked is called when the user clicks the add button.
         * \param sender The object that sent the event.
         * \param e The event arguments.
         */
        private async void OnAddButtonClicked(object sender, EventArgs e) {
            string action = await Application.Current.MainPage.DisplayActionSheet("Add Item", "Cancel", null, "Grain Object", "Solid Object", "State Object");
            switch (action) {
                case "Grain Object":
                    AddObject(ObjectType.GrainSquare);
                    break;
                case "Solid Object":
                    AddObject(ObjectType.Rectangle);
                    break;
                case "State Object":
                    AddObject(ObjectType.StateRectangle);
                    break;
            }
        }

        /**
         * AddObject opens a popup to add an object to the simulation.
         * \param objectType The type of object to add.
         */
        private void AddObject(ObjectType objectType) {
            var createPopup = new CreateObjectPopup(objectType);
            this.MainPage?.ShowPopup(createPopup);
            if (UpdateUI != null) createPopup.OnObjectCreated += UpdateAll;
        }
        
        /**
         * OnSettingsButtonClicked is called when the user clicks the settings button.
         * \param sender The object that sent the event.
         * \param e The event arguments.
         */
        private void UpdateAll() {
            UpdateUI?.Invoke();
        }

        /**
         * OnStartButtonClicked is called when the user clicks the start button.
         * \param sender The object that sent the event.
         * \param e The event arguments.
         */
        private void OnStartButtonClicked(object sender, EventArgs e) {
            Engine.Engine.Start();
            EngineModeChanged?.Invoke();
            SetRunningMode();
        }
        
        /**
         * OnStopButtonClicked is called when the user clicks the stop button.
         * \param sender The object that sent the event.
         * \param e The event arguments.
         */
        private void OnStopButtonClicked(object sender, EventArgs e) {
            Engine.Engine.Stop();
            EngineModeChanged?.Invoke();
            SetStoppedMode();
        }

        /**
         * OnPauseButtonClicked is called when the user clicks the pause button.
         * \param sender The object that sent the event.
         * \param e The event arguments.
         */
        private void OnPauseButtonClicked(object sender, EventArgs e) {
            Engine.Engine.Pause();
            EngineModeChanged?.Invoke();
            SetPausedMode();
        }
    
        /**
         * OnResetButtonClicked is called when the user clicks the reset button.
         * Resets the simulation data.
         * \param sender The object that sent the event.
         * \param e The event arguments.
         */
        private void OnTemperatureModeButtonClicked(object sender, EventArgs e) {
            Engine.Engine.ShowTemperature = !Engine.Engine.ShowTemperature;
            // change color of button
            if (Engine.Engine.ShowTemperature) {
                ModeButton.BackgroundColor = Colors.Olive;
            } else {
                ModeButton.BackgroundColor = Colors.DarkGray;
            }
            UpdateUI?.Invoke();
        }
        
        /**
         * OnMaterialsButtonClicked is called when the user clicks the materials button.
         * Opens the materials popup.
         * \param sender The object that sent the event.
         * \param e The event arguments.
         */
        private void OnMaterialsButtonClicked(object sender, EventArgs e) {
            var materialsPopup = new MaterialsPopup();
            this.MainPage?.ShowPopup(materialsPopup);
            // use action to reopen the popup
            materialsPopup.ReopenMaterialPopup = OnMaterialsButtonClicked;
            UpdateUI?.Invoke();
        }
    
        /**
         * OnSettingsButtonClicked is called when the user clicks the settings button.
         * Opens the settings popup.
         * Used when we want to reopen the materials popup.
         * \param sender The object that sent the event.
         * \param e The event arguments.
         */
        private void OnMaterialsButtonClicked(Material obj) {
            var materialsPopup = new MaterialsPopup();
            this.MainPage?.ShowPopup(materialsPopup);
            materialsPopup.SelectMaterial(obj);
            // use action to reopen the popup
            materialsPopup.ReopenMaterialPopup = OnMaterialsButtonClicked;
            UpdateUI?.Invoke();
        }

        /**
         * OnSettingsButtonClicked is called when the user clicks the settings button.
         * Opens the settings popup.
         * \param sender The object that sent the event.
         * \param e The event arguments.
         */
        private void OnSettingsButtonClicked(object sender, EventArgs e) {
            var settingsPopup = new SettingsPopup();
            this.MainPage?.ShowPopup(settingsPopup);
            UpdateUI?.Invoke();
        }
        
        /**
         * OnGridButtonClicked is called when the user clicks the grid button.
         * Toggles the grid on and off.
         * \param sender The object that sent the event.
         * \param e The event arguments.
         */
        private void OnGridButtonClicked(object sender, EventArgs e) {
            Engine.Engine.ShowGrid = !Engine.Engine.ShowGrid;
            // change color of button
            if (Engine.Engine.ShowGrid) {
                GridButton.BackgroundColor = Colors.Peru;
            } else {
                GridButton.BackgroundColor = Colors.DarkGray;
            }
            UpdateUI?.Invoke();
        }
        
        /**
         * OnColorModeButtonClicked is called when the user clicks the color mode button.
         * Toggles the color mode on and off.
         * \param sender The object that sent the event.
         * \param e The event arguments.
         */
        private void OnColorModeButtonClicked(object sender, EventArgs e) {
            Engine.Engine.ShowColor = !Engine.Engine.ShowColor;
            if (Engine.Engine.ShowColor) {
                ColorModeButton.BackgroundColor = Colors.Salmon;
                ColorModeButton.Text = "Color";
            }else {
                ColorModeButton.BackgroundColor = Colors.DarkGray;
                ColorModeButton.Text = "Temp";
            }
            UpdateUI?.Invoke();
        }
        
        /**
         * OnResetButtonClicked is called when the user clicks the reset button.
         * Resets the simulation data.
         * \param sender The object that sent the event.
         * \param e The event arguments.
         */
        private async void OnResetButtonClicked(object sender, EventArgs e) {
            bool result = await Application.Current.MainPage.DisplayAlert("Confirm Reset", "Are you sure you want to reset simulation data?", "Yes", "No");
            if (result) {
                Engine.Engine.ResetSimulation();
            }
        }
    }
}
