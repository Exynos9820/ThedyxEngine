namespace ThedyxEngine;

/**
 * \class App
 * \brief Initialization of the components
 */
public partial class App : Application {
    public App() {
        InitializeComponent();

        MainPage = new AppShell();
    }
}