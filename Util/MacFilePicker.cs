using UIKit;
using UniformTypeIdentifiers;

namespace ThedyxEngine.Util;

public partial class MacFilePicker
{
    public static async Task<FileResult?> PickAsync(PickOptions options)
    {
        var tcs = new TaskCompletionSource<string>();

        var utTypes = options.FileTypes?.Value.Select(q =>
                UTType.CreateFromIdentifier(q) ??
                UTType.CreateFromMimeType(q) ??
                UTType.CreateFromExtension(q) ??
                UTTypes.Item)
            .ToArray();

        var picker = new UIDocumentPickerViewController(
            utTypes ?? Array.Empty<UTType>(),
            asCopy: true)
        {
            AllowsMultipleSelection = false
        };

        TaskCompletionSource<FileResult?> filesTcs = new();
        picker.DidPickDocumentAtUrls += (_, e) =>
        {
            if (e.Urls.Length == 0)
            {
                filesTcs.TrySetResult(null);
                return;
            }
            var path = e.Urls[0].Path;
            if (path == null)
            {
                filesTcs.TrySetResult(null);
                return;
            }
            filesTcs.TrySetResult(new FileResult(path));
        };

        picker.WasCancelled += (_, e) =>
        {
            filesTcs.TrySetResult(null);
        };

        var controller = Platform.GetCurrentUIViewController();
        ArgumentNullException.ThrowIfNull(controller);

        await controller.PresentViewControllerAsync(picker, true);

        return await filesTcs.Task;
    }
}