using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.UWP.OAuth.Services
{
    public class GoogleAuthenticationProvider : OAuthAuthenticationProvider
    {
        public static readonly string[] BasicGoogleScopes = new string[]
        {
            "profile",
            "email"
        };

        public static readonly string[] GoogleClassroomScopes = new string[]
        {
            "https://www.googleapis.com/auth/classroom.courses.readonly",
            "https://www.googleapis.com/auth/classroom.announcements.readonly",
            "https://www.googleapis.com/auth/classroom.coursework.me"
        };

        public GoogleAuthenticationProvider(string clientId, string redirectUri, params string[][] scopes)
            : base(
                "https://accounts.google.com/o/oauth2/v2/auth",
                "https://www.googleapis.com/oauth2/v4/token",
                redirectUri,
                clientId,
                scopes.SelectMany(s => s).ToArray(),
                "code",
                new string[] { "iss", "email" },
                "https://www.googleapis.com/oauth2/v2/userinfo",
                "name",
                "email")
        { }
    }
}
