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
using ServerConsole.Utilities;
using System;
using System.IdentityModel.Tokens.Jwt;
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
        private static IWebHostBuilder CreateWebHostBuildder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).
                UseStartup<Startup>();
        static void Main(string[] args)
        {
   

            //AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            CreateWebHostBuildder(args).Build().Run();    
            
        }
        static async void OnProcessExit(object sender, EventArgs e)
        {
            RetailHUB rh = new RetailHUB();
           await  rh.Desconectado("Desconectado del servidor");
            Console.WriteLine("Bye :)");
        }


    }
}
