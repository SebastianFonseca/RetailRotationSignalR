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
using Caliburn.Micro;

namespace ServerConsole
{
    [Authorize]
    public class RetailHUB : Hub
    {
        /// <summary>
        /// Nombre del usuario corespondiente al cliente conectado.
        /// </summary>
        public static string usuarioConectado;

        /// <summary>
        /// Usado para testear la conexion entre el servidor y un cliente.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public async Task TestMethod(string a)
        {
            await Clients.Caller.SendCoreAsync("SetStatus", args: new[] { a });
        }

        public async Task Desconectado(string a)
        {
            await Clients.Caller.SendCoreAsync("SetStatusDisconnected", args: new[] { "Desconectado del servidor." });
        }

        /// <summary>
        /// Validacion de la contraseña de un usuario.
        /// </summary>
        /// <param name="Usuario"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public string[] ServidorValidarUsuario(string Usuario, string Password)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("\n\t" + DateTime.Now + "--");
            Console.ForegroundColor = ConsoleColor.White;
            string[] resultado = DbConnection.Login(User: Usuario, Password: Password);
            if (resultado[0] == "Registrado")
            {
                usuarioConectado = Usuario;
                Console.Write(" El usuario " + Usuario + " se ha conectado.");
                Console.ResetColor();
                Console.WriteLine("\n");
                //return resultado;
                //await Clients.Caller.SendCoreAsync("ClienteValidacion", args: new object[] { Usuario, true });
            }
            else
            {
                Console.Write(" El usuario " + Usuario + " fallo al conectarse.");
                Console.ResetColor();
                Console.WriteLine("\n");
                //return resultado;
                //await Clients.Caller.SendCoreAsync("ClienteValidacion", args: new object[] { Usuario, false });

            }
            return resultado;
        }

        /// <summary>
        /// Invocado cuendo un cliente se desconecta.
        /// </summary>
        /// <param name="a">Nombre del usuario que se desconecta.</param>
        public void ClienteDesconectado(string a)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("\n\t" + DateTime.Now + "--");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"El usuario {a} se ha desconectado.\n\n");
            Console.ResetColor();

        }

        /// <summary>
        /// Invocado desde un cliente cuando este se ha reconectado.
        /// </summary>
        /// <param name="a">Nombre del usuario que se reconecto.</param>
        public void ClienteReconectado(string a)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("\n\t" + DateTime.Now + "--");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"El usuario {a} se ha conectado de nuevo.\n");
            Console.ResetColor();

        }

        /// <summary>
        /// Cuando es llamado desde un 'cliente' registra un nuevo producto.
        /// </summary>
        /// <param name="Producto">Instancia de la clase ProductoModel</param>
        /// <returns></returns>
        public async Task<string> ServidorNuevoProducto(ProductoModel Producto)
        {
            return DbConnection.NuevoProducto(Producto);
        }

        #region Locales
        /// <summary>
        ///  Cuando es llamado desde un 'cliente' registra un nuevo Local.
        /// </summary>
        /// <param name="Local">Instancia de la clase LocalModel.</param>
        /// <returns></returns>
        public async Task<string> ServidorNuevoLocal(LocalModel Local)
        {
            return DbConnection.NuevoLocal(Local);
        }

        /// <summary>
        /// Elimina el local en la base de datos correspondiente al codigo dado.
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public string ServidorEliminarLocal(string codigo)
        {
            return DbConnection.deleteLocal(codigo);
        }

        /// <summary>
        /// Actualiza la informacion del local dado.
        /// </summary>
        /// <param name="Local"></param>
        /// <param name="IdAnterior"></param>
        /// <returns></returns>
        public string ServidorActualizarLocal(LocalModel Local, string IdAnterior)
        {
            return DbConnection.ActualizarLocal(Local, IdAnterior);
        }

        /// <summary>
        /// Retorna los locales que coinciden, en nombre o codigo con los caracteres dados como argumento.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public async Task<BindableCollection<LocalModel>> ServidorGetLocales(string Caracteres)
        {
            return DbConnection.getLocales(Caracteres);
        }

        /// <summary>
        /// Retorna una lista con el codigo y nombre de cada local en la base de datos.
        /// </summary>
        /// <returns></returns>
        public async Task<BindableCollection<LocalModel>> ServidorGetIdLocales()
        {
            return DbConnection.getLocales();

        }
        #endregion

        #region Usuarios
        /// <summary>
        /// Registra en la base de datos el nuevo empleado.
        /// </summary>
        /// <param name="empleado"></param>
        /// <returns></returns>
        public string ServidorCreateNuevoUsuario(EmpleadoModel empleado)
        {
            return DbConnection.NuevoUsuario(empleado);
        }

        /// <summary>
        /// Busca en la tabla empleados las coincidencias de cedula, nombre o apellidos de los catacteres dados.
        /// </summary>
        /// <param name="caracteres"></param>
        /// <returns></returns>
        public BindableCollection<EmpleadoModel> ServidorGetUsuarios(string caracteres)
        {
            return DbConnection.getEmpleados(caracteres);
        }

        #endregion

        /// <summary>
        /// Cuando es llamado desde un 'cliente' registra un nuevo cliente.
        /// </summary>
        /// <param name="Cliente">Instancia de la claseCLientesModel</param>
        /// <returns></returns>
        public async Task<int> ServidorAddClient(ClientesModel Cliente)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("\n\t" + DateTime.Now + "--");
            Console.ForegroundColor = ConsoleColor.White;
            String atempt = DbConnection.AddClient(Cliente);
            if (atempt == "Cliente ya existe")
            {
                //await Clients.Caller.SendCoreAsync("ClienteExiste", args: new object[] { "El número de cedula ya esta registrado." });
                //Console.WriteLine($"Se intento registrar un cliente que ya estaba registrado. CC: {Cliente.Cedula}.\n\n");
                return 0;
            }
            if (atempt == "true")
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



    }


}
