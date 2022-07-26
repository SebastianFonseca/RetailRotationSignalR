using Microsoft.AspNet.SignalR;
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
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.SignalR;
//using System;
//using Microsoft.AspNetCore.Internal;

namespace ServerConsole
{
    class Program
    {
        private static IWebHostBuilder CreateWebHostBuildder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args).
            UseStartup<Startup>();
        }

        static void Main(string[] args)
        {


            ///Se establece la cult-info IMPORTANTE pues cuado se hacen operaciones con decimales estos parametros son decisivos
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
                        XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.Name)));



            //AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;

            CreateWebHostBuildder(args).Build().Run();

            Console.ReadKey();
            Console.ReadLine();
            Console.WriteLine("Cambios!");

        }

        //private IHubContext _context;
        static async void OnDomainUnload(object sender, EventArgs e)
        {


            await GlobalHost.ConnectionManager.GetHubContext("RetailHUB").Clients.All.SendCoreAsync("SetStatusDisconnected", args: new[] { "Desconectado del servidor." });
            //RetailHUB rh = new RetailHUB();
            //await  rh.Desconectado("Desconectado del servidor");
            Statics.Imprimir("Bye :)");
        }






    }
}
