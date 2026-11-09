using Duende.IdentityServer.Models;

namespace ShipIt.IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("shipit.pricequotes.api")
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client{
                ClientId = "postman-client", 
                AllowedGrantTypes = GrantTypes.ClientCredentials, 
                ClientSecrets = {
                    new Secret("eenGrootGeheim".Sha256())
                },
                AllowedScopes = { "shipit.pricequotes.api" } 
            }
        };
}
