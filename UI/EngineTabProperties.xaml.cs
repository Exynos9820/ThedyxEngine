using ThedyxEngine.Engine;
using ThedyxEngine.Engine.Managers;

namespace ThedyxEngine.UI {
    /**
     * EngineTabProperties is a tab that displays the properties of the selected object.
     */
    public partial class EngineTabProperties : ContentView {
        /** The currently selected object. */
        private EngineObject? _selectedObject;
        /** Whether the tab is enabled. */
        private bool _isEnabled = true;
        /** The event that is called when the object changes. */
        public Action OnObjectChange;
        
        /**
         * Constructor for the EngineTabProperties class.
         */
        public EngineTabProperties() {
            InitializeComponent();
            // On Loaded event, we need to update the properties tab
            this.Loaded += (sender, args) => Update();
        }

        /**
         * Update updates the properties tab.
         */
        public void Update() {
            if (_selectedObject == null) {
                SetFieldsEnabled(false);
                DeleteButton.IsEnabled = false;
                return;
            }
            DeleteButton.IsEnabled = true;

            ClearFields();
            if (_isEnabled)
                SetFieldsEnabled(true);

            SetObjectParameters();
        }
        
        /**
         * Enable enables or disables the tab.
         * \param IsEnabled Whether the tab is enabled.
         */
        public void Enable(bool isEnabled) {
            _isEnabled = isEnabled;
            if (!_isEnabled) {
                SetFieldsEnabled(false);
            }
        }
        
        /**
         * SetObjectParameters sets the selected object.
         * Sets the text fields to the object's parameters.
         * \param obj The object to set.
         */
        private void SetObjectParameters() {
            if (_selectedObject == null) return;

            tbName.Text = _selectedObject.Name;
            tbTemperature.Text = Math.Round(_selectedObject.CurrentTemperature, 2).ToString();
            tbXPosition.Text = _selectedObject.Position.X.ToString();
            tbYPosition.Text = _selectedObject.Position.Y.ToString();
            tbHeight.Text = _selectedObject.Size.Y.ToString();
            tbWidth.Text = _selectedObject.Size.X.ToString();
            
            cbMaterial.ItemsSource = MaterialManager.GetMaterials();
            cbMaterial.SelectedItem = _selectedObject.Material;
            FixedTemperatureCheckBox.IsChecked = _selectedObject.IsTemperatureFixed;
            GasStateAllowed.IsChecked = _selectedObject.IsGasStateAllowed;
        }
        
        /**
         * SetObject sets the selected object.
         * \param obj The object to set.
         */
        public void SetObject(EngineObject obj) {
            ClearFields();
            _selectedObject = obj;
            Update();
        }
        
        /**
         * SetFieldsEnabled sets the fields to be enabled or disabled.
         * \param enabled Whether the fields are enabled.
         */
        private void SetFieldsEnabled(bool enabled) {
            tbName.IsEnabled = enabled;
            tbTemperature.IsEnabled = enabled;
            tbXPosition.IsEnabled = enabled;
            tbYPosition.IsEnabled = enabled;
            tbHeight.IsEnabled = enabled;
            tbWidth.IsEnabled = enabled;
            cbMaterial.IsEnabled = enabled;
            FixedTemperatureCheckBox.IsEnabled = enabled;
        }
        
        /**
         * ClearFields clears the text fields.
         */
        private void ClearFields() {
            tbName.Text = "";
            tbTemperature.Text = "";
            tbXPosition.Text = "";
            tbYPosition.Text = "";
            tbHeight.Text = "";
            tbWidth.Text = "";
            cbMaterial.SelectedIndex = -1;
        }
        
        /**
         * ShowErrorMessageBox shows a message box with an error message.
         * \param text The text to display in the message box.
         */
        private async void ShowErrorMessageBox(string text) {
            await Application.Current.MainPage.DisplayAlert("Error", text, "OK");
        }

        
        /**
         * OnNameCompleted is called when the user has finished editing the name.
         * \param sender The object that sent the event.
         * \param e The event arguments.
         */
        private void OnNameCompleted(object sender, EventArgs e) {
            if (_selectedObject == null) return;
            if (_selectedObject.Name != tbName.Text && !Engine.Engine.EngineObjectsManager.IsNameAvailable(tbName.Text)) {
                ShowErrorMessageBox("Name is not available");
                tbName.BackgroundColor = Colors.Red;
                return;
            }
            _selectedObject.Name = tbName.Text;
            tbName.BackgroundColor = Colors.White;
            OnObjectChange?.Invoke();
        }

        
        /**
         * OnTemperatureCompleted is called when the user has finished editing the temperature.
         * \param sender The object that sent the event.
         * \param e The event arguments.
         */
        private void OnTemperatureCompleted(object sender, EventArgs e) {
            if (_selectedObject == null) return;
            if (double.TryParse(tbTemperature.Text, out double temperature)) {
                _selectedObject.SimulationTemperature = temperature;
                tbTemperature.BackgroundColor = Colors.White;
            }
            else {
                tbTemperature.BackgroundColor = Colors.Red;
            }
            OnObjectChange?.Invoke();
        }
        
