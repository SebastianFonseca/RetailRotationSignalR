using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using ServerConsole.Utilities;
using System.Threading.Tasks;


namespace ServerConsole
{
    [Authorize]
    public class RetailHUB : Hub
    {
        public async Task ServidorValidarUsuario(string Usuario, string Password)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("\n\t" + DateTime.Now+"--"); Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" User "+ Usuario + " is  now connected.");
            Console.ResetColor();
            Console.WriteLine("\n");
            Console.WriteLine(Usuario+ Password +DbConnection.Login(User: Usuario, Password: Password));
                await Clients.Caller.SendCoreAsync("ClienteValidacion", args: new object[] { Usuario, DbConnection.Login(User: Usuario, Password: Password) });
            
            
    
        }

        public async  Task TestMethod(string a)
        {
            await Clients.Caller.SendCoreAsync("SetStatus", args:new[] { a });
        }

    }

}
