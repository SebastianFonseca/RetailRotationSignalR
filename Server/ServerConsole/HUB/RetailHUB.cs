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
using ServerConsole.Models;

namespace ServerConsole
{
    [Authorize]
    public class RetailHUB : Hub
    {
        public async Task<bool> ServidorValidarUsuario(string Usuario, string Password)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("\n\t" + DateTime.Now+"--"); 
            Console.ForegroundColor = ConsoleColor.White;

            if (DbConnection.Login(User: Usuario, Password: Password))
            {
                Console.Write(" El usuario " + Usuario + " se conecto.");
                Console.ResetColor();
                Console.WriteLine("\n");
                return true;
                //await Clients.Caller.SendCoreAsync("ClienteValidacion", args: new object[] { Usuario, true });
            }
            else
            {
                Console.Write(" El usuario " + Usuario + " fallo al conectarse.");
                Console.ResetColor();
                Console.WriteLine("\n");
                return false;
                //await Clients.Caller.SendCoreAsync("ClienteValidacion", args: new object[] { Usuario, false });

            }

        }

        public async Task<int> ServidorAddClient(ClientesModel Cliente)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("\n\t" + DateTime.Now + "--");
            Console.ForegroundColor = ConsoleColor.White;
            String atempt = DbConnection.AddClient(Cliente);
            if (atempt  == "Cliente ya existe")
            {
                //await Clients.Caller.SendCoreAsync("ClienteExiste", args: new object[] { "El número de cedula ya esta registrado." });
                Console.WriteLine($"Se intento registrar un cliente que ya estaba registrado. CC: {Cliente.Cedula}.\n\n");
                return 0;
            }
            if ( atempt == "true")
            {
                Console.Write($" Se registro un nuevo cliente. CC: {Cliente.Cedula} Nombre: {Cliente.FirstName} {Cliente.LastName} \n\n");
                Console.ResetColor();
                return 1;
            }
            else
            {
                Console.ResetColor();
                return 2;
            }

        }

        public void  ClienteDesconectado(string a)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("\n\t" + DateTime.Now + "--");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"El cliente {a} se ha desonectado.\n\n");
            Console.ResetColor();

        }

        public void ClienteReconectado(string a)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("\n\t" + DateTime.Now + "--");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"El cliente {a} se ha conectado de nuevo.\n\n");
            Console.ResetColor();

        }


        public async  Task TestMethod(string a)
        {
            await Clients.Caller.SendCoreAsync("SetStatus", args:new[] { a });
        }

    }

}
