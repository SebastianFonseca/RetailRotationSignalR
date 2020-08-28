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
using System.Security.Claims;
using System.Threading.Tasks;

//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.SignalR;
//using System;
//using Microsoft.AspNetCore.Internal;

namespace ServerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateWebHostBuildder(args).Build().Run();
        }

        private static IWebHostBuilder CreateWebHostBuildder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).
                UseStartup<Startup>();


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
                           // Ensure the token audience matches our audience value (default true):
                           ValidateAudience = false,
                           ValidAudience = "api://default",
                           // Ensure the token was issued by a trusted authorization server (default true):
                           ValidateIssuer = false,
                           ValidIssuer = "https://{yourOktaDomain}/oauth2/default"
                       };

                       // We have to hook the OnMessageReceived event in order to
                       // allow the JWT authentication handler to read the access
                       // token from the query string when a WebSocket or 
                       // Server-Sent Events request comes in.

                       // Sending the access token in the query string is required due to
                       // a limitation in Browser APIs. We restrict it to only calls to the
                       // SignalR hub in this code.
                       // See https://docs.microsoft.com/aspnet/core/signalr/security#access-token-logging
                       // for more information about security considerations when using
                       // the query string to transmit the access token.
                       options.Events = new JwtBearerEvents
                       {
                           //OnMessageReceived = context =>
                           //{
                           //    Console.WriteLine(context);
                           //    var accessToken = context.Request.Query["access_token"];

                           //    Console.WriteLine(accessToken);

                           //     // If the request is for our hub...
                           //     var path = context.HttpContext.Request.Path;
                           //    if (!string.IsNullOrEmpty(accessToken) &&
                           //        (path.StartsWithSegments("/retailH")))
                           //    {
                           //         // Read the token out of the query string
                           //         context.Token = accessToken;
                           //    }
                           //    return Task.CompletedTask;
                           //}
                           OnMessageReceived = context => {
                               Console.ForegroundColor = ConsoleColor.DarkCyan;

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
}
