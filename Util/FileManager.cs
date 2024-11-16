using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ThedyxEngine.Engine;
using LukeMauiFilePicker;

namespace ThedyxEngine.Util
{
    /**
     * \class FileManager
     * \brief Manages file operations for saving and loading engine objects.
     */
    public static class FileManager
    {
        public static IFilePickerService Picker;
        public static void Init(IFilePickerService picker) {
            Picker = picker;
        }
        
        private static readonly ILog log = LogManager.GetLogger(typeof(FileManager));   // Logger
        static readonly Dictionary<DevicePlatform, IEnumerable<string>> FileType = new()
        {
            {  DevicePlatform.Android, new[] { "text/*" } } ,
            { DevicePlatform.iOS, new[] { "public.json", "public.plain-text" } },
            { DevicePlatform.MacCatalyst, new[] { "public.json", "public.plain-text" } },
            { DevicePlatform.WinUI, new[] { ".txt", ".json" } }
        };
        /**
         * Save to file with user-specified file path using LukeMauiFilePicker.
         */
        public static async void Save()
        {
            var bytes = Encoding.UTF8.GetBytes("Hello World");
            using var memory = new MemoryStream(bytes);

            await Picker.SaveFileAsync(new("text.txt", memory)
            {
                AndroidMimeType = "text/plain",
                WindowsFileTypes = ("Text files", new() { ".txt", })
            });
        }

        /**
         * Load from file with user-specified file path using LukeMauiFilePicker.
         */
        static async Task OnFilesPickedAsync(IEnumerable<IPickFile> files)
        {
            var str = new StringBuilder();

            foreach (var f in files)
            {
                using var s = await f.OpenReadAsync();
                using var reader = new StreamReader(s);

                str.AppendLine(await reader.ReadToEndAsync());
            }

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
            });
        }

        public static async void PickOne()
        {
            var file = await Picker.PickFileAsync("Select a file", FileType);
            if (file is null) { return; }

            await OnFilesPickedAsync(new[] { file });
        }
    }
}
