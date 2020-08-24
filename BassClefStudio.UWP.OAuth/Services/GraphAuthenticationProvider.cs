using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.UWP.OAuth.Services
{
    public class GraphAuthenticationProvider : OAuthAuthenticationProvider
    {
        public static readonly string[] GraphBasicScopes = new string[]
        {
            "openid",
            "profile"
        };

        public static readonly string[] MicrosoftToDoScopes = new string[]
        {
            "Tasks.ReadWrite",
            "Tasks.ReadWrite.Shared",
            "User.Read",
            "email",
            "offline_access"
        };

        public GraphAuthenticationProvider(string clientId, params string[][] scopes)
            : base(
                "https://login.microsoftonline.com/common/oauth2/v2.0/authorize",
                "https://login.microsoftonline.com/common/oauth2/v2.0/token",
                "https://login.microsoftonline.com/common/oauth2/nativeclient",
                clientId,
                scopes.SelectMany(s => s).ToArray(),
                "code",
                new string[] { "iss", "preferred_username" },
                "https://graph.microsoft.com/v1.0/me/",
                "displayName",
                "userPrincipalName")
        { }
    }
}
