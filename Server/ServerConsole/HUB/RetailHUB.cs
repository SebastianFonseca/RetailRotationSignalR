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

            object[] resultado = DbConnection.Login(User: Usuario, Password: Password);
            if ((string)resultado[0] == "Registrado")
            {
                usuarioConectado = ((PersonModel)resultado[1]).cedula + "-" + ((PersonModel)resultado[1]).firstName + " " + ((PersonModel)resultado[1]).lastName;
                Statics.Imprimir(" El usuario " + usuarioConectado + " se ha conectado. Holi :)");

                //return resultado;
                //await Clients.Caller.SendCoreAsync("ClienteValidacion", args: new object[] { Usuario, true });
            }
            else
            {
                Statics.Imprimir(" El usuario " + Usuario + " fallo al conectarse.");

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

            Statics.Imprimir($"El usuario {a} se ha desconectado.\n\n");
            
        }

        /// <summary>
        /// Invocado desde un cliente cuando este se ha reconectado.
        /// </summary>
        /// <param name="a">Nombre del usuario que se reconecto.</param>
        public void ClienteReconectado(string a)
        {
            
            Statics.Imprimir($"El usuario {a} se ha conectado de nuevo.\n");
            

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
        
        /// <summary>
        /// Actualiza en la base de datos el precio del producto pasado como parametro.
        /// </summary>
        /// <param name="Producto"></param>
        /// <returns></returns>
        public  string ServidoractualizarPrecioProducto(ProductoModel Producto) 
        {
            return DbConnection.actualizarPrecioProducto(Producto);
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
        /// Retorna la informacion de todos los proveedores en la base de datos
        /// </summary>
        /// <returns></returns>
        public BindableCollection<ProveedorModel> ServidorGetTodosProveedor()
        {
            return DbConnection.getProveedores();
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
            return DbConnection.actualizarProveedor(proveedor);
        }

        /// <summary>
        /// Obtiene los proveedores
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public BindableCollection<ProveedorModel> ServidorgetNombresProveedores(string Caracteres)
        {
            return DbConnection.getNombresProveedores(Caracteres);
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

        /// <summary>
        /// Retorna las coincidencias en las existencias de los caracteres dados comparados con los codigos de las existencias y los codigos de los locales.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public BindableCollection<ExistenciasModel> servidorGetExistencias(string caracteres)
        {
            return DbConnection.getExistencias(caracteres);
        }

        /// <summary>
        /// Devuelve el nombre, codigo y existencia de los productos relacionados con el codigo de existencia dado como parametro.
        /// </summary>
        /// <param name="codigoExistencia"></param>
        /// <returns></returns>
        public BindableCollection<ProductoModel> servidorGetProductoExistencia(string codigoExistencia)
        {
            return DbConnection.getProductoExistencia(codigoExistencia);
        }

        /// <summary>
        /// Devuelve la instancia de las existencias junto con los productos relacionados con el codigo de existencia dado como parametro.
        /// </summary>
        /// <param name="codigoExistencia"></param>
        /// <returns></returns>
        public BindableCollection<ExistenciasModel> servidorGetExistenciasConProductos(string codigoExistencia)
        {
            return DbConnection.getExistenciasConProductos(codigoExistencia);
        }

        /// <summary>
        /// Retorna todos los documemtos de existencia sregistrados
        /// </summary>
        /// <returns></returns>
        public BindableCollection<ExistenciasModel> servidorGetTodasLasExistencias()
        {
            return DbConnection.getTodasLasExistencias();
        }


        #endregion

        #region Pedido

        /// <summary>
        /// Inserta en la base de datos el nuevo documeto de pedido
        /// </summary>
        /// <param name="pedido"></param>
        /// <returns></returns>
        public string ServidorNuevoPedido(PedidoModel pedido)
        {
            return DbConnection.NuevoPedido(pedido);
        } 
       
        /// <summary>
        /// Al ser llamado desde un cliente retorna los pedidoc que coinciden con el numero d elocal, la fecha o el codigo dado en los caracteres
        /// </summary>
        /// <param name="caracteres"></param>
        /// <returns></returns>
        public BindableCollection<PedidoModel> ServidorGetPedidos(string caracteres) 
        {
            return DbConnection.getPedidos(caracteres);
        }

        /// <summary>
        /// Retorna los productos del pedido del codigo dado
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public BindableCollection<ProductoModel> ServidorgetProductoPedido(String codigo)
        {
            return DbConnection.getProductoPedido(codigo);
        }

        /// <summary>
        /// Devuelve el pedido con el codigo dado junto con los productos.
        /// </summary>
        /// <param name="caracteres"></param>
        /// <returns></returns>
        public BindableCollection<PedidoModel> ServidorGetPedidoConProductos(string caracteres)
        {
            return DbConnection.getPedidoConProductos(caracteres);
        }

        /// <summary>
        /// Devuelve el pedido con el codigo dado junto con los productos.
        /// </summary>
        /// <returns></returns>
        public BindableCollection<PedidoModel> ServidorGetTodoPedidoConProductos()
        {
            return DbConnection.getTodoPedido();
        }
        #endregion

        #region Compras

        /// <summary>
        /// Inserta el registro del nuevo documento de compra.
        /// </summary>
        /// <param name="compra"></param>
        /// <returns></returns>
        public  string ServidorNuevaCompra(ComprasModel compras) 
        {
            return DbConnection.NuevaCompra(compras); 
        }

        /// <summary>
        /// Inserta en la base de datos, en la tabla RegistroCompra los datos de dicho documento.
        /// </summary>
        /// <param name="compra"></param>
        /// <returns></returns>
        public  string ServidorInsertarRegistroCompraProducto(ComprasModel compra)
        { 
            return DbConnection.InsertarRegistroCompraProducto(compra); 
        }

        /// <summary>
        /// Actualiza la informacion del registro de compra del producto dado como parametro.
        /// </summary>
        /// <param name="compra"></param>
        /// <returns></returns>
        public  string ServidorUpdateRegistroCompra(ComprasModel compra)
        { 
            return DbConnection.UpdateRegistroCompra(compra);
        }

        /// <summary>
        /// Inserta en la base de datos, en la tabla CompraPedido la relacion entre un documento de compra y los pedidos que la componen.
        /// </summary>
        /// <param name="compra"></param>
        /// <returns></returns>
        public  string ServidorInsertarPedidosCompra(ComprasModel compra)
        {
            return DbConnection.InsertarPedidosCompra(compra); 
        }

        /// <summary>
        /// Retorna las coincidencias en las compras con de los caracteres dados comparados con los codigos de los compra o la fecha.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public  BindableCollection<ComprasModel> ServidorgetCompra(string Caracteres) 
        { 
            return DbConnection.getCompras(Caracteres); 
        }

        /// <summary>
        /// Devuelve el nombre, codigo y suma  de los productos relacionados con el codigo del compra dado como parametro.
        /// </summary>
        /// <param name="codigoCompra"></param>
        /// <returns></returns>
        public  BindableCollection<ProductoModel> ServidorgetProductoCompra(string codigoCompra)
        {
            return DbConnection.getProductoCompra(codigoCompra); 
        }

        /// <summary>
        /// Retorna la compra del codigo dado como parametro, la informacion de los registros de compra y los documentos de pedido que conforman el documento de compra tambien es obtenida.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public BindableCollection<ComprasModel> ServidorGetComprasConProductos(string Caracteres)
        {
          return  DbConnection.getComprasConProductos(Caracteres);
        }

        /// <summary>
        /// Devuelve una instancia de la clase ComprasModel con un unico producto en la propiedad productos que contiene la informacion relacionada con el registro de la compra del producto dado como parametro.
        /// </summary>
        /// <param name="codigoCompraCodigoProducto"></param>
        /// <returns></returns>
        public  BindableCollection<ProductoModel> ServidorgetRegistroCompra(string codigoCompraCodigoProducto)
        {
           return  DbConnection.getRegistroCompra(codigoCompraCodigoProducto);
        }

        /// <summary>
        /// Retorna una lista de objetos de la clase ProductoModel con los registros de compra del proveedor o el producto dado como parametro.
        /// /// </summary>
        /// <param name="codigoProductoCedula"></param>
        /// <returns></returns>
        public BindableCollection<ProductoModel> ServidorgetRegistroCompraCodigoCedula(string codigoProductoCedula)
        {
            return DbConnection.getRegistroCompraCodigoCedula(codigoProductoCedula);
        }

        /// <summary>
        /// Devuelve una instancia de la clase producto con la informacion relacionada con su ultimo registro de compra.
        /// </summary>
        /// <param name="codigoCompra"></param>
        /// <returns></returns>
        public  BindableCollection<ProductoModel> ServidorgetUltimoRegistroCompraProducto(string codigoProducto)
        {
            return DbConnection.getUltimoRegistroCompraProducto(codigoProducto);
        }

        /// <summary>
        /// Retorna informacion de las compras pendietes por pagar, y del número total de compras
        /// </summary>
        /// <returns></returns>
        public decimal[] ServidorinfoRegistros()
        {
            return DbConnection.infoRegistros();
        }

        #endregion

        #region Envios

        /// <summary>
        /// Regista en la base de datos la informacion relacionada con el nuevo documento de envio.
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public string ServidorNuevoEnvioBool(EnvioModel envio)
        {
            return   DbConnection.NuevoEnvioBool(envio);
        }

        /// <summary>
        /// Obtiene los productos con la cantidad enviada para el documento de envio.
        /// </summary>
        /// <param name="codigoEnvio"></param>
        /// <returns></returns>
        public BindableCollection<ProductoModel> ServidorgetProductoEnvio(string codigoEnvio)
        {
            return DbConnection.getProductoEnvio(codigoEnvio);
        }

        /// <summary>
        /// Devuelve la instancia del pedido envio con los productos relacionados con el codigo de pedido dado como parametro.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>

        public BindableCollection<EnvioModel> ServidorgetEnvioConProductos(string Caracteres)
        {
            return DbConnection.getEnvioConProductos(Caracteres);
        }

        /// <summary>
        /// Actualiza la informacion de un envio.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string ServidorupdateEnvio(EnvioModel envio)
        {
            return DbConnection.updateEnvio(envio);
        }

        /// <summary>
        /// Devuelve todas las instancias de envios por local registradas en la base de datos.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public  BindableCollection<EnvioModel> ServidorgetTodosLosEnviosPorLocal(string ccEmpleado)
        {
            return DbConnection.getTodosLosEnviosPorLocal(ccEmpleado);
        }
        
        /// <summary>
        /// Obtiene la suma de envios de la ultima compra del producto con el codigo dado
        /// </summary>
        /// <param name="codigoProducto"></param>
        /// <returns></returns>
        public decimal? ServidorgetTotalEnvioProduco(string codigoProducto) 
        {
            return DbConnection.getTotalEnvioProduco(codigoProducto);
        }


        #endregion

        #region Recibido

        /// <summary>
        /// Inserta en la base de datos los datos del nuevo recibido.
        /// </summary>
        /// <param name="recibido"></param>
        /// <returns></returns>
        public string ServidorNuevoRecibidoBool(RecibidoModel recibido)
        {
            return DbConnection.NuevoRecibidoBool(recibido);
        }

        /// <summary>
        /// Regista en la base de datos la informacion relacionada con el nuevo documento de envio.
        /// </summary>
        /// <param name="recibido"></param>
        /// <returns></returns>
        public string ServidorNuevoRecibidoBoolNoInventario(RecibidoModel recibido)
        {
            return DbConnection.NuevoRecibidoBoolNoInventario(recibido);
        }

        /// <summary>
        /// Retorna la instancia del recibido con el codigo dado como parametro, incluidos los productos 
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public  BindableCollection<RecibidoModel> ServidorgetRecibidoConProductos(string Caracteres)
        {
            return DbConnection.getRecibidoConProductos(Caracteres);
        }

        /// <summary>
        /// Retorna las coincidencias en los recibidos del local dado como parametro, los codigos y fechas de los recibidos.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <param name="codigoPuntoVenta"></param>
        /// <returns></returns>
        public  BindableCollection<RecibidoModel> ServidorgetRecibidos(string Caracteres, string codigoPuntoVenta)
        {
            return DbConnection.getRecibidos(Caracteres, codigoPuntoVenta);
        }

        /// <summary>
        /// Obtiene los productos con la cantidad enviada y la recibida en el documento de recibido con codigo igual al dado como parametro
        /// </summary>
        /// <param name="codigoRecibido"></param>
        /// <returns></returns>
        public  BindableCollection<ProductoModel> ServidorgetProductosRecibido(string codigoRecibido)
        {
            return DbConnection.getProductosRecibido(codigoRecibido);
        }

        /// <summary>
        /// Actualiza la informacion de un recibido.
        /// </summary>
        /// <param name="recibido"></param>
        /// <returns></returns>
        public string ServidorupdateRecibido(RecibidoModel recibido)
        {            
            return DbConnection.updateRecibido(recibido);
        }

        /// <summary>
        /// Actualiza la informacion de un recibido.
        /// </summary>
        /// <param name="recibido"></param>
        /// <returns></returns>
        public  string ServidorupdateRecibidoNoInventario(RecibidoModel recibido) 
        {
            return DbConnection.updateRecibidoNoInventario(recibido);
        }

        #endregion

        #region Inventario

        /// <summary>
        /// Retorna la instancia de inventario del local con el nombre dado
        /// </summary>
        /// <param name="nombreLocal"></param>
        /// <returns></returns>
        public BindableCollection<InventarioModel> ServidorgetInventario(string nombreLocal)
        {
            return DbConnection.getInventario(nombreLocal);
        }

        /// <summary>
        /// Inserta en la base de datos el cambio en el inventario del local dado en el invetario
        /// </summary>
        /// <param name="inventario"></param>
        /// <returns></returns>
        public string ServidorNuevoRegistroCambioEnInventario(InventarioModel inventario)
        {
            return DbConnection.NuevoRegistroCambioEnInventario(inventario);
        }

        /// <summary>
        /// Obtiene el registro de la tabla con los registros de cambios en inventario
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public  BindableCollection<InventarioModel> ServidorgetCambioInventario(string codigo)
        {
            return DbConnection.getCambioInventario(codigo);
        }



        #endregion

        #region Clientes
        /// <summary>
        /// Cuando es llamado desde un 'cliente' registra un nuevo cliente.
        /// </summary>
        /// <param name="Cliente">Instancia de la claseCLientesModel</param>
        /// <returns></returns>
        public string ServidorAddClient(ClientesModel Cliente)
        {
                return DbConnection.AddClient(Cliente);
        }

        /// <summary>
        /// Method that does a select query against the 'Clientes' table at the local database and get the result of searching the coincidences into the cedula, nombre or apellido of the given characters.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public  BindableCollection<ClientesModel> ServidorgetClientes(string Caracteres)
        {
            return DbConnection.getClientes(Caracteres);
        }
        
        /// <summary>
        /// Method that does a select query against the 'Clientes' table at the local database and get the result of searching the coincidences into the cedula, nombre or apellido of the given characters.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public  BindableCollection<ClientesModel> ServidorgetClienteCedula(string Caracteres)
        {
            return DbConnection.getClienteCedula(Caracteres);
        }

        /// <summary>
        ///  Elimina el cliente con el número de cédula dado.
        /// </summary>
        /// <param name="Cedula">Número de cédula del cliente a eliminar.</param>
        /// <returns></returns>
        public  string ServidordeleteCliente(string Cedula)
        {
            return DbConnection.deleteCliente(Cedula);
        }
       
        /// <summary>
        /// Actualiza los datos del usuario dado.
        /// </summary>
        /// <param name="Cliente"></param>
        /// <returns></returns>
        public  string ServidorActualizarCliente(ClientesModel Cliente)
        {
            return DbConnection.ActualizarCliente(Cliente);
        }

        /// <summary>
        /// Devuelve el promedio de clientes que registraron su factura para el local dado en las ultimas 500 facturas
        /// </summary>
        /// <param name="idLocal"></param>
        /// <returns></returns>
        public  decimal[] ServidorpromediofacturasRegistradas(string? idLocal)
        {
            return DbConnection.promediofacturasRegistradas(idLocal);
        }



        #endregion

        #region Facturas

        /// <summary>
        /// Registra en la base de datos la informacion relacionada con la nueva factura
        /// </summary>
        /// <param name="factura">Datos de la factura que se va a registrar</param>
        /// <returns></returns>
        public string ServidorNuevaFacturaBool(FacturaModel factura) 
        {
            return DbConnection.NuevaFacturaBool(factura);
        }

        /// <summary>
        /// Registra en la base de datos la informacion relacionada con la  factura borrada
        /// </summary>
        /// <param name="factura">Datos de la factura que se va a registrar</param>
        /// <returns></returns>
        public  string ServidorNuevaFacturaBorradaBool(FacturaModel factura)
        {
            return DbConnection.NuevaFacturaBorradaBool(factura);
        }


        /// <summary>
        /// Obtiene los datos de las facturas del cliente dado como parametro
        /// </summary>
        /// <param name="cedulaCliente"></param>
        /// <returns></returns>
        public  object[] ServidorgetFacturasCliente(string cedulaCliente)
        {
          return  DbConnection.getFacturasCliente(cedulaCliente);
        }








        #endregion

        #region Ingresos
        /// <summary>
        /// Registra el nuevo ingreso
        /// </summary>
        /// <param name="Ingreso"></param>
        /// <returns></returns>
        public string ServidorNuevoIngreso(IngresoModel Ingreso)
        {
            return DbConnection.NuevoIngreso(Ingreso);
        }

        /// <summary>
        /// Obtiene el ingreso registrado con las correspondientes facturas
        /// </summary>
        /// <param name="codigoIngreso"></param>
        /// <returns></returns>
        public  BindableCollection<IngresoModel> ServidorgetIngresoConFacturas(string codigoIngreso)
        {
            return DbConnection.getIngresoConFacturas(codigoIngreso);
        }



        #endregion

        #region MovimientoEfectivo

        /// <summary>
        /// retorna el proximo valor de la coumna indentada ItemsMovimientoEfectivo
        /// </summary>
        /// <returns></returns>
        public string ServidorgetNextIdMovimientoEfectivo()
        {
            return DbConnection.getNextIdMovimientoEfectivo();
        }

        /// <summary>
        /// Inserta la descripcion y tipo del nuevo itemMovimientoEfectivo
        /// </summary>
        /// <param name="movimiento"></param>
        /// <returns></returns>
        public string ServidornuevoItemMovimientoEfectivo(ItemMovimientoEfectivoModel movimiento)
        {
            return DbConnection.nuevoItemMovimientoEfectivo(movimiento);
        }

        /// <summary>
        /// Retorna los items de egresos
        /// </summary>
        /// <returns></returns>
        public BindableCollection<ItemMovimientoEfectivoModel> ServidorgetItemsEgresos() 
        {
            return DbConnection.getItemsEgresos();
        }


        /// <summary>
        /// Retorna los movimientos de efectivo de un local
        /// </summary>
        /// <param name="codigoLocal"></param>
        /// <returns></returns>
        public  BindableCollection<MovimientoEfectivoModel> ServidorGetMovimientosEfectivoLocal(string codigoLocal)
        {
            return DbConnection.GetMovimientosEfectivoLocal(codigoLocal);
        }

        #endregion

        #region Egreso
        /// <summary>
        /// Retorna el valor del codigo del nuevo egreso
        /// </summary>
        /// <returns></returns>
        public string ServidorgetNextIdEgreso() 
        {
            return DbConnection.getNextIdEgreso();
         }

        /// <summary>
        /// Registra el nuevo egreso y su correspondiente registro en el movimiemto de efectivo
        /// </summary>
        /// <param name="egreso"></param>
        /// <returns></returns>
        public  string ServidorNuevoEgreso(EgresoModel egreso)
        {
            return DbConnection.NuevoEgreso(egreso);
        }

        /// <summary>
        /// Retorna la inforamcin de un egreso
        /// </summary>
        /// <param name="codigoEgreso"></param>
        /// <returns></returns>
        public  BindableCollection<EgresoModel> ServidorgetEgreso(string codigoEgreso)
        {
            return DbConnection.getEgreso(codigoEgreso);
        }

        /// <summary>
        /// Registra el pago a un proveedor
        /// </summary>
        /// <param name="egreso"></param>
        /// <returns></returns>
        public string ServidorpagoProveedor(EgresoModel egreso)
        {
            return DbConnection.pagoProveedor(egreso);
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







    }


}
