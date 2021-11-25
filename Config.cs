// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

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
                new ApiScope("manage")
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
                new Client
                {
                    ClientId = "postman",
                    ClientSecrets = { new Secret(Configuration["PostmanSecret"].Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        "enumerate",
                        "finance.read",
                        "mining.read",
                        "finance.write",
                        "mining.write",
                        "manage"
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
                        "manage"
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
                        "finance.write"
                    }
                }
            };
    }
}