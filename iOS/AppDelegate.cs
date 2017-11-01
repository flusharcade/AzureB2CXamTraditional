using Foundation;
using UIKit;

using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AzureB2CXamTraditional.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        /// <summary>
        /// The window.
        /// </summary>
        private UIWindow _window;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            _window = new UIWindow(UIScreen.MainScreen.Bounds);
            _window.RootViewController = new LoginViewController();
            _window.MakeKeyAndVisible();

            return true;
        }
    }
}
