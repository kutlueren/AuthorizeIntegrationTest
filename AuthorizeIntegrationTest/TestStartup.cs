using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace AuthorizeIntegrationTest
{
    public class TestStartup : Startup
    {
        private readonly HttpMessageHandler httpMessageHandler;

        public TestStartup(IConfiguration configuration, HttpMessageHandler httpMessageHandler) : base(configuration)
        {
            this.httpMessageHandler = httpMessageHandler;
        }

        protected override void AddIdentityServer(IApplicationBuilder app)
        {
            // NOTE: Has to be empty. This server is not going to host an identity server.
        }

        protected override void ConfigureIdentityServer(IServiceCollection services)
        {
            // NOTE: Has to be empty. This server is not going to host an identity server.
        }

        protected override void AddAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = IdentityServerConfiguration.Authority;
                    options.RequireHttpsMetadata = false;
                    options.JwtBackChannelHandler = httpMessageHandler;
                });
        }
    }
}
