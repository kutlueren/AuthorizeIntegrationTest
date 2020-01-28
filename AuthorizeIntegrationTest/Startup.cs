using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AuthorizeIntegrationTest
{
    public class Startup
    {


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ////services.AddControllers();

            ConfigureIdentityServer(services);

            AddAuthentication(services);

            AddMvc(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthentication();

            AddIdentityServer(app);

            app.UseMvc(routes =>
            {
                routes
                .MapRoute(name: "Default", template: "{controller=Default}/{action=Index}")
                .MapRoute(name: "Public", template: "{*url}", defaults: new { controller = "Default", action = "Index" });
            });
        }

        protected virtual void AddIdentityServer(IApplicationBuilder app)
        {
            app.UseIdentityServer();
        }

        protected virtual void ConfigureIdentityServer(IServiceCollection services)
        {
            services.AddIdentityServer()
                .AddDeveloperSigningCredential(false)
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(IdentityServerConfiguration.GetIdentityResources())
                .AddInMemoryApiResources(IdentityServerConfiguration.InitApiResources(IdentityServerConfiguration.ApiName, IdentityServerConfiguration.ApiSecret))
                .AddInMemoryClients(IdentityServerConfiguration.GetClients())
                .AddTestUsers(IdentityServerConfiguration.GetUsers());
        }

        protected virtual void AddAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = IdentityServerConfiguration.Authority;
                    options.RequireHttpsMetadata = false;
                    options.ApiName = IdentityServerConfiguration.ApiName;
                    options.SupportedTokens = SupportedTokens.Jwt;
                    options.JwtValidationClockSkew = TimeSpan.FromTicks(TimeSpan.TicksPerMinute);
                });
        }

        protected virtual void AddMvc(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }
    }
}
