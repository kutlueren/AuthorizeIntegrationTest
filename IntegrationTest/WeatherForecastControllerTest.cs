using AuthorizeIntegrationTest;
using IdentityModel.Client;
using IdentityServer4.Contrib.AspNetCore.Testing.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTest
{
    public class WeatherForecastControllerTest
    {
        private readonly HttpClient client;
        private readonly HttpClient identityClient;
        private const string TokenEndpoint = "http://localhost/connect/token";
        private IdentityServerProxy identityServerProxy;

        public WeatherForecastControllerTest()
        {
            var identityServerWebHostBuilder = new WebHostBuilder()
                    .UseStartup<Startup>()
                    .UseDefaultServiceProvider(x => x.ValidateScopes = false)
                    .UseKestrel();

            identityServerProxy = new IdentityServerProxy(identityServerWebHostBuilder);

            var server = new TestServer(new WebHostBuilder()
                    .UseStartup<TestStartup>()
                    .ConfigureServices(
                        services =>
                        {
                            services
                            .AddSingleton(identityServerProxy.IdentityServer.CreateHandler());
                        })
                    .UseDefaultServiceProvider(x => x.ValidateScopes = false)
                    .UseKestrel());


            client = server.CreateClient();

            identityClient = identityServerProxy.IdentityServer.CreateClient();
        }

        [Fact]
        public async Task GetForecasts_Async()
        {
            client.SetBearerToken(await GetToken());

            HttpResponseMessage response = await client.GetAsync("api/weatherforecast");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private async Task<string> GetToken()
        {
            var response = await identityClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = TokenEndpoint,
                ClientId = IdentityServerConfiguration.Client,
                ClientSecret = IdentityServerConfiguration.ApiSecret,
                Scope = "api1",
            });

            return response.AccessToken;
        }
    }
}
