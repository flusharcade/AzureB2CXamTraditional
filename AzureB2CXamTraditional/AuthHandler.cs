using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Identity.Client;

using Newtonsoft.Json.Linq;

namespace AzureB2CXamTraditional
{
    /// <summary>
    /// Auth handler.
    /// </summary>
    public static class AuthHandler
    {
        /// <summary>
        /// Ons the sign in sign out.
        /// </summary>
        /// <returns>The sign in sign out.</returns>
        /// <param name="isSignIn">If set to <c>true</c> is sign in.</param>
        public static async Task<AuthenticationResult> OnSignInSignOut(bool isSignIn)
        {
            try
            {
                if (isSignIn)
                {
                    AuthenticationResult ar = await App.PCA.AcquireTokenAsync(App.Scopes, GetUserByPolicy(App.PCA.Users, App.PolicySignUpSignIn), App.UiParent);
                    return ar;
                }
                else
                {
                    foreach (var user in App.PCA.Users)
                    {
                        App.PCA.Remove(user);
                    }
                }
            }
            catch (Exception ex)
            {
                var t = 0;
            }

            return null;
        }

        /// <summary>
        /// Ons the edit profile.
        /// </summary>
        /// <returns>The edit profile.</returns>
        public static async Task<AuthenticationResult> OnEditProfile()
        {
            try
            {
                // KNOWN ISSUE:
                // User will get prompted 
                // to pick an IdP again.
                AuthenticationResult ar = await App.PCA.AcquireTokenAsync(App.Scopes, AuthHandler.GetUserByPolicy(App.PCA.Users, App.PolicyEditProfile), UIBehavior.SelectAccount, string.Empty, null, App.AuthorityEditProfile, App.UiParent);
                return ar;
            }
            catch (Exception ex)
            {
                // Alert if any exception excludig user cancelling sign-in dialog
                if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                {
                    //DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
                }
            }

            return null;
        }

        /// <summary>
        /// Ons the password reset.
        /// </summary>
        /// <returns>The password reset.</returns>
        public static async Task<AuthenticationResult> OnPasswordReset()
        {
            try
            {
                AuthenticationResult ar = await App.PCA.AcquireTokenAsync(App.Scopes, (IUser)null, UIBehavior.SelectAccount, string.Empty, null, App.AuthorityPasswordReset, App.UiParent);
                return ar;
            }
            catch (Exception ex)
            {
                // Alert if any exception excludig user cancelling sign-in dialog
                if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                {
                    //DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
                }
            }

            return null;
        }


        /// <summary>
        /// Gets the user by policy.
        /// </summary>
        /// <returns>The user by policy.</returns>
        /// <param name="users">Users.</param>
        /// <param name="policy">Policy.</param>
        public static IUser GetUserByPolicy(IEnumerable<IUser> users, string policy)
        {
            foreach (var user in users)
            {
                string userIdentifier = Base64UrlDecode(user.Identifier.Split('.')[0]);

                if (userIdentifier.EndsWith(policy.ToLower()))
                    return user;
            }

            return null;
        }

        /// <summary>
        /// Updates the user info.
        /// </summary>
        /// <param name="ar">Ar.</param>
        public static JObject RetrieveUserInfo(AuthenticationResult ar)
        {
            return ParseIdToken(ar.IdToken);
        }

        /// <summary>
        /// Base64s the URL decode.
        /// </summary>
        /// <returns>The URL decode.</returns>
        /// <param name="s">S.</param>
        private static string Base64UrlDecode(string s)
        {
            s = s.Replace('-', '+').Replace('_', '/');
            s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
            var byteArray = Convert.FromBase64String(s);
            var decoded = System.Text.Encoding.UTF8.GetString(byteArray, 0, byteArray.Count());
            return decoded;
        }

        /// <summary>
        /// Parses the identifier token.
        /// </summary>
        /// <returns>The identifier token.</returns>
        /// <param name="idToken">Identifier token.</param>
        private static JObject ParseIdToken(string idToken)
        {
            // Get the piece with actual user info
            idToken = idToken.Split('.')[1];
            idToken = Base64UrlDecode(idToken);
            return JObject.Parse(idToken);
        }
    }
}
