using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using ThedyxEngine.Engine;
using ThedyxEngine.Engine.Managers;

namespace ThedyxEngine.UI
{
    public partial class EngineTabProperties : ContentView
    {
        private EngineObject? _selectedObject;
        private bool isEnabled = true;

        public EngineTabProperties()
        {
            InitializeComponent();
            // On Loaded event, we need to update the properties tab
            this.Loaded += (sender, args) => Update();
        }

        public void Update()
        {
            if (_selectedObject == null)
            {
                SetFieldsEnabled(false);
                return;
            }

            ClearFields();
            if (isEnabled)
                SetFieldsEnabled(true);

            SetObjectParameters();
        }

        public void Enable(bool IsEnabled)
        {
            isEnabled = IsEnabled;
            if (!isEnabled)
            {
                SetFieldsEnabled(false);
            }
        }

        private void SetObjectParameters()
        {
            if (_selectedObject == null) return;

            tbName.Text = _selectedObject.Name;
            tbTemperature.Text = Math.Round(_selectedObject.CurrentTemperature, 2).ToString();
            tbXPosition.Text = _selectedObject.Position.X.ToString();
            tbYPosition.Text = _selectedObject.Position.Y.ToString();
            tbHeight.Text = _selectedObject.Size.Y.ToString();
            tbWidth.Text = _selectedObject.Size.X.ToString();
            
            cbMaterial.ItemsSource = MaterialManager.GetMaterials();
            cbMaterial.SelectedItem = _selectedObject.Material;
        }

        public void SetObject(EngineObject obj)
        {
            ClearFields();
            _selectedObject = obj;
            Update();
        }

        private void SetFieldsEnabled(bool enabled)
        {
            tbName.IsEnabled = enabled;
            tbTemperature.IsEnabled = enabled;
            tbXPosition.IsEnabled = enabled;
            tbYPosition.IsEnabled = enabled;
            tbHeight.IsEnabled = enabled;
            tbWidth.IsEnabled = enabled;
            cbMaterial.IsEnabled = enabled;
        }

        private void ClearFields()
        {
            tbName.Text = "";
            tbTemperature.Text = "";
            tbXPosition.Text = "";
            tbYPosition.Text = "";
            tbHeight.Text = "";
            tbWidth.Text = "";
            cbMaterial.SelectedIndex = -1;
        }

        private async void ShowErrorMessageBox(string text)
        {
            await Application.Current.MainPage.DisplayAlert("Error", text, "OK");
        }

        private void OnNameCompleted(object sender, EventArgs e)
        {
            if (_selectedObject == null) return;
            if (_selectedObject.Name != tbName.Text && !Engine.Engine.EngineObjectsManager.IsNameAvailable(tbName.Text))
            {
                ShowErrorMessageBox("Name is not available");
                tbName.BackgroundColor = Colors.Red;
                return;
            }
            _selectedObject.Name = tbName.Text;
            tbName.BackgroundColor = Colors.White;
        }

        private void OnTemperatureCompleted(object sender, EventArgs e)
        {
            if (_selectedObject == null) return;
            if (double.TryParse(tbTemperature.Text, out double temperature))
            {
                _selectedObject.SimulationTemperature = temperature;
                tbTemperature.BackgroundColor = Colors.White;
            }
            else
            {
                tbTemperature.BackgroundColor = Colors.Red;
            }
        }

        private void OnXPositionCompleted(object sender, EventArgs e)
        {
            if (_selectedObject == null) return;
            if (double.TryParse(tbXPosition.Text, out double x))
            {
                _selectedObject.Position = new Microsoft.Maui.Graphics.Point(x, _selectedObject.Position.Y);
            }
        }

        private void OnYPositionCompleted(object sender, EventArgs e)
        {
            if (_selectedObject == null) return;
            if (double.TryParse(tbYPosition.Text, out double y))
            {
                _selectedObject.Position = new Microsoft.Maui.Graphics.Point(_selectedObject.Position.X, y);
            }
        }

        private void OnHeightCompleted(object sender, EventArgs e)
        {
            if (_selectedObject == null) return;
            if (_selectedObject.GetObjectType() == ObjectType.GrainSquare)
            {
                tbHeight.Text = "1";
            }

            if (double.TryParse(tbHeight.Text, out double height))
            {
                _selectedObject.Size = new Microsoft.Maui.Graphics.Point(_selectedObject.Size.X, height);
            }
        }
        
        private void OnWidthCompleted(object sender, EventArgs e)
        {
            if (_selectedObject == null) return;
            if (_selectedObject.GetObjectType() == ObjectType.GrainSquare)
            {
                tbHeight.Text = "1";
            }

            if (double.TryParse(tbWidth.Text, out double width))
            {
                _selectedObject.Size = new Microsoft.Maui.Graphics.Point(width, _selectedObject.Size.Y);
            }
        }

        private void OnMaterialChanged(object sender, EventArgs e)
        {
            if (_selectedObject == null) return;
            if (cbMaterial.SelectedItem != null)
                _selectedObject.Material = (Material)cbMaterial.SelectedItem;
        }
    }
}
