using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SPAhost
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie(op =>
                {
                    op.Cookie.Name = "Cookies";
                    op.Cookie.Domain = "localhost";
                    //op.Cookie.Expiration = TimeSpan.FromSeconds(10);
                })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = "Cookies";

                    options.Authority = Configuration["IdentityServer:BaseAddress"];
                    options.RequireHttpsMetadata = false;

                    options.ClientId = Configuration["IdentityServer:ClientId"];
                    options.ClientSecret = Configuration["IdentityServer:ClientSecret"];
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.ResponseType = "id_token code";
                    options.Scope.Add(Configuration["IdentityServer:ApiScope"]);
                    options.Scope.Add("offline_access");
                    //options.UseTokenLifetime = true;
                    options.Events = new OpenIdConnectEvents()
                    {
                        OnTokenValidated = AddUserClaims()

                    };
                });
        }


        private static Func<TokenValidatedContext, System.Threading.Tasks.Task> AddUserClaims()
            => async ctx =>
            {
                // Get existing user identity (includes claims from JWT)
                var claimsIdentity = ctx.Principal.Identity as ClaimsIdentity;

                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Manager"));

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
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                await next();
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();
        }
    }
}
