using Microsoft.Identity.Client;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureB2CXamTraditional
{
    public static class App
    {
        public static string TenantId = $"viime.onmicrosoft.com";

        public static string Authority = $"https://login.windows.net/{TenantId}/";
        public static string ResourceId = "https://graph.windows.net";
        public static string ClientId = "42d59532-f239-4669-9c22-fa8243277139";
        public static string RedirectUri = "https://viime.azurewebsites.net/.auth/login/aad/callback";
    }
}
