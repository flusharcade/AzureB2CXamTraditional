using System;
using System.Linq;
using System.Threading.Tasks;

using UIKit;

using Newtonsoft.Json.Linq;

using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AzureB2CXamTraditional.iOS
{
    /// <summary>
    /// Login view controller.
    /// </summary>
    public class LoginViewController : UIViewController
    {
        /// <summary>
        /// The lbl name title.
        /// </summary>
        private UILabel _lblNameTitle;

        /// <summary>
        /// The name of the lbl.
        /// </summary>
        private UILabel _lblName;

        /// <summary>
        /// The lbl identifier title.
        /// </summary>
        private UILabel _lblIdTitle;

        /// <summary>
        /// The lbl identifier.
        /// </summary>
        private UILabel _lblId;

        /// <summary>
        /// The button sign in sign out.
        /// </summary>
        private UIButton _btnSignInSignOut;

        /// <summary>
        /// The loading.
        /// </summary>
        private bool _loading;

        /// <summary>
        /// Views the did load.
        /// </summary>
        public override void ViewDidLoad()
        {
            View.BackgroundColor = UIColor.White;

            Title = "Login";

            var mainView = new UIView()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };

            Add(mainView);

            _lblNameTitle = new UILabel()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextAlignment = UITextAlignment.Center,
                Text = "Name"
            };

            _lblName = new UILabel()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextAlignment = UITextAlignment.Center,
            };

            _lblIdTitle = new UILabel()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextAlignment = UITextAlignment.Center,
                Text = "Id"
            };

            _lblId = new UILabel()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextAlignment = UITextAlignment.Center,
            };

            _btnSignInSignOut = new UIButton()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            _btnSignInSignOut.TouchUpInside += async (sender, e) => 
            {
                if (!_loading)
                {
                    _loading = true;

                    var title = _btnSignInSignOut.TitleLabel.Text.ToLower();

                    if (title.Equals("sign in"))
                    {
                        var ar = await Authenticate(App.Authority, App.ResourceId, App.ClientId, App.RedirectUri);

                        if (ar != null)
                        {
                            var user = AuthHandler.RetrieveUserInfo(ar);
                            UpdateUserInfo(user);
                        }
                    }
                    else
                    {
                        await DeAuthenticate(App.Authority);
                        UpdateUserInfo(null);
                    }

                    _loading = false;
                }
            };
            _btnSignInSignOut.SetTitle("Sign in", UIControlState.Normal);
            _btnSignInSignOut.SetTitleColor(UIColor.Black, UIControlState.Normal);

            // labels
            mainView.Add(_lblNameTitle);
            mainView.Add(_lblName);
            mainView.Add(_lblIdTitle);
            mainView.Add(_lblId);

            // buttons
            mainView.Add(_btnSignInSignOut);

            var views = new DictionaryViews()
            {
                {"mainView", mainView},
            };

            var mainViews = new DictionaryViews()
            {
                {"lblNameTitle", _lblNameTitle},
                {"lblName", _lblName},
                {"lblIdTitle", _lblIdTitle},
                {"lblId", _lblId},
                {"btnSignInSignOut", _btnSignInSignOut}
            };

            View.AddConstraints(
                NSLayoutConstraint.FromVisualFormat("V:|[mainView]|", NSLayoutFormatOptions.DirectionLeftToRight, null, views)
                .Concat(NSLayoutConstraint.FromVisualFormat("H:|[mainView]|", NSLayoutFormatOptions.AlignAllTop, null, views))
                .ToArray());

            mainView.AddConstraints(
                NSLayoutConstraint.FromVisualFormat("V:|-140-[lblNameTitle]-40-[lblIdTitle]-40-[btnSignInSignOut(60)]", NSLayoutFormatOptions.DirectionLeftToRight, null, mainViews)
                .Concat(NSLayoutConstraint.FromVisualFormat("V:|-140-[lblName]-40-[lblId]-40-[btnSignInSignOut(60)]", NSLayoutFormatOptions.DirectionLeftToRight, null, mainViews))
                .Concat(NSLayoutConstraint.FromVisualFormat("H:|[lblNameTitle][lblName(lblNameTitle)]|", NSLayoutFormatOptions.AlignAllTop, null, mainViews))
                .Concat(NSLayoutConstraint.FromVisualFormat("H:|[lblIdTitle][lblId(lblIdTitle)]|", NSLayoutFormatOptions.AlignAllTop, null, mainViews))
                .Concat(NSLayoutConstraint.FromVisualFormat("H:[btnSignInSignOut(300)]", NSLayoutFormatOptions.AlignAllTop, null, mainViews))
                .Concat(new[] { NSLayoutConstraint.Create(_btnSignInSignOut, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, mainView, NSLayoutAttribute.CenterX, 1f, 0) })
                .ToArray());
        }

        /// <summary>
        /// Authenticate the specified authority, resource, clientId and returnUri.
        /// </summary>
        /// <returns>The authenticate.</returns>
        /// <param name="authority">Authority.</param>
        /// <param name="resource">Resource.</param>
        /// <param name="clientId">Client identifier.</param>
        /// <param name="returnUri">Return URI.</param>
        public async Task<AuthenticationResult> Authenticate(string authority, string resource, string clientId, string returnUri)
        {
            var authContext = new AuthenticationContext(authority);

            if (authContext.TokenCache.ReadItems().Any())
                authContext = new AuthenticationContext(authContext.TokenCache.ReadItems().First().Authority);

            var topController = UIApplication.SharedApplication.KeyWindow.RootViewController;

            // ensures that the currently presented viewcontroller is acquired, even a modally presented one
            while (topController.PresentedViewController != null)
            {
                topController = topController.PresentedViewController;
            }

            var platformParams = new PlatformParameters(topController);

            var authResult = await authContext.AcquireTokenAsync(resource, clientId, new Uri(returnUri), platformParams);

            return authResult;
        }


        /// <summary>
        /// Des the authenticate.
        /// </summary>
        /// <returns>The authenticate.</returns>
        /// <param name="authority">Authority.</param>
        public async Task DeAuthenticate(string authority)
        {
            try
            {
                var authContext = new AuthenticationContext(authority);
                await Task.Factory.StartNew(() => {
                    authContext.TokenCache.Clear();
                });
            }
            catch (Exception ex)
            {
                DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }

        public void UpdateUserInfo(JObject user)
        {
            _lblName.Text = user["name"]?.ToString();
            _lblId.Text = user["oid"]?.ToString();
        }

        void UpdateSignInState(bool isSignedIn)
        {
            _btnSignInSignOut.SetTitle(isSignedIn ? "Sign out" : "Sign in", UIControlState.Normal);
        }

        /// <summary>
        /// Displaies the alert.
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="message">Message.</param>
        /// <param name="cancel">Cancel.</param>
        void DisplayAlert(string title, string message, string cancel)
        {
            UIAlertView alert = new UIAlertView()
            {
                Title = title,
                Message = message,
            };
            alert.AddButton(cancel);
            alert.Show();
        }
    }
}