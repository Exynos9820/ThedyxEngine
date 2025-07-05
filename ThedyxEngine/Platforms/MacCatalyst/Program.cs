using UIKit;

namespace ThedyxEngine;

/**
 * \class Program
 * \brief Starts the application by invoking the native launcher.
 */
public class Program {
    static void Main(string[] args) {
        UIApplication.Main(args, null, typeof(AppDelegate));
    }
}