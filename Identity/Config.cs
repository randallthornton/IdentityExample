using IdentityServer4.Models;

namespace Identity
{
    public static class Config
    {
        public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope>
        {
            new ApiScope("monolith", "My API")
        };

        public static IEnumerable<Client> Clients => new List<Client>
        {
            new Client
            {
                ClientId = "gateway",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("gatewaysecret".Sha256())
                },
                AllowedScopes =
                {
                    "monolith"
                }
            }
        };
    }
}
