using BassClefStudio.NET.Api.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.UWP.OAuth.Credentials
{
    public class CredentialLockerService : ICredentialService<OAuthAccount>
    {
        const string CredentialResource = "OAuth";

        public void SaveCredentials(OAuthAccount account)
        {
            var vault = new Windows.Security.Credentials.PasswordVault();

            var credentials = vault.RetrieveAll();
            if (credentials.Any(c => c.Resource == CredentialResource && c.UserName == account.UserId))
            {
                vault.Remove(vault.Retrieve(CredentialResource, account.UserId));
            }
            vault.Add(new Windows.Security.Credentials.PasswordCredential(CredentialResource, account.UserId, account.RefreshToken));
        }

        public bool RefreshCredentials(OAuthAccount account)
        {
            var vault = new Windows.Security.Credentials.PasswordVault();

            var credentials = vault.RetrieveAll();
            if (credentials.Any(c => c.Resource == CredentialResource && c.UserName == account.UserId))
            {
                var credential = vault.Retrieve(CredentialResource, account.UserId);
                credential.RetrievePassword();
                account.RefreshToken = credential.Password;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveCredentials(OAuthAccount account)
        {
            var vault = new Windows.Security.Credentials.PasswordVault();

            var credentials = vault.RetrieveAll();
            if (credentials.Any(c => c.Resource == CredentialResource && c.UserName == account.UserId))
            {
                var credential = vault.Retrieve(CredentialResource, account.UserId);
                vault.Remove(credential);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
