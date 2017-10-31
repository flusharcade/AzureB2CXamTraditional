using Microsoft.Identity.Client;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureB2CXamTraditional
{
    public static class App
    {
        public static PublicClientApplication PCA = null;
        
        // Azure AD B2C Coordinates
        public static string Tenant = "viimee.onmicrosoft.com";
        public static string ClientID = "42d59532-f239-4669-9c22-fa8243277139";
        public static string PolicySignUpSignIn = "B2C_1_viime-signup-signon-policy";
        public static string PolicyEditProfile = "B2C_1_viime-signup-signon-policy";
        public static string PolicyResetPassword = "B2C_1_viime-password-reset-policy";

        public static string[] Scopes = { "https://viime.onmicrosoft.com/api" };
        public static string ApiEndpoint = "https://viime.azurewebsites.net/hello";

        public static string AuthorityBase = $"https://login.microsoftonline.com/tfp/{Tenant}/";
        public static string Authority = $"{AuthorityBase}{PolicySignUpSignIn}";
        public static string AuthorityEditProfile = $"{AuthorityBase}{PolicyEditProfile}";
        public static string AuthorityPasswordReset = $"{AuthorityBase}{PolicyResetPassword}";

        public static UIParent UiParent = null;

        static App()
        {
            // default redirectURI; each platform specific project will have to override it with its own
            PCA = new PublicClientApplication(ClientID, Authority);
            PCA.RedirectUri = $"msal{ClientID}://auth";
        }    
    }
}
