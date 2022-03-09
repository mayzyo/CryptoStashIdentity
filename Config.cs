using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace CryptoStashIdentity
{
    public class Config
    {
        IConfiguration Configuration { get; }

        public Config(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("enumerate"),
                new ApiScope("finance.read"),
                new ApiScope("mining.read"),
                new ApiScope("finance.write"),
                new ApiScope("mining.write"),
                new ApiScope("manage"),
                new ApiScope(IdentityServerConstants.LocalApi.ScopeName)
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("urn:finance", "Finance API")
                {
                    Scopes = { "finance.read", "finance.write", "manage", "enumerate" }
                },

                new ApiResource("urn:mining", "Mining API")
                {
                    Scopes = { "mining.read", "mining.write", "manage", "enumerate" },
                    //RequireResourceIndicator = true
                }
            };

        public IEnumerable<Client> Clients =>
            new Client[]
            {
                // Remove this in production
                new Client
                {
                    ClientId = "development",
                    ClientSecrets = { new Secret(Configuration["DevelopmentSecret"].Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        "enumerate",
                        "finance.read",
                        "mining.read",
                        "finance.write",
                        "mining.write",
                        "manage",
                        IdentityServerConstants.LocalApi.ScopeName
                    }
                },
                new Client
                {
                    ClientId = "cryptostashnotification",
                    ClientSecrets = { new Secret(Configuration["CryptoStashNotificationSecret"].Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        "mining.write"
                    }
                },
                new Client
                {
                    ClientId = "cryptostashconnect",
                    ClientSecrets = { new Secret(Configuration["CryptoStashConnectSecret"].Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        "enumerate",
                        "mining.read",
                        "finance.read",
                        "mining.write",
                        "finance.write",
                        "manage",
                        IdentityServerConstants.LocalApi.ScopeName
                    }
                },
                new Client
                {
                    ClientId = "cryptostashclient",
                    ClientName = "Crypto Stash Client",
                    RequireClientSecret = false,

                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { "com.cryptostashclient.auth:/oauth" },

                    AllowOfflineAccess = true,
                    RequirePkce = true,

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "enumerate",
                        "mining.read",
                        "finance.read",
                        "mining.write",
                        "finance.write",
                        IdentityServerConstants.LocalApi.ScopeName
                    }
                }
            };
    }
}