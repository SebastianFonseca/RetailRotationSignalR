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
        private static IWebHostBuilder CreateWebHostBuildder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).
                UseStartup<Startup>();
        static void Main(string[] args)
        {
            ///Se establece la cult-info IMPORTANTE pues cuado se hacen operaciones con decimales estos parametros son decisivos
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
                        XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.Name)));



            //AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            CreateWebHostBuildder(args).Build().Run();    
            
        }
        static async void OnProcessExit(object sender, EventArgs e)
        {
            RetailHUB rh = new RetailHUB();
           await  rh.Desconectado("Desconectado del servidor");
            Statics.Imprimir("Bye :)");
        }


    }
}
