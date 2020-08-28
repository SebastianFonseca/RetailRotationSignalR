using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServerConsole
{
    public class RetailHUB : Hub
    {
        public async Task ServidorValidarUsuario(string Usuario, string Password)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("\n\t" + DateTime.Now+"--"); Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" User "+ Usuario + " is  now connected. Passsword: " + Password);
            Console.ResetColor();
            Console.WriteLine("\n");
            await Clients.Caller.SendCoreAsync("ClienteValidacion", args: new[] {Usuario});
        }

    }

}
