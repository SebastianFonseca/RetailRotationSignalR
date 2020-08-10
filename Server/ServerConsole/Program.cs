using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Owin;
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

            public void ConfigureServices(IServiceCollection services)
            {
                //services.AddSignalRCore();
                services.AddSignalR();
            }

            public void Configure(IApplicationBuilder app)
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapHub<RetailHUB>("/retailHub");
                });

            }
        }
    }
}
