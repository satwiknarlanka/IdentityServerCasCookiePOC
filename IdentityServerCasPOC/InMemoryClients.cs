using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityServerCasPOC
{
    public class InMemoryClients
    {
        public static IEnumerable<Client> Clients()
        {
            return new []
            {
                new Client
                {
                    ClientId = "MySpaHost",
                    ClientSecrets = new []
                    {
                        new Secret("secret".Sha256()),  
                    },
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    RequireConsent = false,
                    AllowAccessTokensViaBrowser = true,
                    AllowedScopes = new []
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "MyRestApi"
                    },
                    RedirectUris = new [] { "https://localhost:5003/signin-oidc" },
                    PostLogoutRedirectUris = new [] { "https://localhost:5003/signout-callback-oidc" },
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 5,
                    AccessTokenType = AccessTokenType.Reference,
                    AbsoluteRefreshTokenLifetime = 3600,
                    
                }, 
            };
        }
    }
}
