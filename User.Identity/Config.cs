using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace User.Identity
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetResource()
        {
            return new List<ApiResource>()
            {
                new ApiResource("gateway_api","gateway api service"),
                new ApiResource("contact_api","contact api service"),
                new ApiResource("user_api","user api service"),
                new ApiResource("project_api","project api service")
            };
        }
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>()
            {
                new Client
                {
                    ClientId = "android",
                    ClientSecrets = new[] {new Secret("secret".Sha256())},
                    RefreshTokenExpiration=TokenExpiration.Sliding,
                    AllowedGrantTypes =new List<string>(){"sms_auth_code"},
                    AlwaysIncludeUserClaimsInIdToken = true,           
                    AllowOfflineAccess = true,
                    AllowedScopes = new[]
                    {
                        "gateway_api",
                        "contact_api",
                        "user_api",
                        "project_api",
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                      
                    }
                },

            };
        }
    
        public static IEnumerable<IdentityResource> GetiIdentityResources()
        {
            return new List<IdentityResource>()
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),              
            };
        }
    }
}