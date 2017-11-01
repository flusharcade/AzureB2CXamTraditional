using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Android.App;
using Android.Content.PM;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;

using Newtonsoft.Json.Linq;

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
        /// The button sign in sign out.
        /// </summary>
        private Button _btnSignInSignOut;

        /// <summary>
        /// The loading.
        /// </summary>
        private bool loading = false;

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

            _btnSignInSignOut = FindViewById<Button>(Resource.Id.signButton);
            _btnSignInSignOut.Touch += async (sender, e) =>
            {
                if (!loading)
                {
                    loading = true;

                    var title = _btnSignInSignOut.Text.ToLower();

                    if (title.Equals("sign in"))
                    {
                        var ar = await Authenticate(this, App.Authority, App.ResourceId, App.ClientId, App.RedirectUri);

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

                    loading = false;
                }
            };
        }

        /// <summary>
        /// Updates the user info.
        /// </summary>
        /// <param name="user">User.</param>
        public void UpdateUserInfo(JObject user)
        {
            if (user != null)
            {
                _lblName.Text = user["name"]?.ToString();
                _lblId.Text = user["oid"]?.ToString();
            }
            else {
                _lblName.Text = "";
                _lblId.Text = "";
            }

            UpdateSignInState(user != null);
        }

        /// <summary>
        /// Authenticate the specified context, authority, resource, clientId and returnUri.
        /// </summary>
        /// <returns>The authenticate.</returns>
        /// <param name="context">Context.</param>
        /// <param name="authority">Authority.</param>
        /// <param name="resource">Resource.</param>
        /// <param name="clientId">Client identifier.</param>
        /// <param name="returnUri">Return URI.</param>
        public async Task<AuthenticationResult> Authenticate(Activity context, string authority, string resource, string clientId, string returnUri)
        {
            var authContext = new AuthenticationContext(authority);
            if (authContext.TokenCache.ReadItems().Any())
                authContext = new AuthenticationContext(authContext.TokenCache.ReadItems().First().Authority);

            var uri = new Uri(returnUri);
            var platformParams = new PlatformParameters(context);
            try
            {
                var authResult = await authContext.AcquireTokenAsync(resource, clientId, uri, platformParams);
                return authResult;
            }
            catch (AdalException ex) {
                DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }

            return null;
        }

        /// <summary>
        /// Ons the activity result.
        /// </summary>
        /// <param name="requestCode">Request code.</param>
        /// <param name="resultCode">Result code.</param>
        /// <param name="data">Data.</param>
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationAgentContinuationHelper.SetAuthenticationAgentContinuationEventArgs(requestCode, resultCode, data);
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

        /// <summary>
        /// Updates the state of the sign in.
        /// </summary>
        /// <param name="isSignedIn">If set to <c>true</c> is signed in.</param>
        private void UpdateSignInState(bool isSignedIn)
        {
            _btnSignInSignOut.Text = isSignedIn ? "Sign out" : "Sign in";
        }

        /// <summary>
        /// Displaies the alert.
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="message">Message.</param>
        /// <param name="cancel">Cancel.</param>
        private void DisplayAlert(string title, string message, string cancel)
        {
            Toast.MakeText(this, message, ToastLength.Long).Show();
        }
    }
}

