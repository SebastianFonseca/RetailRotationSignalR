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
        public object[] ServidorValidarUsuario(string Usuario, string Password)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("\n\t" + DateTime.Now + "--");
            Console.ForegroundColor = ConsoleColor.White;
            object[] resultado = DbConnection.Login(User: Usuario, Password: Password);
            if ((string)resultado[0] == "Registrado")
            {
                usuarioConectado = ((PersonModel)resultado[1]).cedula+ "-" + ((PersonModel)resultado[1]).firstName + " " +((PersonModel)resultado[1]).lastName;
                Console.Write(" El usuario " + usuarioConectado + " se ha conectado.");
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
        /// Invocado cuando un cliente se desconecta.
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

        #region Productos
        /// <summary>
        /// Cuando es llamado desde un 'cliente' registra un nuevo producto.
        /// </summary>
        /// <param name="Producto">Instancia de la clase ProductoModel</param>
        /// <returns></returns>
        public string ServidorNuevoProducto(ProductoModel Producto)
        {
            return DbConnection.NuevoProducto(Producto);
        }

        /// <summary>
        /// Retorna el Id y el nombre de los productos en la base de datos.
        /// </summary>
        /// <returns></returns>
        public BindableCollection<ProductoModel> ServidorgetIdProductos()
        {
            return DbConnection.getProductos();
        }

        /// <summary>
        /// Invocado dede un cliente retorna los productos que coinciden con los caracteres dados.
        /// </summary>
        /// <param name="caracteres"></param>
        /// <returns></returns>
        public BindableCollection<ProductoModel> ServidorGetProductos(string caracteres)
        {
            return DbConnection.getProductos(caracteres);
        }

        /// <summary>
        /// Metodo llamado desde un cliente para eliminar de la base de datos la informacion del producto con el ID dado como parametro.
        /// </summary>
        /// <param name="idProducto"></param>
        /// <returns></returns>
        public string ServidorDeleteProducto(string idProducto)
        {
            return DbConnection.deleteProducto(idProducto);
        }

        /// <summary>
        /// Metodo llamado desde un cliente para actualizar la informacion de un producto.
        /// </summary>
        /// <param name="producto"></param>
        /// <returns></returns>
        public string ServidorActualizarProducto(ProductoModel producto)
        {
            return DbConnection.actualizarProducto(producto);
        }

        #endregion

        #region Locales
        /// <summary>
        ///  Cuando es llamado desde un 'cliente' registra un nuevo Local.
        /// </summary>
        /// <param name="Local">Instancia de la clase LocalModel.</param>
        /// <returns></returns>
        public string ServidorNuevoLocal(LocalModel Local)
        {
            return DbConnection.NuevoLocal(Local);
        }

        /// <summary>
        /// Elimina el local en la base de datos correspondiente al codigo dado.
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public object ServidorEliminarLocal(string codigo, string nombre)
        {
            return DbConnection.deleteLocal(codigo, nombre);
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
        public BindableCollection<LocalModel> ServidorGetLocales(string Caracteres)
        {
            return DbConnection.getLocales(Caracteres);
        }

        /// <summary>
        /// Retorna una lista con el codigo y nombre de cada local en la base de datos.
        /// </summary>
        /// <returns></returns>
        public BindableCollection<LocalModel> ServidorGetIdLocales()
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

        /// <summary>
        /// Elimina al usuario con el numero de cedula dado.
        /// </summary>
        /// <param name="Cedula"></param>
        /// <returns></returns>
        public string ServidorDeleteUsuario(string Cedula)
        {
            return DbConnection.DeleteEmpleado(Cedula);
        }

        /// <summary>
        /// Actualiza la informacion del usuario con el número de cedula dado como parametro.
        /// </summary>
        /// <param name="Empleado"></param>
        /// <param name="CC"></param>
        /// <returns></returns>
        public string ServidorUpdateUsuario(EmpleadoModel Empleado, string CC)
        {
            return DbConnection.ActualizarUsuario(Empleado, CC);
        }
        #endregion

        #region Proveedores
        /// <summary>
        /// Metodo que al ser llamado desde un cliente agrega los datos de un nuevo proveedor.
        /// </summary>
        /// <param name="proveedor"></param>
        /// <returns></returns>
        public string ServidorNuevoProveedor(ProveedorModel proveedor)
        {           
            return DbConnection.NuevoProveedor(proveedor);
        }

        /// <summary>
        /// Retorna los Proveedores encontrados en la base de datos.
        /// </summary>
        /// <param name="caracteres"></param>
        /// <returns></returns>
        public BindableCollection<ProveedorModel> ServidorGetProveedores(string caracteres)
        {
            return DbConnection.getProveedores(caracteres);
        }

        /// <summary>
        /// Retorna un proveedor especificamente daso su numero de cedula.
        /// </summary>
        /// <param name="CedulaProveedor"></param>
        /// <returns></returns>
        public BindableCollection<ProveedorModel> ServidorGetProveedor(string numeroDeCedula)
        {
            return DbConnection.getProveedor(numeroDeCedula);
        }

        /// <summary>
        /// Metodo que se llama desde un cliente para eliminar un proveedor.
        /// </summary>
        /// <param name="cedula"></param>
        /// <returns></returns>
        public string ServidorDeleteProveedor(string cedula)
        {
            return DbConnection.deleteProveedor(cedula);
        }

        /// <summary>
        /// Cuado es invocado desde un cliete actualiza en la base de datos la informacion de la instancia de proveedor dada.
        /// </summary>
        /// <param name="proveedor"></param>
        /// <returns></returns>
        public string ServidorActualizarProveedor(ProveedorModel proveedor)
        {
          return  DbConnection.actualizarProveedor(proveedor);
        }
        #endregion

        #region Existencias
        /// <summary>
        /// Metodo que al ser llamado desde un cliente agrega los datos de un nuevo docuemento de existencias.
        /// </summary>
        /// <param name="existencias"></param>
        /// <returns></returns>
        public string ServidorNuevaExistencia(ExistenciasModel existencias)
        {
            return DbConnection.NuevaExistencia(existencias);
        } 
        #endregion

        /// <summary>
        /// Cuando es llamado desde un cliente retorna los registros de cambios guardados en la tabla que consigna dicha informacion en la base de datos
        /// </summary>
        /// <returns></returns>
        public BindableCollection<string[]> ServidorNuevosRegistros(int a)
        {
            return DbConnection.registroCambios(a); 
        }


        /// <summary>
        /// Cuando es llamado desde un 'cliente' registra un nuevo cliente.
        /// </summary>
        /// <param name="Cliente">Instancia de la claseCLientesModel</param>
        /// <returns></returns>
        public int ServidorAddClient(ClientesModel Cliente)
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
                Console.Write($" Se registro un nuevo cliente. CC: {Cliente.cedula} Nombre: {Cliente.firstName} {Cliente.lastName} \n\n");
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
