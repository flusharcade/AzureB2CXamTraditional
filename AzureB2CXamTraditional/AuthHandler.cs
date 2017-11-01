using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AzureB2CXamTraditional
{
    /// <summary>
    /// Auth handler.
    /// </summary>
    public static class AuthHandler
    {
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
