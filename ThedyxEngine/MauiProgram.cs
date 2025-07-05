using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using Microsoft.Extensions.Logging;

/**
 * \namespace ThedyxEngine
 * \brief Main namespace of the application
 */
namespace ThedyxEngine;

/**
 * \class MauiProgram
 * \brief Registers CommunityToolkit and file-picker services.
 */
public static class MauiProgram {
    public static MauiApp CreateMauiApp() {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }).ConfigureFilePicker(100);
        
        builder.Services
            .AddFilePicker()
            .AddSingleton<MainPage>() 
            .AddSingleton(FileSaver.Default)
            .AddSingleton<MainPage>();
        
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}