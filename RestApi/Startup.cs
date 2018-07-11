using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RestApi
{
    public class Startup
    {
        private ILogger _logger;
        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddCors(
                options =>
                {
                    //options.AddPolicy("AllowHost", builder =>
                    //{
                    //    builder.WithOrigins("https://localhost:5003")
                    //        .AllowAnyMethod()
                    //        .AllowAnyHeader()
                    //        .AllowCredentials();
                    //});

                    options.AddPolicy("AllowAll", builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
                });

            //services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            //    .AddIdentityServerAuthentication(options =>
            //    {
            //        options.Authority = Configuration["IdentityServer:BaseAddress"];
            //        options.ApiName = Configuration["IdentityServer:ApiScope"];
            //        options.ApiSecret = "secret";
            //        options.EnableCaching = true;
            //        options.CacheDuration = TimeSpan.FromMinutes(10);
            //    });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddOAuth2Introspection(options =>
                {
                    options.Authority = Configuration["IdentityServer:BaseAddress"];
                    options.ClientId = Configuration["IdentityServer:ApiScope"];
                    options.ClientSecret = "secret";
                    options.EnableCaching = false;
                    options.CacheDuration = TimeSpan.FromMinutes(10);
                });

            services.AddAuthorization(options => 
            {
                options.AddPolicy("Audience", policy =>
                {
                    policy.RequireAssertion(Audience);
                });
            });

        }

        private readonly Func<AuthorizationHandlerContext, Task<bool>> Audience = async ctx =>
        {   
            // Custom policy login (eg get permissions from db
            return ctx.User.HasClaim("aud", "MyRestApi");
        };

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseCors("AllowAll");
            app.UseMiddleware<AuthenticationService>();

            app.UseAuthentication();
            app.UseMiddleware<RolesMiddleware>();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
