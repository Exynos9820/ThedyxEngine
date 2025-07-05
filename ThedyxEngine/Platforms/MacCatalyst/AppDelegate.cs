using Foundation;

namespace ThedyxEngine;

/**
 * \class AppDelegate
 * \brief Entry point
 */
[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate {
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}