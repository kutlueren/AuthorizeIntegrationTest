using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace AuthorizeIntegrationTest
{
    public static class IdentityServerConfiguration
    {
        public const string ApiName = "api1";
        public const string ApiSecret = "secret";
        public const string Authority = "http://localhost/";
        public const string Client = "client";

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static List<ApiResource> InitApiResources(string apiName, string apiSecret)
        {
            return new List<ApiResource>
            {
                new ApiResource(apiName, "My API") {ApiSecrets = {new Secret(apiSecret.Sha256())}}
            };
        }

        // Default client
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = Client,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret(ApiSecret.Sha256())
                    },
                    AllowedScopes = {
                        ApiName,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                    },
                    AllowOfflineAccess = true
                }
            };
        }

        // Default user
        public static  List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "user",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("name", "User"),
                        new Claim("website", "https://user.com")
                    }
                }
            };
        }
    }
}
