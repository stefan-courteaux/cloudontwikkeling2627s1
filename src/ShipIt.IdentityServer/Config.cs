using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;

namespace ShipIt.IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource( 
                name: "roles",
                userClaims: new[] { JwtClaimTypes.Role }
            )
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("shipit.pricequotes.api.read"),
            new ApiScope("shipit.pricequotes.api.write")
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
                AllowedScopes = { "shipit.pricequotes.api.write", "shipit.pricequotes.api.read" } 
            },
            new Client{ 
                ClientId = "label-api-client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = {new Secret("eenLabelGeheim".Sha256())},
                AllowedScopes = { "shipit.pricequotes.api.read" } 
            },
            new Client {
                ClientId = "react-spa-client",
                ClientSecrets = {new Secret("react-secret".Sha256())},
                AllowedGrantTypes = GrantTypes.Code,
                AllowedScopes = {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "shipit.pricequotes.api.read",
                    "roles"
                },
                RedirectUris = { "http://localhost:5173/" }, 
                PostLogoutRedirectUris = { "http://localhost:5173/" }, 
                AllowedCorsOrigins = ["http://localhost:5173"], 
            }                        
        };
}
