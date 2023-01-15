using IdentityServer4;
using IdentityServer4.Models;

namespace Identity
{
    public static class Config
    {
        public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope>
        {
            new ApiScope("monolith", "My API"),
        };

        public static IEnumerable<Client> Clients => new List<Client>
        {
            new Client
            {
                ClientId = "gateway",
                ClientSecrets =
                {
                    new Secret("gatewaysecret".Sha256())
                },
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris =
                {
                    "https://localhost:7190/signin-oidc"
                },
                PostLogoutRedirectUris = {
                    "https://localhost:7190/signout-callback-oidc"
                },
                AllowedScopes =
                {
                    "monolith",
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile
                },
            }
        };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
    }
}
