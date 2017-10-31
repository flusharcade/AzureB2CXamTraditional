using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Identity.Client;

using UIKit;

using Newtonsoft.Json.Linq;

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
        /// The lbl API title.
        /// </summary>
        private UILabel _lblApiTitle;

        /// <summary>
        /// The lbl API.
        /// </summary>
        private UILabel _lblApi;

        /// <summary>
        /// The button edit profile.
        /// </summary>
        private UIButton _btnEditProfile;

        /// <summary>
        /// The button call API.
        /// </summary>
        private UIButton _btnCallApi;

        /// <summary>
        /// The button sign in sign out.
        /// </summary>
        private UIButton _btnSignInSignOut;

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

            _lblApiTitle = new UILabel()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextAlignment = UITextAlignment.Center,
                Text = "API"
            };

            _lblApi = new UILabel()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextAlignment = UITextAlignment.Center,
            };

            _btnEditProfile = new UIButton()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            _btnEditProfile.TouchUpInside += async (sender, e) =>
            {
                await OnEditProfile();
            };
            _btnEditProfile.SetTitle("Edit", UIControlState.Normal);
            _btnEditProfile.SetTitleColor(UIColor.Black, UIControlState.Normal);

            _btnCallApi = new UIButton()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            _btnCallApi.TouchUpInside += async (sender, e) =>
            {
                await OnCallApi();
            };
            _btnCallApi.SetTitle("Call", UIControlState.Normal);
            _btnCallApi.SetTitleColor(UIColor.Black, UIControlState.Normal);

            _btnSignInSignOut = new UIButton()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            _btnSignInSignOut.TouchUpInside += async (sender, e) => 
            {
                await OnSignInSignOut();
            };
            _btnSignInSignOut.SetTitle("Sign In/Out", UIControlState.Normal);
            _btnSignInSignOut.SetTitleColor(UIColor.Black, UIControlState.Normal);

            // labels
            mainView.Add(_lblNameTitle);
            mainView.Add(_lblName);
            mainView.Add(_lblIdTitle);
            mainView.Add(_lblId);
            mainView.Add(_lblApiTitle);
            mainView.Add(_lblApi);

            // buttons
            mainView.Add(_btnEditProfile);
            mainView.Add(_btnCallApi);
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
                {"lblApiTitle", _lblApiTitle},
                {"lblApi", _lblApi},
                {"btnEditProfile", _btnEditProfile},
                {"btnCallApi", _btnCallApi},
                {"btnSignInSignOut", _btnSignInSignOut}
            };

            View.AddConstraints(
                NSLayoutConstraint.FromVisualFormat("V:|[mainView]|", NSLayoutFormatOptions.DirectionLeftToRight, null, views)
                .Concat(NSLayoutConstraint.FromVisualFormat("H:|[mainView]|", NSLayoutFormatOptions.AlignAllTop, null, views))
                .ToArray());

            mainView.AddConstraints(
                NSLayoutConstraint.FromVisualFormat("V:|-140-[lblNameTitle]-40-[lblIdTitle]-40-[lblApiTitle]-40-[btnEditProfile(60)]-40-[btnCallApi(btnEditProfile)]-[btnSignInSignOut(btnEditProfile)]", NSLayoutFormatOptions.DirectionLeftToRight, null, mainViews)
                .Concat(NSLayoutConstraint.FromVisualFormat("V:|-140-[lblName]-40-[lblId]-40-[lblApi]-40-[btnEditProfile(60)]-40-[btnCallApi(btnEditProfile)]-[btnSignInSignOut(btnEditProfile)]", NSLayoutFormatOptions.DirectionLeftToRight, null, mainViews))
                .Concat(NSLayoutConstraint.FromVisualFormat("H:|[lblNameTitle][lblName(lblNameTitle)]|", NSLayoutFormatOptions.AlignAllTop, null, mainViews))
                .Concat(NSLayoutConstraint.FromVisualFormat("H:|[lblIdTitle][lblId(lblIdTitle)]|", NSLayoutFormatOptions.AlignAllTop, null, mainViews))
                .Concat(NSLayoutConstraint.FromVisualFormat("H:|[lblApiTitle][lblApi(lblApiTitle)]|", NSLayoutFormatOptions.AlignAllTop, null, mainViews))
                .Concat(NSLayoutConstraint.FromVisualFormat("H:[btnEditProfile(300)]", NSLayoutFormatOptions.AlignAllTop, null, mainViews))
                .Concat(NSLayoutConstraint.FromVisualFormat("H:[btnCallApi(300)]", NSLayoutFormatOptions.AlignAllTop, null, mainViews))
                .Concat(NSLayoutConstraint.FromVisualFormat("H:[btnSignInSignOut(300)]", NSLayoutFormatOptions.AlignAllTop, null, mainViews))
                .Concat(new[] { NSLayoutConstraint.Create(_btnEditProfile, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, mainView, NSLayoutAttribute.CenterX, 1f, 0) })
                .Concat(new[] { NSLayoutConstraint.Create(_btnCallApi, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, mainView, NSLayoutAttribute.CenterX, 1f, 0) })
                .Concat(new[] { NSLayoutConstraint.Create(_btnSignInSignOut, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, mainView, NSLayoutAttribute.CenterX, 1f, 0) })
                .ToArray());
        }

        /// <summary>
        /// Views the did appear.
        /// </summary>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        public override async void ViewDidAppear(bool animated)
        {
            UpdateSignInState(false);

            // Check to see if we have a User
            // in the cache already.
            try
            {
                AuthenticationResult ar = await App.PCA.AcquireTokenSilentAsync(App.Scopes, AuthHandler.GetUserByPolicy(App.PCA.Users, App.PolicySignUpSignIn), App.Authority, false);
                UpdateUserInfo(AuthHandler.RetrieveUserInfo(ar));
                UpdateSignInState(true);
            }
            catch (Exception ex)
            {
                // Uncomment for debugging purposes
                DisplayAlert($"Exception:", ex.ToString(), "Dismiss");

                // Doesn't matter, we go in interactive mode
                UpdateSignInState(false);
            }
        }

        async Task OnSignInSignOut()
        {
            try
            {
                if (_btnSignInSignOut.TitleLabel.Text == "Sign in")
                {
                    var ar = await AuthHandler.OnSignInSignOut(true);
                    UpdateUserInfo(AuthHandler.RetrieveUserInfo(ar));
                    UpdateSignInState(true);
                }
                else
                {
                    foreach (var user in App.PCA.Users)
                    {
                        App.PCA.Remove(user);
                    }
                    UpdateSignInState(false);
                }
            }
            catch (Exception ex)
            {
                // Checking the exception message 
                // should ONLY be done for B2C
                // reset and not any other error.
                if (ex.Message.Contains("AADB2C90118"))
                    OnPasswordReset();
                // Alert if any exception excludig user cancelling sign-in dialog
                //else if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                //    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }

        public void UpdateUserInfo(JObject user)
        {
            _lblName.Text = user["name"]?.ToString();
            _lblId.Text = user["oid"]?.ToString();
        }

        async Task OnCallApi()
        {
            try
            {
                _lblApi.Text = $"Calling API {App.ApiEndpoint}";
                AuthenticationResult ar = await App.PCA.AcquireTokenSilentAsync(App.Scopes, AuthHandler.GetUserByPolicy(App.PCA.Users, App.PolicySignUpSignIn), App.Authority, false);
                string token = ar.AccessToken;

                // Get data from API
                var client = new HttpClient();
                var message = new HttpRequestMessage(HttpMethod.Get, App.ApiEndpoint);
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.SendAsync(message);
                string responseString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    _lblApi.Text = $"Response from API {App.ApiEndpoint} | {responseString}";
                }
                else
                {
                    _lblApi.Text = $"Error calling API {App.ApiEndpoint} | {responseString}";
                }
            }
            catch (MsalUiRequiredException ex)
            {
                DisplayAlert($"Session has expired, please sign out and back in.", ex.ToString(), "Dismiss");
            }
            catch (Exception ex)
            {
                DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }

        async Task OnEditProfile()
        {
            try
            {
                // KNOWN ISSUE:
                // User will get prompted 
                // to pick an IdP again.
                var ar = await AuthHandler.OnEditProfile();
                UpdateUserInfo(AuthHandler.RetrieveUserInfo(ar));
            }
            catch (Exception ex)
            {
                // Alert if any exception excludig user cancelling sign-in dialog
                if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                    DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }

        /// <summary>
        /// Ons the password reset.
        /// </summary>
        /// <returns>The password reset.</returns>
        async Task OnPasswordReset()
        {
            try
            {
                var ar = await AuthHandler.OnPasswordReset();
                UpdateUserInfo(AuthHandler.RetrieveUserInfo(ar));
            }
            catch (Exception ex)
            {
                // Alert if any exception excludig user cancelling sign-in dialog
                if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                    DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }

        void UpdateSignInState(bool isSignedIn)
        {
            _btnSignInSignOut.SetTitle(isSignedIn ? "Sign out" : "Sign in", UIControlState.Normal);
            _btnEditProfile.Hidden = !isSignedIn;
            _btnCallApi.Hidden = !isSignedIn;
            //slUser.IsVisible = isSignedIn;
            _lblApi.Text = "";
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