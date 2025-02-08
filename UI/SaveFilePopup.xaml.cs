using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using Newtonsoft.Json;
using ThedyxEngine.Util;

namespace ThedyxEngine.UI;

public partial class SaveFilePopup : Popup
{
    public SaveFilePopup() {
        InitializeComponent();
    }
    
    private async void OnSaveClicked(object sender, EventArgs e) {
        if (string.IsNullOrEmpty(FileName.Text)) {
            await Application.Current.MainPage.DisplayAlert("Error", "File name cannot be empty", "OK");
            return;
        }
        string fileName = FileName.Text;
        string json = FileManager.GetObjectsJsonRepresentation();
        
        string workingDirectory = Environment.CurrentDirectory;
        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"{fileName}.tdx");
        // check if file exists
        if (File.Exists(filePath)) {
            // if file exists, ask user if he wants to overwrite it
            bool overwrite = await Application.Current.MainPage.DisplayAlert("Warning", "File already exists. Do you want to overwrite it?", "Yes", "No");
            if (!overwrite) {
                return;
            }
        }
        
        using (StreamWriter outputFile = new StreamWriter(filePath)) {
                outputFile.Write(json);
        }
        // Show success message
        await Application.Current.MainPage.DisplayAlert("Success", $"File saved successfully to {filePath}", "OK");
        this.Close();
        
    }
}