        /**
         * OnFixedTemperatureCheckBoxChanged is called when the user has changed the fixed temperature checkbox.
         * \param sender The object that sent the event.
         * \param e The event arguments.
         */
        private void OnFixedTemperatureCheckBoxChanged(object sender, EventArgs e) {
            if (_selectedObject == null) return; 
            _selectedObject.IsTemperatureFixed = FixedTemperatureCheckBox.IsChecked;
            OnObjectChange?.Invoke();
        }
    
        
        private void OnGasStateAllowedCheckBoxChanged(object sender, EventArgs e) {
            if (_selectedObject == null) return; 
            _selectedObject.IsGasStateAllowed = GasStateAllowed.IsChecked;
            OnObjectChange?.Invoke();
        }
        
        /**
         * OnXPositionCompleted is called when the user has finished editing the x position.
         * \param sender The object that sent the event.
         * \param e The event arguments.
         */
        private void OnXPositionCompleted(object sender, EventArgs e) {
            if (_selectedObject == null) return;
            if (double.TryParse(tbXPosition.Text, out var x)) {
                _selectedObject.Position = new Microsoft.Maui.Graphics.Point(x, _selectedObject.Position.Y);
            }
            OnObjectChange?.Invoke();
        }
    
        
        /**
         * OnYPositionCompleted is called when the user has finished editing the y position.
         * \param sender The object that sent the event.
         * \param e The event arguments.
         */
        private void OnYPositionCompleted(object sender, EventArgs e) {
            if (_selectedObject == null) return;
            if (double.TryParse(tbYPosition.Text, out double y)) {
                _selectedObject.Position = new Microsoft.Maui.Graphics.Point(_selectedObject.Position.X, y);
            }
            OnObjectChange?.Invoke();
        }
    
        /**
         * OnHeightCompleted is called when the user has finished editing the height.
         * \param sender The object that sent the event.
         * \param e The event arguments.
         */
        private void OnHeightCompleted(object sender, EventArgs e) {
            if (_selectedObject == null) return;
            if (_selectedObject.GetObjectType() == ObjectType.GrainSquare) {
                tbHeight.Text = "1";
            }

            if (double.TryParse(tbHeight.Text, out double height)) {
                _selectedObject.Size = new Microsoft.Maui.Graphics.Point(_selectedObject.Size.X, height);
            }
            OnObjectChange?.Invoke();
        }
        
        
        /**
         * OnWidthCompleted is called when the user has finished editing the width.
         * \param sender The object that sent the event.
         * \param e The event arguments.
         */
        private void OnWidthCompleted(object sender, EventArgs e) {
            if (_selectedObject == null) return;
            if (_selectedObject.GetObjectType() == ObjectType.GrainSquare) {
                tbHeight.Text = "1";
            }

            if (double.TryParse(tbWidth.Text, out double width)) {
                _selectedObject.Size = new Microsoft.Maui.Graphics.Point(width, _selectedObject.Size.Y);
            }
            OnObjectChange?.Invoke();
        }
        
        /**
         * OnMaterialChanged is called when the user has selected a material.
         * \param sender The object that sent the event.
         * \param e The event arguments.
         */
        private void OnMaterialChanged(object sender, EventArgs e) {
            if (_selectedObject == null) return;
            if (cbMaterial.SelectedItem != null)
                _selectedObject.Material = (Material)cbMaterial.SelectedItem;
        }
        
        /**
         * OnDeleteButtonClicked is called when the user clicks the delete button.
         * \param sender The object that sent the event.
         * \param e The event arguments.
         */
        private void OnDeleteButtonClicked(object sender, EventArgs e) {
            if(Engine.Engine.IsRunning()) return;
            ClearFields();
            Engine.Engine.EngineObjectsManager.RemoveObject(_selectedObject);
            _selectedObject = null;
            OnObjectChange?.Invoke();
        }
    }
}
