using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content.PM;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;

using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AzureB2CXamTraditional.Droid
{
    [Activity(Label = "AzureB2CXamTraditional", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Activity
    {
        /// <summary>
        /// The name of the lbl.
        /// </summary>
        private TextView _lblName;

        /// <summary>
        /// The lbl identifier.
        /// </summary>
        private TextView _lblId;

        /// <summary>
        /// The lbl API.
        /// </summary>
        private TextView _lblApi;

        /// <summary>
        /// The button edit profile.
        /// </summary>
        private Button _btnEditProfile;

        /// <summary>
        /// The button call API.
        /// </summary>
        private Button _btnCallApi;

        /// <summary>
        /// The button sign in sign out.
        /// </summary>
        private Button _btnSignInSignOut;

        /// <summary>
        /// Ons the create.
        /// </summary>
        /// <param name="bundle">Bundle.</param>
        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            _lblName = FindViewById<TextView>(Resource.Id.nameTextView);
            _lblId = FindViewById<TextView>(Resource.Id.idTextView);
            _lblApi = FindViewById<TextView>(Resource.Id.apiTextView);

            _btnEditProfile = FindViewById<Button>(Resource.Id.editButton);
            _btnEditProfile.Touch += async (sender, e) =>
            {
            };

            _btnCallApi = FindViewById<Button>(Resource.Id.callButton);
            _btnCallApi.Touch += async (sender, e) =>
            {
            };

            _btnSignInSignOut = FindViewById<Button>(Resource.Id.signButton);
            _btnSignInSignOut.Touch += async (sender, e) =>
            {
                await AuthenticateAsync();
            };

            //App.UiParent = new UIParent(this as Activity);

            UpdateSignInState(false);

            // Check to see if we have a User
            // in the cache already.
            try
            {
                //AuthenticationResult ar = await App.PCA.AcquireTokenSilentAsync(App.Scopes, AuthHandler.GetUserByPolicy(App.PCA.Users, App.PolicySignUpSignIn), App.Authority, false);
                //UpdateUserInfo(AuthHandler.RetrieveUserInfo(ar));
                //UpdateSignInState(true);
            }
            catch (Exception ex)
            {
                // Uncomment for debugging purposes
                DisplayAlert($"Exception:", ex.ToString(), "Dismiss");

                // Doesn't matter, we go in interactive mode
                UpdateSignInState(false);
            }
        }

        /// <summary>
        /// Updates the user info.
        /// </summary>
        /// <param name="ar">Ar.</param>
        public void UpdateUserInfo(JObject user)
        {
            _lblName.Text = user["name"]?.ToString();
            _lblId.Text = user["oid"]?.ToString();
        }

        private MobileServiceClient client;
        private MobileServiceUser user;
        private async Task AuthenticateAsync()
        {

            string authority = "https://login.microsoftonline.com/tfp/viimee.onmicrosoft.com";
            string resourceId = "42d59532-f239-4669-9c22-fa8243277139";
            string clientId = "42d59532-f239-4669-9c22-fa8243277139";
            string redirectUri = "https://viime.azurewebsites.net";

            try
            {
                AuthenticationContext ac = new AuthenticationContext(authority);
                var ar = await ac.AcquireTokenAsync(resourceId, clientId,
                    new Uri(redirectUri), new PlatformParameters(this));
                JObject payload = new JObject();
                payload["access_token"] = ar.AccessToken;
                user = await client.LoginAsync(
                    MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory, payload);
            }
            catch (Exception ex)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetMessage(ex.Message);
                builder.SetTitle("You must log in. Login Required");
                builder.Create().Show();
            }
        }

        async Task OnSignInSignOut()
        {
            try
            {
                if (_btnSignInSignOut.Text.ToLower() == "sign in")
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

                // Alert if any exception excludig user cancelling sign-in dialog
                //else if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                //    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }

        void UpdateSignInState(bool isSignedIn)
        {
            _btnSignInSignOut.Text = isSignedIn ? "Sign out" : "Sign in";
            _btnEditProfile.Visibility = isSignedIn ? ViewStates.Visible : ViewStates.Invisible;
            _btnCallApi.Visibility = isSignedIn ? ViewStates.Visible : ViewStates.Invisible;
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
            Toast.MakeText(this, message, ToastLength.Long).Show();
        }
    }
}

