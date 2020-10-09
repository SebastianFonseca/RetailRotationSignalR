using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Owin;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServerConsole
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        public string SecretKey { get; set; } = "TW9zaGVFcmV6UHJpdmF0ZUtleQ==";
        private SecurityKey GetSymmetricSecurityKey()
        {
            byte[] symmetricKey = Convert.FromBase64String(SecretKey);
            return new SymmetricSecurityKey(symmetricKey);
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                // Identity made Cookie authentication the default.
                // However, we want JWT Bearer Auth to be the default.
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                       // Clock skew compensates for server time drift.
                       // We recommend 5 minutes or less:
                    ClockSkew = TimeSpan.FromMinutes(5),
                       // Specify the key used to sign the token:
                       IssuerSigningKey = GetSymmetricSecurityKey(),
                   RequireSignedTokens = true,
                       // Ensure the token hasn't expired:
                       RequireExpirationTime = true,
                   ValidateLifetime = true,
                  // Ensure the token audience matches our audience value(default true):
                       ValidateAudience = false,
                   ValidAudience = "api://default",
                   // Ensure the token was issued by a trusted authorization server (default true):
                   ValidateIssuer = false,
                   ValidIssuer = "https://{yourOktaDomain}/oauth2/default"
               };

               options.Events = new JwtBearerEvents
               {
                   OnAuthenticationFailed = context =>
                   {

                       Console.ForegroundColor = ConsoleColor.Red;
                       Console.WriteLine(context.ToString());
                       Console.ResetColor();

                       return Task.CompletedTask;

                   }

               };
           });


            services.AddMvc();
            services.AddSignalRCore();
            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<RetailHUB>("/retailHub");
            });

        }
    }
}

