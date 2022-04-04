using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Security.API.Infrastructure;
using Security.Api.Framework.Handlers;
using Microsoft.AspNetCore.Authentication;

namespace Security.Api
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
            services.AddControllers();

            services.AddDistributedMemoryCache();

            services.AddInfrastructureSecurity();

            //BASIC AUTH
            services.AddAuthentication(Security.Api.Framework.Consts.Constants.AUTH_BASIC_SCHEMA)
                .AddScheme<AuthenticationSchemeOptions, 
                BasicAuthenticationHandler>(Framework.Consts.Constants.AUTH_BASIC_SCHEMA, null);

            //OAuthBasic
            services.AddAuthentication(Framework.Consts.Constants.AUTH_BASIC_SCHEMA_OAUTH)
                .AddScheme<AuthenticationSchemeOptions, OAuthBasicAuthenticationHandler>(Framework.Consts.Constants.AUTH_BASIC_SCHEMA_OAUTH, null);
                   
            //Custom OAuth to suppor GUID -> WILL BE BETTER USING JWT TOKENS IN OAUTH2 STANDARS
            services.AddAuthentication(Framework.Consts.Constants.SCHEMA_OAUTH)
                .AddScheme<AuthenticationSchemeOptions, OAuthBearerAuthenticationHandler>(Framework.Consts.Constants.SCHEMA_OAUTH, null);

            services.AddAuthorization(op => {
                op.AddPolicy("RequiredAdminRolePO", confPO => {
                    confPO.RequireRole("ADMIN");
                    confPO.AddAuthenticationSchemes(Framework.Consts.Constants.SCHEMA_OAUTH);
                    confPO.RequireAuthenticatedUser();
                    // [Authorize(AuthenticationSchemes = Security.Api.Framework.Consts.Constants.SCHEMA_OAUTH)]
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
