using Caliburn.Micro;
using Client.Main.Models;
using Dapper;
using Dotmim.Sync;
using Dotmim.Sync.SqlServer;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Replication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Client.Main.ViewModels.AddClientViewModel;

namespace Client.Main.Utilities
{
    public class DbConnection
    {
        /// <summary>
        /// Variable local para obtener la cadena de conexion de la base de datos local.
        /// </summary>
        private static string _connString = ConnectionString("RetailRotationClientDataBase");

        /// <summary>
        /// Metodo responsable de verificar en la base de datos localmente si el usuario que intenta ingresar esta o no registrado.
        /// </summary>
        /// <param name="User"></param>
        /// <param name="Password"></param>
        /// <returns></returns>             
        public static object[] Login(string User, string Password)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = "select empleado.Password, Empleado.CedulaEmpleado,empleado.CodigoPuntoVenta,empleado.Nombres,Empleado.Apellidos,Empleado.FechaContratacion,empleado.Salario,Empleado.Telefono,Empleado.Cargo,Empleado.Direccion, PuntoVenta.Nombres as nombrePuntoVenta from empleado join puntoventa on empleado.codigopuntoventa = puntoventa.codigopuntoventa where [CedulaEmpleado]=@user and empleado.Estado = 'Activo'";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@user", User);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows) { return new[] { "Usuario no registrado." }; }
                        while (reader.Read())
                        {
                            if (Statics.Verify(Password, reader["Password"].ToString()))
                            {
                                EmpleadoModel persona = new EmpleadoModel
                                {
                                    cedula = reader["CedulaEmpleado"].ToString()
                                };
                                persona.puntoDeVenta.codigo = reader["CodigoPuntoVenta"].ToString();
                                persona.puntoDeVenta.nombre = reader["nombrePuntoVenta"].ToString();
                                persona.firstName = reader["Nombres"].ToString();
                                persona.lastName = reader["Apellidos"].ToString();
                                persona.fechaDeContratacion = DateTime.Parse(reader["FechaContratacion"].ToString());
                                persona.salario = Convert.ToDecimal(reader["Salario"].ToString());
                                persona.telefono = reader["Telefono"].ToString();
                                persona.cargo = reader["Cargo"].ToString();
                                persona.direccion = reader["Direccion"].ToString();
                                return new object[] { "Registrado", persona };
                            }

                        }
                        conn.Close();
                        return new[] { "Contraseña incorrecta." };
                    }

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return new[] { e.Message };
            }


        }


        #region Productos
        /// <summary>
        /// Registra en la base de datos el nuevo producto.
        /// </summary>
        /// <param name="Producto">Instancia de la clase ProductoModel</param>
        /// <returns></returns>
        public static bool NuevoProducto(ProductoModel Producto)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena0 = "SELECT *  FROM Producto where [CodigoProducto]=@codigo";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.Parameters.AddWithValue("@codigo", Producto.codigoProducto);
                    conn.Open();
                    using (SqlDataReader reader0 = cmd0.ExecuteReader())
                    {
                        if (reader0.HasRows)
                        {
                            conn.Close();
                            return false;
                            //return "Codigo de producto ya registrado.";
                        }
                    }
                    string cadena1 = "SELECT *  FROM Producto where [codigoBarras]=@codigob";
                    SqlCommand cmd1 = new SqlCommand(cadena1, conn);
                    cmd1.Parameters.AddWithValue("@codigob", Producto.codigoBarras);
                    using (SqlDataReader reader1 = cmd1.ExecuteReader())
                    {
                        if (reader1.HasRows)
                        {
                            conn.Close();
                            return false;
                            //return "Codigo de barras ya registrado.";
                        }
                    }
                    string cadena = "SELECT *  FROM Producto where [Nombre]=@nombre";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@nombre", Producto.nombre);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            conn.Close();
                            return false;
                            // return "Nombre de producto ya registrado.";
                        }
                        else
                        {
                            reader.Close();
                            string cadena2 = "INSERT INTO Producto(CodigoProducto, Nombre, UnidadVenta,	UnidadCompra, PrecioVenta, Seccion, FechaVencimiento, IVA, CodigoBarras, Estado, FactorConversion )" +
                                " VALUES (@codigo,@nombre,@univenta,@unicompra,@precio,@seccion,@fv,@iva,@cb,'Activo', @fc)";
                            SqlCommand cmd2 = new SqlCommand(cadena2, conn);
                            cmd2.Parameters.AddWithValue("@codigo", Statics.PrimeraAMayuscula(Producto.codigoProducto));
                            cmd2.Parameters.AddWithValue("@nombre", Statics.PrimeraAMayuscula(Producto.nombre));
                            cmd2.Parameters.AddWithValue("@univenta", Statics.PrimeraAMayuscula(Producto.unidadVenta));
                            cmd2.Parameters.AddWithValue("@unicompra", Statics.PrimeraAMayuscula(Producto.unidadCompra));
                            cmd2.Parameters.AddWithValue("@precio", decimal.TryParse(Producto.precioVenta.ToString(), out decimal b) ? b : (object)DBNull.Value);
                            cmd2.Parameters.AddWithValue("@seccion", Statics.PrimeraAMayuscula(Producto.seccion));
                            cmd2.Parameters.AddWithValue("@fv", Producto.fechaVencimiento == DateTime.MinValue ? (object)DBNull.Value : Producto.fechaVencimiento);
                            cmd2.Parameters.AddWithValue("@iva", decimal.TryParse(Producto.iva.ToString(), out decimal c) ? c : (object)DBNull.Value);
                            cmd2.Parameters.AddWithValue("@cb", string.IsNullOrEmpty(Producto.codigoBarras) ? (object)DBNull.Value : Producto.codigoBarras);
                            cmd2.Parameters.AddWithValue("@fc", decimal.TryParse(Producto.factorConversion.ToString(), out decimal d) ? d : (object)DBNull.Value);
                            cmd2.ExecuteNonQuery();
                            conn.Close();
                            nuevoInventarioProducto(Producto.codigoProducto);
                            return true;
                            ///return $"Nuevo producto: {Producto.codigoProducto}- {Producto.nombre}.";
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
                //return e.Message;
            }
        }

        /// <summary>
        /// Actualiza en la base de datos la informacion relacionada al producto dado.
        /// </summary>
        /// <param name="Producto"></param>
        /// <returns></returns>
        public static bool actualizarProducto(ProductoModel Producto)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena1 = $"select * from Producto where CodigoBarras = @codigob except select * from Producto where CodigoProducto = '{Producto.codigoProducto}'";
                    SqlCommand cmd1 = new SqlCommand(cadena1, conn);
                    cmd1.Parameters.AddWithValue("@codigob", Producto.codigoBarras);
                    conn.Open();
                    using (SqlDataReader reader1 = cmd1.ExecuteReader())
                    {
                        if (reader1.HasRows)
                        {
                            conn.Close();
                            reader1.Close();
                            return false;
                            //return "Codigo de barras ya registrado.";
                        }
                    }
                    string cadena2 = $"select * from Producto where Nombre = @nombre except select * from Producto where CodigoProducto = '{Producto.codigoProducto}'";
                    SqlCommand cmd2 = new SqlCommand(cadena2, conn);
                    cmd2.Parameters.AddWithValue("@nombre", Producto.nombre);
                    using (SqlDataReader reader = cmd2.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            conn.Close();
                            reader.Close();
                            return false;
                            //return "Nombre de producto ya registrado."; 
                        }
                        else
                        {
                            reader.Close();
                            string cadena = $"UPDATE Producto SET  Nombre=@nombre, UnidadVenta=@univenta, PorcentajePromocion = @porcentajePromocion, UnidadCompra=@unicompra, PrecioVenta=@precio, Seccion=@seccion, FechaVencimiento=@fv, IVA=@iva, CodigoBarras=@cb, FactorConversion = @fc WHERE CodigoProducto = '{Producto.codigoProducto}' ";
                            SqlCommand cmd = new SqlCommand(cadena, conn);
                            cmd.Parameters.AddWithValue("@codigo", Statics.PrimeraAMayuscula(Producto.codigoProducto));
                            cmd.Parameters.AddWithValue("@nombre", Statics.PrimeraAMayuscula(Producto.nombre));
                            cmd.Parameters.AddWithValue("@univenta", Statics.PrimeraAMayuscula(Producto.unidadVenta));
                            cmd.Parameters.AddWithValue("@unicompra", Statics.PrimeraAMayuscula(Producto.unidadCompra));
                            cmd.Parameters.AddWithValue("@precio", Producto.precioVenta.ToString());
                            cmd.Parameters.AddWithValue("@seccion", Statics.PrimeraAMayuscula(Producto.seccion));
                            cmd.Parameters.AddWithValue("@fv", Producto.fechaVencimiento == DateTime.MinValue ? (object)DBNull.Value : Producto.fechaVencimiento);
                            cmd.Parameters.AddWithValue("@iva", Producto.iva);
                            cmd.Parameters.AddWithValue("@cb", string.IsNullOrEmpty(Producto.codigoBarras) ? (object)DBNull.Value : Producto.codigoBarras);
                            cmd.Parameters.AddWithValue("@fc", Producto.factorConversion);
                            cmd.Parameters.AddWithValue("@porcentajePromocion", Producto.porcentajePromocion == null ? (object)DBNull.Value : Producto.porcentajePromocion);
                            cmd.ExecuteNonQuery();
                            conn.Close();
                            return true;
                            //return "Producto actualizado.";
                        }
                    }
                }

            }
            catch (Exception)
            {
                return false;
                //Console.WriteLine(e.Message);
                //return e.Message;
            }
        }

        /// <summary>
        /// Elimina de la base de datos la informacion del producto con el ID dado como parametro.
        /// </summary>
        /// <param name="idProducto"></param>
        /// <returns></returns>
        public static bool deleteProducto(string idProducto)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"UPDATE Producto SET Estado = 'Inactivo' Where CodigoProducto = '{idProducto}' ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

        }

        /// <summary>
        /// Variable retornada por el metodo getProducots
        /// </summary>
        public static BindableCollection<ProductoModel> productos = new BindableCollection<ProductoModel>();

        /// <summary>
        /// Obtener Nombres y CodigoProducto de todos los productos en la base de datos.
        /// </summary>
        /// <returns></returns>
        public static BindableCollection<ProductoModel> getProductos()
        {

            productos.Clear();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = "SELECT *  FROM Producto where Estado = 'Activo' ORDER BY Nombre ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ProductoModel producto = new ProductoModel
                            {
                                codigoProducto = reader["CodigoProducto"].ToString(),
                                nombre = reader["Nombre"].ToString(),
                                unidadCompra = reader["UnidadCompra"].ToString(),
                                unidadVenta = reader["UnidadVenta"].ToString(),
                                seccion = reader["Seccion"].ToString(),
                                codigoBarras = reader["CodigoBarras"].ToString()
                            };
                            if (decimal.TryParse(reader["PrecioVenta"].ToString(), out decimal b)) { producto.precioVenta = b; } else { producto.precioVenta = null; }
                            if (decimal.TryParse(reader["IVA"].ToString(), out decimal c)) { producto.iva = c; } else { producto.iva = null; }
                            if (decimal.TryParse(reader["FactorConversion"].ToString(), out decimal d)) { producto.factorConversion = d; } else { producto.factorConversion = null; }
                            if (reader["FechaVencimiento"].ToString() == "") { producto.fechaVencimiento = DateTime.MinValue; } else { producto.fechaVencimiento = DateTime.Parse(reader["FechaVencimiento"].ToString()); }
                            productos.Add(producto);
                        }
                    }
                    conn.Close();
                    return productos;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }

        /// <summary>
        /// Devuielve los productos con coincidencias de los carateres dados como parametro y los nombres de los productos
        /// </summary>
        /// <param name="caracteres"></param>
        /// <returns></returns>
        public static BindableCollection<ProductoModel> getProductos(string caracteres)
        {

            productos.Clear();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"SELECT * FROM Producto where (Nombre like @caracteres or CodigoProducto like @caracteres)  AND Estado = 'Activo' ORDER BY Nombre ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@caracteres", "%"+ caracteres + "%");
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ProductoModel producto = new ProductoModel
                            {
                                codigoProducto = reader["CodigoProducto"].ToString(),
                                nombre = reader["Nombre"].ToString(),
                                unidadCompra = reader["UnidadCompra"].ToString(),
                                unidadVenta = reader["UnidadVenta"].ToString().Substring(0,3) + ".",
                                seccion = reader["Seccion"].ToString(),
                                codigoBarras = reader["CodigoBarras"].ToString()

                            };
                            if (decimal.TryParse(reader["PrecioVenta"].ToString(), out decimal precio)) { producto.precioVenta = precio; }
                            else { producto.precioVenta = null; }
                            if (decimal.TryParse(reader["IVA"].ToString(), out decimal iva)) { producto.iva = iva; }
                            else { producto.iva = null; }
                            if (decimal.TryParse(reader["FactorConversion"].ToString(), out decimal FactorConversion)) { producto.factorConversion = FactorConversion; }
                            else { producto.factorConversion = null; }
                            if (reader["FechaVencimiento"].ToString() == "") { producto.fechaVencimiento = DateTime.MinValue; }
                            else { producto.fechaVencimiento = DateTime.Parse(reader["FechaVencimiento"].ToString()); }
                            if (int.TryParse(reader["PorcentajePromocion"].ToString(), out int porcentajePromocion))
                            { 
                                producto.porcentajePromocion = porcentajePromocion;
                                producto.precioVentaConDescuento = decimal.Subtract((decimal)producto.precioVenta, decimal.Multiply((decimal)producto.precioVenta, ((decimal)producto.porcentajePromocion / 100)));
                            }
                            else { producto.porcentajePromocion = null; }
                            productos.Add(producto);
                        }
                    }
                    conn.Close();
                    return productos;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }

        /// <summary>
        /// Devuielve las unidades de venta y de compra de la lista de productos dados como parametros.
        /// </summary>
        /// <param name="caracteres"></param>
        /// <returns></returns>
        public static BindableCollection<ProductoModel> getUnidadVentaCompraProductos(BindableCollection<ProductoModel> productos)
        {

            productos.Clear();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {

                    foreach (ProductoModel producto in productos)
                    {
                        string cadena = $"SELECT *  FROM Producto where CodigoProducto = '{producto.codigoProducto}'";
                        SqlCommand cmd = new SqlCommand(cadena, conn);
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            producto.unidadCompra = reader["UnidadCompra"].ToString();
                            producto.unidadVenta = reader["UnidadVenta"].ToString();
                        }
                        conn.Close();
                    }

                    return productos;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }

        /// <summary>
        /// Actualiza en la base de datos el precio del producto pasado como parametro.
        /// </summary>
        /// <param name="Producto"></param>
        /// <returns></returns>
        public static bool actualizarPrecioProducto(ProductoModel Producto)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"UPDATE Producto SET  PrecioVenta=@precio, PorcentajePromocion = @porcentajePromocion  where CodigoProducto = @codigo";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigo", Producto.codigoProducto);
                    cmd.Parameters.AddWithValue("@precio", Producto.precioVenta);
                    cmd.Parameters.AddWithValue("@porcentajePromocion", Producto.porcentajePromocion == null ? (object)DBNull.Value : Producto.porcentajePromocion);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return true;
                }

            }
            catch (Exception e)
            {
               MessageBox.Show(e.Message);
                return false;
            }
        }

        #endregion

        #region Proveedor

        /// <summary>
        ///     Metodo encargado de ejecutar el query insert del nuevo proveedor en la base de datos.
        /// </summary>
        /// <param name="proveedor"></param>
        /// <returns></returns>
        public static bool NuevoProveedor(ProveedorModel proveedor)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = "INSERT INTO Proveedor(CedulaProveedor,Nombres,Apellidos,Telefono,Ciudad,Direccion, Estado) VALUES " +
                                                        "(@cedula,@nombre,@apellidos,@telefono,@ciudad,@direccion, 'Activo');";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@cedula", Statics.PrimeraAMayuscula(proveedor.cedula));
                    cmd.Parameters.AddWithValue("@nombre", Statics.PrimeraAMayuscula(proveedor.firstName));
                    cmd.Parameters.AddWithValue("@apellidos", Statics.PrimeraAMayuscula(proveedor.lastName));
                    cmd.Parameters.AddWithValue("@telefono", proveedor.telefono);
                    cmd.Parameters.AddWithValue("@ciudad", proveedor.ciudad);
                    cmd.Parameters.AddWithValue("@direccion", proveedor.direccion);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    string response = insertProductoProveedor(proveedor.cedula, proveedor.productos);
                    if (response == "Y")
                    {

                        //Registrar("Insert", "ServidorGetProveedores", proveedor.cedula, "ProveedorModel[]", "NuevoProducto");
                        conn.Close();
                        return true;
                        //return "Se ha registrado al nuevo proveedor.";
                    }
                    ///Si fallo la incersion de los productos se borra el registro del proveedor para que en un siguiente intento de registro no se repita la clave primaria.
                    string cadena0 = $"delete from Proveedor Where CedulaProveedor = '{proveedor.cedula}' ";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return false;
                    //return response;
                }
            }
            catch (Exception e)
            {
                if (e.Message.Substring(0, 50) == $"Violation of PRIMARY KEY constraint 'PK_Proveedor'")
                {
                    return false;
                    //return "Proveedor ya registrado.";
                }
                else
                {
                    return false;
                    //return e.Message;
                }
            }
        }

        /// <summary>
        /// Insertar las parejas Proveedor-Producto en la respectiva tabla de la base de datos.
        /// </summary>
        /// <param name="idProveedor"></param>
        /// <param name="productos"></param>
        /// <returns></returns>
        public static string insertProductoProveedor(string idProveedor, BindableCollection<ProductoModel> productos)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    ///Si la operacion fallo en un anterior intento. Para poder actualizar.
                    string cadena0 = $"delete from ProveedorProducto where CedulaProveedor = '{idProveedor}';";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.ExecuteNonQuery();
                    foreach (ProductoModel producto in productos)
                    {
                        string cadena = "INSERT INTO ProveedorProducto(CedulaProveedor,CodigoProducto) VALUES (@prov,@prod);";
                        SqlCommand cmd = new SqlCommand(cadena, conn);
                        cmd.Parameters.AddWithValue("@prov", idProveedor);
                        cmd.Parameters.AddWithValue("@prod", producto.codigoProducto);
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                    return "Y";
                }
            }
            catch (Exception e)
            {
                if (e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    return "Ya registrado.";
                }
                else
                {
                    return "server " + e.Message;
                }
            }
        }

        /// <summary>
        /// Method that does a select query against the 'Proveedor' table at the database and get the result of searching the coincidences into the cedula, nombre or apellido of the given characters.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public static BindableCollection<ProveedorModel> getProveedores(string Caracteres)
        {

            try
            {
                BindableCollection<ProveedorModel> proveedores = new BindableCollection<ProveedorModel>();
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"SELECT Distinct * FROM Proveedor WHERE (( CedulaProveedor like '%{Caracteres}%' ) or ( Nombres like '%{Caracteres}%' ) or ( Apellidos like '%{Caracteres}%' )) and Estado = 'Activo' ORDER BY Nombres ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        proveedores.Clear();
                        while (reader.Read())
                        {
                            ProveedorModel proveedor = new ProveedorModel
                            {
                                cedula = reader["CedulaProveedor"].ToString(),
                                firstName = reader["Nombres"].ToString(),
                                lastName = reader["Apellidos"].ToString(),
                                telefono = reader["Telefono"].ToString(),
                                ciudad = reader["Ciudad"].ToString(),
                                direccion = reader["Direccion"].ToString()
                            };
                            proveedores.Add(proveedor);
                        }
                    }
                    proveedores.Add(new ProveedorModel() { cedula = "-", firstName = "-", lastName = "-prove", ciudad = "separador" });

                    string cadena0 = $"SELECT Distinct Proveedor.CedulaProveedor,Proveedor.Nombres, Proveedor.Apellidos, Proveedor.Telefono, Proveedor.Ciudad, Proveedor.Direccion FROM proveedor  JOIN ProveedorProducto ON ProveedorProducto.CedulaProveedor = Proveedor.CedulaProveedor JOIN Producto ON ProveedorProducto.CodigoProducto = Producto.CodigoProducto WHERE Producto.Nombre LIKE '%{Caracteres}%';";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    using (SqlDataReader reader0 = cmd0.ExecuteReader())
                    {
                        while (reader0.Read())
                        {
                            ProveedorModel proveedor = new ProveedorModel
                            {
                                cedula = reader0["CedulaProveedor"].ToString(),
                                firstName = reader0["Nombres"].ToString(),
                                lastName = reader0["Apellidos"].ToString(),
                                telefono = reader0["Telefono"].ToString(),
                                ciudad = reader0["Ciudad"].ToString(),
                                direccion = reader0["Direccion"].ToString()
                            };
                            proveedores.Add(proveedor);
                        }
                    }

                    conn.Close();
                    return proveedores;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene la lista de todos los proveedores.
        /// </summary>
        /// <returns></returns>
        public static BindableCollection<ProveedorModel> getProveedores()
        {

            try
            {
                BindableCollection<ProveedorModel> proveedores = new BindableCollection<ProveedorModel>();
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"SELECT Distinct * FROM Proveedor where Estado = 'Activo' ORDER BY Nombres ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        proveedores.Clear();
                        while (reader.Read())
                        {
                            ProveedorModel proveedor = new ProveedorModel
                            {
                                cedula = reader["CedulaProveedor"].ToString(),
                                firstName = reader["Nombres"].ToString(),
                                lastName = reader["Apellidos"].ToString()
                            };
                            proveedores.Add(proveedor);
                        }
                    }

                    conn.Close();
                    return proveedores;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Retorna un proveedor especificamente daso su numero de cedula.
        /// </summary>
        /// <param name="Cedula"></param>
        /// <returns></returns>
        public static ProveedorModel getProveedor(string Cedula)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"SELECT Distinct * FROM Proveedor WHERE CedulaProveedor = '{Cedula}' AND Estado = 'Activo' ORDER BY Nombres";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    ProveedorModel proveedor = new ProveedorModel();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            proveedor.cedula = reader["CedulaProveedor"].ToString();
                            proveedor.firstName = reader["Nombres"].ToString();
                            proveedor.lastName = reader["Apellidos"].ToString();
                            proveedor.telefono = reader["Telefono"].ToString();
                            proveedor.ciudad = reader["Ciudad"].ToString();
                            proveedor.direccion = reader["Direccion"].ToString();
                        }
                    }

                    string cadena0 = $"select distinct * from Producto join ProveedorProducto on ProveedorProducto.CodigoProducto = Producto.CodigoProducto where ProveedorProducto.CedulaProveedor = '{Cedula}';";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    BindableCollection<ProductoModel> productos = new BindableCollection<ProductoModel>();
                    using (SqlDataReader reader0 = cmd0.ExecuteReader())
                    {
                        while (reader0.Read())
                        {

                            ProductoModel producto = new ProductoModel
                            {
                                codigoProducto = reader0["CodigoProducto"].ToString(),
                                nombre = reader0["Nombre"].ToString()
                            };
                            productos.Add(producto);

                        }
                    }
                    proveedor.productos = productos;
                    conn.Close();
                    return proveedor;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }


        /// <summary>
        /// Obtiene los proveedores
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public static BindableCollection<ProveedorModel> getNombresProveedores(string Caracteres)
        {

            try
            {
                BindableCollection<ProveedorModel> proveedores = new BindableCollection<ProveedorModel>();
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"SELECT Distinct * FROM Proveedor WHERE (( Nombres like '%{Caracteres}%' ) or ( Apellidos like '%{Caracteres}%' )) and Estado = 'Activo' ORDER BY Nombres ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();

                    using (SqlDataReader reader0 = cmd.ExecuteReader())
                    {
                        while (reader0.Read())
                        {
                            ProveedorModel proveedor = new ProveedorModel
                            {
                                cedula = reader0["CedulaProveedor"].ToString(),
                                firstName = reader0["Nombres"].ToString(),
                                lastName = reader0["Apellidos"].ToString(),
                                telefono = reader0["Telefono"].ToString(),
                                ciudad = reader0["Ciudad"].ToString(),
                                direccion = reader0["Direccion"].ToString()
                            };
                            proveedores.Add(proveedor);
                        }
                    }

                    conn.Close();
                    return proveedores;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        
        /// <summary>
        /// Elimina de la base de datos el proveedor con el número de cédula dado.
        /// </summary>
        /// <param name="cedula"></param>
        /// <returns></returns>
        public static bool deleteProveedor(string Cedula)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"UPDATE Proveedor SET Estado ='Inactivo' Where CedulaProveedor = '{Cedula}' ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return true;
                    //return "Se ha eliminado al proveedor.";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
                //return e.Message;
            }
        }

        /// <summary>
        /// Actualiza en la base de datos la informacion relacionada con el proveedor proporcionado como parametro,
        /// </summary>
        /// <param name="proveedor"></param>
        /// <returns></returns>
        public static bool actualizarProveedor(ProveedorModel proveedor)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = "UPDATE Proveedor SET Nombres=@nombre,Apellidos=@apellidos,Telefono=@telefono,Ciudad=@ciudad, Direccion=@direccion WHERE CedulaProveedor=@cedula;";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@cedula", proveedor.cedula);
                    cmd.Parameters.AddWithValue("@nombre", Statics.PrimeraAMayuscula(proveedor.firstName));
                    cmd.Parameters.AddWithValue("@apellidos", Statics.PrimeraAMayuscula(proveedor.lastName));
                    cmd.Parameters.AddWithValue("@telefono", proveedor.telefono);
                    cmd.Parameters.AddWithValue("@ciudad", proveedor.ciudad);
                    cmd.Parameters.AddWithValue("@direccion", proveedor.direccion);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    string response = insertProductoProveedor(proveedor.cedula, proveedor.productos);
                    if (response == "Y")
                    {
                        conn.Close();
                        return true;
                        // return "Se ha editado la informacion.";
                    }
                    return false;
                    //return response;
                }
            }
            catch (Exception e)
            {
                if (e.Message.Substring(0, 50) == $"Violation of PRIMARY KEY constraint 'PK_Proveedor'")
                {
                    return false;
                    //return "Proveedor ya registrado.";
                }
                else
                {
                    return false;
                    //return e.Message;
                }
            }
        }

        #endregion

        #region Usuario

        /// <summary>
        /// Metodo encargado de ejecutar el query insert del nuevo empleado en la base de datos local.
        /// </summary>
        /// <param name="Empleado">Instancia de la clase Empleado.</param>
        /// <returns></returns>
        public static bool NuevoUsuario(EmpleadoModel Empleado)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {

                    string cadena = "INSERT INTO Empleado(CedulaEmpleado,CodigoPuntoVenta,Nombres,Apellidos,FechaContratacion,Salario,Telefono,Cargo,Password,Direccion,Estado) VALUES " +
                                                        "(@cedula,@puntodeventa,@nombre,@apellidos,@fecha,@salario,@telefono,@cargo,@contraseña,@direccion,'Activo');";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@cedula", Empleado.cedula);
                    cmd.Parameters.AddWithValue("@puntodeventa", Empleado.puntoDeVenta.codigo);
                    cmd.Parameters.AddWithValue("@nombre", Statics.PrimeraAMayuscula(Empleado.firstName));
                    cmd.Parameters.AddWithValue("@apellidos", Statics.PrimeraAMayuscula(Empleado.lastName));
                    cmd.Parameters.AddWithValue("@fecha", Empleado.fechaDeContratacion.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@salario", Empleado.salario);
                    cmd.Parameters.AddWithValue("@telefono", Empleado.telefono);
                    cmd.Parameters.AddWithValue("@direccion", Empleado.direccion);
                    cmd.Parameters.AddWithValue("@cargo", Empleado.cargo);
                    cmd.Parameters.AddWithValue("@contraseña", Empleado.password);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    //MessageBox.Show($"Se ha registrado al nuevo usuario {Empleado.firstName} {Empleado.lastName}");
                    conn.Close();
                    return true;

                }
            }
            catch (Exception e)
            {

                if (e.Message.Substring(0, 50) == $"Violation of PRIMARY KEY constraint 'PK_Empleado'.")
                {
                    MessageBox.Show($"La cedula {Empleado.cedula} ya esta registrada.");
                    return false;
                }
                else
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// Actualiza el empleado del numero de cedula dado.
        /// </summary>
        /// <param name="Empleado"></param>
        /// <param name="CC"></param>
        /// <returns></returns>
        public static bool ActualizarUsuario(EmpleadoModel Empleado)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {


                    string cadena = "UPDATE Empleado SET CodigoPuntoVenta = @puntodeventa, Nombres = @nombre, Apellidos = @apellidos, FechaContratacion = @fecha, Salario =@salario, Telefono = @telefono, Cargo = @cargo," +
                                    "Password = @contraseña, Direccion = @direccion WHERE CedulaEmpleado = @NuevoCedula ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@NuevoCedula", Empleado.cedula);
                    cmd.Parameters.AddWithValue("@puntodeventa", Empleado.puntoDeVenta.codigo);
                    cmd.Parameters.AddWithValue("@nombre", Statics.PrimeraAMayuscula(Empleado.firstName));
                    cmd.Parameters.AddWithValue("@apellidos", Statics.PrimeraAMayuscula(Empleado.lastName));
                    cmd.Parameters.AddWithValue("@fecha", Empleado.fechaDeContratacion.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@salario", Empleado.salario);
                    cmd.Parameters.AddWithValue("@telefono", Empleado.telefono);
                    cmd.Parameters.AddWithValue("@direccion", Empleado.direccion);
                    cmd.Parameters.AddWithValue("@cargo", Empleado.cargo);
                    cmd.Parameters.AddWithValue("@contraseña", Statics.Hash(Empleado.password));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    //MessageBox.Show($"Se ha Actualizado el usuario {Empleado.firstName} {Empleado.lastName} aqui");
                    conn.Close();
                    return true;

                }
            }
            catch (Exception e)
            {

                if (e.Message.Substring(0, 50) == $"Violation of PRIMARY KEY constraint 'PK_Empleado'.")
                {
                    //MessageBox.Show($"La cedula {Empleado.cedula} ya esta registrada.");
                    return false;
                }
                else
                {
                    //MessageBox.Show(e.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// Elimina al empleado del número de cedúla dado.
        /// </summary>
        /// <param name="Cedula">Número de cédula del empleado a eliminar. </param>
        /// <returns></returns>
        public static bool deleteEmpleado(string Cedula)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {

                    string cadena = $"UPDATE empleado set Estado = 'Inactivo' Where CedulaEmpleado = '{Cedula}' ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }

        }

        /// <summary>
        /// Variable que retorna el metodo getEmpleados()
        /// </summary>
        public static BindableCollection<EmpleadoModel> emp = new BindableCollection<EmpleadoModel>();

        /// <summary>
        /// Method that does a select query against the 'Empleado' table at the local database and get the result of searching the coincidences into the cedula, nombre or apellido of the given characters.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public static BindableCollection<EmpleadoModel> getEmpleados(string Caracteres)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {

                    string cadena = $"SELECT Distinct * FROM EMPLEADO WHERE (( CedulaEmpleado like '%{Caracteres}%' ) or ( Nombres like '%{Caracteres}%' ) or ( Apellidos like '%{Caracteres}%' )) AND Estado = Activo ORDER BY Nombres;";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        emp.Clear();
                        while (reader.Read())
                        {
                            EmpleadoModel persona = new EmpleadoModel
                            {
                                cedula = reader["CedulaEmpleado"].ToString()
                            };
                            persona.puntoDeVenta.codigo = reader["CodigoPuntoVenta"].ToString();
                            persona.firstName = reader["Nombres"].ToString();
                            persona.lastName = reader["Apellidos"].ToString();
                            persona.fechaDeContratacion = DateTime.Parse(reader["FechaContratacion"].ToString());
                            persona.salario = Decimal.Parse(reader["Salario"].ToString());
                            persona.telefono = reader["Telefono"].ToString();
                            persona.cargo = reader["Cargo"].ToString();
                            persona.direccion = reader["Direccion"].ToString();
                            emp.Add(persona);
                        }
                    }
                    conn.Close();
                    return emp;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Variable que retorna los metodos getEmpleados y getAdministradires.
        /// </summary>
        public static BindableCollection<EmpleadoModel> empleados = new BindableCollection<EmpleadoModel>();

        /// <summary>
        /// Metodo que obtiene los empelados con cargo de "Administrador de sede"
        /// </summary>
        /// <returns></returns>
        public static BindableCollection<EmpleadoModel> getAdministradores()
        {
            EmpleadoModel empleado = new EmpleadoModel();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = "SELECT [CedulaEmpleado], [Nombres], [Apellidos]  FROM EMPLEADO where [Cargo]=@Admin AND Estado = 'Activo'";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@Admin", " Administrador de sede");
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            empleado.cedula = reader["CedulaEmpleado"].ToString();
                            empleado.firstName = reader["Nombres"].ToString();
                            empleado.lastName = reader["Apellidos"].ToString();
                            empleados.Add(empleado);
                        }
                    }
                    conn.Close();
                    return empleados;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }
        #endregion

        #region Locales

        /// <summary>
        /// Metodo que inserta la instancia de NuevoLocal en la base de datos local.
        /// </summary>
        /// <param name="NuevoLocal"></param>
        /// <returns></returns>
        public static bool NuevoLocal(LocalModel NuevoLocal)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {

                    string cadena = "INSERT INTO PuntoVenta(codigoPuntoVenta,Nombres,Direccion,Telefono,Ciudad,NumeroCanastillas,FechaDeApertura,Estado) " +
                                                   "VALUES (@codigo,@nombre,@direccion,@telefono,@ciudad,@nrocanastillas,@fechaapertura,'Activo')";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigo", NuevoLocal.codigo);
                    cmd.Parameters.AddWithValue("@nombre", Statics.PrimeraAMayuscula(NuevoLocal.nombre));
                    cmd.Parameters.AddWithValue("@direccion", Statics.PrimeraAMayuscula(NuevoLocal.direccion));
                    cmd.Parameters.AddWithValue("@telefono", Statics.PrimeraAMayuscula(NuevoLocal.telefono));
                    cmd.Parameters.AddWithValue("@ciudad", Statics.PrimeraAMayuscula(NuevoLocal.ciudad));
                    cmd.Parameters.AddWithValue("@nrocanastillas", Statics.PrimeraAMayuscula(NuevoLocal.numeroDeCanastillas.ToString()));
                    cmd.Parameters.AddWithValue("@fechaapertura", Statics.PrimeraAMayuscula(NuevoLocal.fechaDeApertura.Date.ToString("yyyy-MM-dd")));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    //MessageBox.Show($"Se ha registrado al nuevo local: {NuevoLocal.nombre }");
                    conn.Close();
                    return true;

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                if (e.Message.Substring(0, 52) == $"Violation of PRIMARY KEY constraint 'PK_PuntoVenta'.")
                {
                    MessageBox.Show($"El nombre {NuevoLocal.nombre} ya esta registrado.");
                    return false;
                }
                else
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// Actualiza la inforacion de un local en la base de datos.
        /// </summary>
        /// <param name="Local">Nuevo objeto de la clase LocalModel que se va a guardar en la base de datos. </param>
        /// <param name="IdAnterior">String del id del Local a actualizar.</param>
        /// <returns></returns>
        public static bool ActualizarLocal(LocalModel Local)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {

                    string cadena = "UPDATE PuntoVenta SET codigoPuntoVenta = @codigo, Nombres= @nombre, Direccion=@direccion, Telefono=@telefono, Ciudad=@ciudad, NumeroCanastillas=@nrocanastillas, FechaDeApertura=@fechaapertura WHERE codigoPuntoVenta=@codigo";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigo", Local.codigo);
                    cmd.Parameters.AddWithValue("@nombre", Statics.PrimeraAMayuscula(Local.nombre));
                    cmd.Parameters.AddWithValue("@direccion", Statics.PrimeraAMayuscula(Local.direccion));
                    cmd.Parameters.AddWithValue("@telefono", Statics.PrimeraAMayuscula(Local.telefono));
                    cmd.Parameters.AddWithValue("@ciudad", Statics.PrimeraAMayuscula(Local.ciudad));
                    cmd.Parameters.AddWithValue("@nrocanastillas", Statics.PrimeraAMayuscula(Local.numeroDeCanastillas.ToString()));
                    cmd.Parameters.AddWithValue("@fechaapertura", Statics.PrimeraAMayuscula(Local.fechaDeApertura.Date.ToString("yyyy-MM-dd")));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    //MessageBox.Show($"Se ha editado la informacion del local: {Local.nombre }");
                    conn.Close();
                    return true;

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                if (e.Message.Substring(0, 52) == $"Violation of PRIMARY KEY constraint 'PK_PuntoVenta'.")
                {
                    MessageBox.Show($"El nombre {Local.nombre} ya esta registrado.");
                    return false;
                }
                else
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// Elimina de la base de datos el punto de ventan del codigo dado.
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        public static bool deleteLocal(string Codigo)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {

                    string cadena = $"Update PuntoVenta SET Estado = 'Inactivo' Where CodigoPuntoVenta = '{Codigo}' ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }

        }

        /// <summary>
        /// Method that does a select query against the 'Local' table at the local database and get the result of searching the coincidences into the codigo o nombre.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public static BindableCollection<LocalModel> getLocales(string Caracteres)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {

                    string cadena = $"SELECT Distinct * FROM PuntoVenta WHERE (( CodigoPuntoVenta like '%{Caracteres}%' ) or ( Nombres like '%{Caracteres}%' )) AND Estado = 'Activo' ORDER BY Nombres ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        locales.Clear();
                        while (reader.Read())
                        {
                            LocalModel local = new LocalModel
                            {
                                codigo = reader["codigoPuntoVenta"].ToString(),
                                nombre = reader["Nombres"].ToString(),
                                direccion = reader["Direccion"].ToString(),
                                telefono = reader["Telefono"].ToString(),
                                ciudad = reader["Ciudad"].ToString(),
                                numeroDeCanastillas = Int32.Parse(reader["NumeroCanastillas"].ToString()),
                                fechaDeApertura = DateTime.Parse(reader["FechaDeApertura"].ToString())
                            };
                            locales.Add(local);
                        }
                    }
                    conn.Close();
                    return locales;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Variable que retorna el metodo getLocales().
        /// </summary>
        public static BindableCollection<LocalModel> locales = new BindableCollection<LocalModel>();

        /// <summary>
        /// Obtener Nombres y CodigoPuntoVenta de los locales en la base de datos.
        /// </summary>
        /// <returns></returns>
        public static BindableCollection<LocalModel> getLocales()
        {

            locales.Clear();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = "SELECT Distinct [CodigoPuntoVenta], [Nombres]  FROM PuntoVenta where Estado = 'Activo' ORDER BY Nombres";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            LocalModel local = new LocalModel
                            {
                                codigo = reader["CodigoPuntoVenta"].ToString(),
                                nombre = reader["Nombres"].ToString()
                            };
                            locales.Add(local);
                        }
                    }
                    conn.Close();
                    return locales;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }

        }

        #endregion

        #region Existencias

        /// <summary>
        /// Inserta el registro del nuevo documento de existencias.
        /// </summary>
        /// <param name="existencia"></param>
        /// <returns></returns>
        public static string NuevaExistencia(ExistenciasModel existencia)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"INSERT INTO ExistenciasFisicas(CodigoExistencia,CedulaEmpleado,CodigoPuntoVenta,FechaExistencia) VALUES (@codigo,@empleado,@pv,@fecha);";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigo", Statics.PrimeraAMayuscula(existencia.codigo));
                    cmd.Parameters.AddWithValue("@empleado", Statics.PrimeraAMayuscula(existencia.responsable.cedula));
                    cmd.Parameters.AddWithValue("@pv", Statics.PrimeraAMayuscula(existencia.puntoVenta.codigo));
                    cmd.Parameters.AddWithValue("@fecha", existencia.fecha.Date);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    string response = InsertarExistenciaProducto(existencia);
                    if (response == "Y")
                    {
                        conn.Close();
                        registrarCambioLocal(NombreMetodoLocal: "getExistenciasConProductos", PK: $"{existencia.codigo}", NombreMetodoServidor: "ServidorNuevaExistencia", RespuestaExitosaServidor: "Se ha registrado el nuevo documento.", Tipo: "Insert");
                        return "Se ha registrado el nuevo documento.";
                    }
                    /// Si algo fallo en la insercion de los productos y las cantidades se borra el registro del documento de existencias para que pueda ser exitoso en proximos intentos.
                    string cadena0 = $"delete from ExistenciasFisicas where CodigoExistencia = '{existencia.codigo}';";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.ExecuteNonQuery();
                    return response;
                }
            }
            catch (Exception e)
            {

                if (e.Message.Length > 35 && e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    return "Existencia ya registrada.";
                }
                else
                {
                    return e.Message;
                }
            }
        }

        /// <summary>
        /// Inserta el registro del nuevo documento de existencias.
        /// </summary>
        /// <param name="existencia"></param>
        /// <returns></returns>
        public static bool NuevaExistenciaBool(ExistenciasModel existencia)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"INSERT INTO ExistenciasFisicas(CodigoExistencia,CedulaEmpleado,CodigoPuntoVenta,FechaExistencia) VALUES (@codigo,@empleado,@pv,@fecha);";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigo", Statics.PrimeraAMayuscula(existencia.codigo));
                    cmd.Parameters.AddWithValue("@empleado", Statics.PrimeraAMayuscula(existencia.responsable.cedula));
                    cmd.Parameters.AddWithValue("@pv", Statics.PrimeraAMayuscula(existencia.puntoVenta.codigo));
                    cmd.Parameters.AddWithValue("@fecha", existencia.fecha.Date);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    string response = InsertarExistenciaProducto(existencia);
                    if (response == "Y")
                    {
                        conn.Close();
                        return true;
                    }
                    /// Si algo fallo en la insercion de los productos y las cantidades se borra el registro del documento de existencias para que pueda ser exitoso en proximos intentos.
                    string cadena0 = $"delete from ExistenciasFisicas where CodigoExistencia = '{existencia.codigo}';";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.ExecuteNonQuery();

                    return false;
                }
            }
            catch (Exception e)
            {
                if (e.Message.Length > 35 && e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    
                    ///Este metodo es llamado desde el servidor y la intancia ya ha sido guardada en la base de datos
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Inserta en la base de datos, en la tabla ExisteciaProducto los datos de dichos objetos.
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="productos"></param>
        /// <returns></returns>
        public static string InsertarExistenciaProducto(ExistenciasModel existencia)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    ///Si la operacion no termino exitosamente en un intento anterior, la realiza desde cero borrando los registros insertados en el anterior intento.
                    string cadena0 = $"delete from ExistenciaProducto where CodigoExistencia = '{existencia.codigo}';";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.ExecuteNonQuery();
                    foreach (ProductoModel producto in existencia.productos)
                    {
                        string cadena = "INSERT INTO ExistenciaProducto(CodigoExistencia,CodigoProducto, Cantidad) VALUES (@cod,@codProd,@existencia);";
                        SqlCommand cmd = new SqlCommand(cadena, conn);
                        cmd.Parameters.AddWithValue("@cod", existencia.codigo);
                        cmd.Parameters.AddWithValue("@codProd", producto.codigoProducto);
                        cmd.Parameters.AddWithValue("@existencia", producto.existencia);
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                    return "Y";
                }
            }
            catch (Exception e)
            {
                if (e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    return "Ya registrado.";
                }
                else
                {
                    return "Server " + e.Message;
                }
            }
        }

        /// <summary>
        /// Retorna las coincidencias en las existencias de los caracteres dados comparados con los codigos de las existencias y los codigos de los locales.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public static BindableCollection<ExistenciasModel> getExistencias(string Caracteres)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<ExistenciasModel> cExistencias = new BindableCollection<ExistenciasModel>();
                    DateTime fecha = new DateTime();
                    if (!DateTime.TryParse(Caracteres, out fecha))
                    {
                        fecha = DateTime.MinValue;
                    }

                    string cadena = $" select * from ExistenciasFisicas where FechaExistencia = '{fecha:yyyy-MM-dd}' or CodigoExistencia like '%{Caracteres}%'  or CodigoExistencia like '______________{Caracteres}' ORDER BY CodigoExistencia;";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cExistencias.Clear();
                        while (reader.Read())
                        {
                            ExistenciasModel exist = new ExistenciasModel
                            {
                                codigo = reader["CodigoExistencia"].ToString()
                            };
                            exist.responsable.cedula = reader["CedulaEmpleado"].ToString();
                            exist.fecha = DateTime.Parse(reader["FechaExistencia"].ToString());
                            exist.puntoVenta.codigo = reader["CodigoPuntoVenta"].ToString();
                            cExistencias.Add(exist);
                        }
                    }
                    conn.Close();
                    return cExistencias;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Devuelve el nombre, codigo y existencia de los productos relacionados con el codigo de existencia dado como parametro.
        /// </summary>
        /// <param name="codigoExistencia"></param>
        /// <returns></returns>
        public static BindableCollection<ProductoModel> getProductoExistencia(string codigoExistencia)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<ProductoModel> productos = new BindableCollection<ProductoModel>();
                    string cadena = $"select distinct producto.codigoproducto, producto.Nombre, producto.unidadventa, ExistenciaProducto.Cantidad from  producto join existenciaproducto on producto.codigoproducto = existenciaproducto.codigoproducto where existenciaproducto.codigoexistencia = '{codigoExistencia}' and producto.estado = 'Activo'; ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        productos.Clear();
                        while (reader.Read())
                        {
                            ProductoModel producto = new ProductoModel
                            {
                                codigoProducto = reader["codigoproducto"].ToString(),
                                nombre = reader["nombre"].ToString(),
                                unidadVenta = reader["unidadventa"].ToString(),
                                existencia = decimal.Parse(reader["cantidad"].ToString())
                            };

                            productos.Add(producto);
                        }
                    }
                    conn.Close();
                    return productos;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }

        /// <summary>
        /// Devuelve la instancia de las existencias junto con los productos relacionados con el codigo de existencia dado como parametro.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public static BindableCollection<ExistenciasModel> getExistenciasConProductos(string Caracteres)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<ExistenciasModel> cExistencias = new BindableCollection<ExistenciasModel>();
                    string cadena = $" select * from ExistenciasFisicas where CodigoExistencia = '{Caracteres}' ORDER BY CodigoExistencia;";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cExistencias.Clear();
                        while (reader.Read())
                        {
                            ExistenciasModel exist = new ExistenciasModel
                            {
                                codigo = reader["CodigoExistencia"].ToString()
                            };
                            exist.responsable.cedula = reader["CedulaEmpleado"].ToString();
                            exist.fecha = DateTime.Parse(reader["FechaExistencia"].ToString());
                            exist.puntoVenta.codigo = reader["CodigoPuntoVenta"].ToString();
                            exist.productos = getProductoExistencia(exist.codigo);
                            cExistencias.Add(exist);
                        }
                    }
                    conn.Close();
                    return cExistencias;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Retorna todos los registros de documentos de existencias en la base de datos.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public static BindableCollection<ExistenciasModel> getTodasLasExistencias()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<ExistenciasModel> cExistencias = new BindableCollection<ExistenciasModel>();
                    string cadena = $"select CodigoExistencia,existenciasfisicas.CedulaEmpleado,existenciasfisicas.CodigoPuntoVenta,FechaExistencia,Nombres,Apellidos from ExistenciasFisicas join Empleado on ExistenciasFisicas.CedulaEmpleado = Empleado.CedulaEmpleado";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cExistencias.Clear();
                        while (reader.Read())
                        {
                            ExistenciasModel exist = new ExistenciasModel
                            {
                                codigo = reader["CodigoExistencia"].ToString()
                            };
                            exist.responsable.cedula = reader["CedulaEmpleado"].ToString();
                            exist.responsable.firstName = reader["Nombres"].ToString();
                            exist.responsable.lastName = reader["Apellidos"].ToString();
                            exist.fecha = DateTime.Parse(reader["FechaExistencia"].ToString());
                            exist.puntoVenta.codigo = reader["CodigoPuntoVenta"].ToString();
                            cExistencias.Add(exist);
                        }
                    }
                    conn.Close();
                    return cExistencias;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        #endregion

        #region Pedido
        /// <summary>
        /// Inserta el registro del nuevo documento de pedido.
        /// </summary>
        /// <param name="pedido"></param>
        /// <returns></returns>
        public static string NuevoPedido(PedidoModel pedido)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"INSERT INTO Pedido(CodigoPedido,CedulaEmpleado,CodigoPuntoVenta,FechaPedido) VALUES (@codigo,@empleado,@pv,@fecha);";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigo", Statics.PrimeraAMayuscula(pedido.codigo));
                    cmd.Parameters.AddWithValue("@empleado", Statics.PrimeraAMayuscula(pedido.responsable.cedula));
                    cmd.Parameters.AddWithValue("@pv", Statics.PrimeraAMayuscula(pedido.puntoVenta.codigo));
                    cmd.Parameters.AddWithValue("@fecha", pedido.fecha.Date);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    string response = InsertarPedidoProducto(pedido);
                    if (response == "Y")
                    {
                        conn.Close();
                        registrarCambioLocal(Tipo: "Insert", NombreMetodoLocal: "getPedidoConProductos", PK: $"{pedido.codigo}", NombreMetodoServidor: "ServidorNuevoPedido", RespuestaExitosaServidor: "Se ha registrado el nuevo documento.");
                        return "Se ha registrado el nuevo documento.";
                    }
                    /// Si algo fallo en la insercion de los productos y las cantidades se borra el registro del documento de existencias para que pueda ser exitoso en proximos intentos.
                    string cadena0 = $"delete from Pedido where CodigoPedido = '{pedido.codigo}';";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.ExecuteNonQuery();
                    return response;
                }
            }
            catch (Exception e)
            {

                if (e.Message.Length > 35 && e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    return "Pedido ya registrado.";
                }
                else
                {
                    return e.Message;
                }
            }
        }

        /// <summary>
        /// Inserta el registro del nuevo documento de existencias.
        /// </summary>
        /// <param name="pedido"></param>
        /// <returns></returns>
        public static bool NuevoPedidoBool(PedidoModel pedido)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"INSERT INTO Pedido(CodigoPedido,CedulaEmpleado,CodigoPuntoVenta,FechaPedido) VALUES (@codigo,@empleado,@pv,@fecha);";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigo", Statics.PrimeraAMayuscula(pedido.codigo));
                    cmd.Parameters.AddWithValue("@empleado", Statics.PrimeraAMayuscula(pedido.responsable.cedula));
                    cmd.Parameters.AddWithValue("@pv", Statics.PrimeraAMayuscula(pedido.puntoVenta.codigo));
                    cmd.Parameters.AddWithValue("@fecha", pedido.fecha.Date);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    string response = InsertarPedidoProducto(pedido);
                    if (response == "Y")
                    {
                        conn.Close();
                        return true;
                    }
                    /// Si algo fallo en la insercion de los productos y las cantidades se borra el registro del documento de existencias para que pueda ser exitoso en proximos intentos.
                    string cadena0 = $"delete from Pedido where CodigoPedido = '{pedido.codigo}';";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.ExecuteNonQuery();

                    return false;
                }
            }
            catch (Exception e)
            {
                if (e.Message.Length > 35 && e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    ///El pedido ya esta registrado cuado este metodo es llamado desde el servidor
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Inserta en la base de datos, en la tabla PedidoProducto los datos de dichos objetos.
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="productos"></param>
        /// <returns></returns>
        public static string InsertarPedidoProducto(PedidoModel pedido)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    ///Si la operacion no termino exitosamente en un intento anterior, la realiza desde cero borrando los registros insertados en el anterior intento.
                    string cadena0 = $"delete from PedidoProducto where CodigoPedido = '{pedido.codigo}';";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.ExecuteNonQuery();
                    foreach (ProductoModel producto in pedido.productos)
                    {
                        string cadena = "INSERT INTO PedidoProducto(CodigoPedido,CodigoProducto, Cantidad) VALUES (@cod,@codProd,@pedido);";
                        SqlCommand cmd = new SqlCommand(cadena, conn);
                        cmd.Parameters.AddWithValue("@cod", pedido.codigo);
                        cmd.Parameters.AddWithValue("@codProd", producto.codigoProducto);
                        cmd.Parameters.AddWithValue("@pedido", producto.pedido);
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                    return "Y";
                }
            }
            catch (Exception e)
            {
                if (e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    return "Ya registrado.";
                }
                else
                {
                    return "Server " + e.Message;
                }
            }
        }

        /// <summary>
        /// Retorna las coincidencias en los pedidos con de los caracteres dados comparados con los codigos de los pedidos y los codigos de los locales.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public static BindableCollection<PedidoModel> getPedidos(string Caracteres)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<PedidoModel> cPedidos = new BindableCollection<PedidoModel>();
                    DateTime fecha = new DateTime();
                    if (!DateTime.TryParse(Caracteres, out fecha))
                    {
                        fecha = DateTime.MinValue;
                    }

                    string cadena = $" select * from Pedido where FechaPedido = '{fecha:yyyy-MM-dd}' or CodigoPedido like '%{Caracteres}%'  or CodigoPedido like '______________{Caracteres}' ORDER BY CodigoPedido;";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cPedidos.Clear();
                        while (reader.Read())
                        {
                            PedidoModel ped = new PedidoModel
                            {
                                codigo = reader["CodigoPedido"].ToString()
                            };
                            ped.responsable.cedula = reader["CedulaEmpleado"].ToString();
                            ped.fecha = DateTime.Parse(reader["FechaPedido"].ToString());
                            ped.puntoVenta.codigo = reader["CodigoPuntoVenta"].ToString();
                            cPedidos.Add(ped);
                        }
                    }
                    conn.Close();
                    return cPedidos;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Devuelve el nombre, codigo y pedido de los productos relacionados con el codigo del pedido dado como parametro.
        /// </summary>
        /// <param name="codigoPedido"></param>
        /// <returns></returns>
        public static BindableCollection<ProductoModel> getProductoPedido(string codigoPedido)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<ProductoModel> productos = new BindableCollection<ProductoModel>();
                    string cadena = $"select  producto.codigoproducto, producto.Nombre, producto.unidadventa, producto.unidadcompra, PedidoProducto.Cantidad, PedidoProducto.CantidadEnviada, producto.factorconversion,ExistenciaProducto.Cantidad as ExistenciaCantidad from  producto join pedidoproducto on producto.codigoproducto = PedidoProducto.CodigoProducto join ExistenciaProducto on ExistenciaProducto.CodigoProducto = Producto.CodigoProducto where pedidoproducto.codigopedido = '{codigoPedido}' and CodigoExistencia = '{codigoPedido.Split(':')[0]}' and estado = 'Activo' order by ExistenciaProducto.CodigoProducto; ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        productos.Clear();
                        while (reader.Read())
                        {
                            ProductoModel producto = new ProductoModel
                            {
                                codigoProducto = reader["codigoproducto"].ToString(),
                                nombre = reader["nombre"].ToString(),
                                unidadVenta = reader["unidadventa"].ToString(),
                                unidadCompra = reader["unidadcompra"].ToString(),
                                factorConversion = decimal.Parse(reader["factorconversion"].ToString()),
                                existencia = decimal.Parse(reader["ExistenciaCantidad"].ToString()),
                                pedido = decimal.Parse(reader["cantidad"].ToString()),

                            };
                            //Int32.TryParse(reader["CantidadEnviada"].ToString(), out int a);
                            //producto.compraPorLocal = a;
                            productos.Add(producto);
                        }
                    }
                    conn.Close();
                    return productos;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }

        /// <summary>
        /// Devuelve la instancia del pedido junto con los productos relacionados con el codigo de pedido dado como parametro.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public static BindableCollection<PedidoModel> getPedidoConProductos(string Caracteres)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<PedidoModel> cPedido = new BindableCollection<PedidoModel>();
                    string cadena = $" select * from Pedido where CodigoPedido = '{Caracteres}' ORDER BY CodigoPedido;";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cPedido.Clear();
                        while (reader.Read())
                        {
                            PedidoModel ped = new PedidoModel
                            {
                                codigo = reader["CodigoPedido"].ToString()
                            };
                            ped.responsable.cedula = reader["CedulaEmpleado"].ToString();
                            ped.fecha = DateTime.Parse(reader["FechaPedido"].ToString());
                            ped.puntoVenta.codigo = reader["CodigoPuntoVenta"].ToString();
                            ped.productos = getProductoPedido(ped.codigo);
                            cPedido.Add(ped);
                        }
                    }
                    conn.Close();
                    return cPedido;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Devuelve todas las instancias de pedidos registradas en la base de datos.
        /// </summary>
        /// <returns></returns>
        public static BindableCollection<PedidoModel> getTodoPedido()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<PedidoModel> cPedido = new BindableCollection<PedidoModel>();
                    string cadena = $" select CodigoPedido,Pedido.CedulaEmpleado,Pedido.CodigoPuntoVenta,FechaPedido,empleado.Nombres,Apellidos, PuntoVenta.Nombres as name from Pedido join Empleado on Pedido.CedulaEmpleado = Empleado.CedulaEmpleado join PuntoVenta on Pedido.CodigoPuntoVenta = PuntoVenta.CodigoPuntoVenta ORDER BY CodigoPedido;";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cPedido.Clear();
                        while (reader.Read())
                        {
                            PedidoModel ped = new PedidoModel
                            {
                                codigo = reader["CodigoPedido"].ToString()
                            };
                            ped.responsable.cedula = reader["CedulaEmpleado"].ToString();
                            ped.responsable.firstName = reader["Nombres"].ToString();
                            ped.responsable.lastName = reader["Apellidos"].ToString();
                            ped.fecha = DateTime.Parse(reader["FechaPedido"].ToString());
                            ped.puntoVenta.codigo = reader["CodigoPuntoVenta"].ToString();
                            ped.puntoVenta.nombre = reader["name"].ToString();

                            cPedido.Add(ped);
                        }
                    }
                    conn.Close();
                    return cPedido;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        #endregion

        #region Compras

        /// <summary>
        /// Inserta el registro del nuevo documento de compra.
        /// </summary>
        /// <param name="compra"></param>
        /// <returns></returns>
        public static bool NuevaCompraBool(ComprasModel compra)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"INSERT INTO Compras(CodigoCompra,CedulaEmpleado,FechaCompra) VALUES (@codigo,@empleado,@fecha);";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigo", compra.codigo);
                    cmd.Parameters.AddWithValue("@empleado", compra.responsable.cedula);
                    cmd.Parameters.AddWithValue("@fecha", compra.fecha.Date);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    ///Se registra aqui la nueva compra pues esto debe al orden en que el servidor hara las inserciones
                    registrarCambioLocal(Tipo: "Insert", NombreMetodoLocal: "getComprasConProductos", PK: $"{compra.codigo}", NombreMetodoServidor: "ServidorNuevaCompra", RespuestaExitosaServidor: "true");
                    string response = InsertarRegistroCompraProducto(compra);
                    string rta = InsertarPedidosCompra(compra);
                    if (response == "Y" && rta == "Y")
                    {
                        conn.Close();
                        return true;
                    }
                    /// Si algo fallo en la insercion de los productos y las cantidades se borra el registro del documento de compra para que pueda ser exitoso en proximos intentos.
                    string cadena0 = $"delete from Compras where CodigoCompra = '{compra.codigo}';";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.ExecuteNonQuery();

                    return false;
                }
            }
            catch (Exception e)
            {
                if (e.Message.Length > 35 && e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Inserta el registro del nuevo documento de compra, este metodo es necesario para el uso desde el servidor para evitar redundacia.
        /// </summary>
        /// <param name="compra"></param>
        /// <returns></returns>
        public static bool NuevaCompraBoolServidor(ComprasModel compra)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"INSERT INTO Compras(CodigoCompra,CedulaEmpleado,FechaCompra) VALUES (@codigo,@empleado,@fecha);";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigo", compra.codigo);
                    cmd.Parameters.AddWithValue("@empleado", compra.responsable.cedula);
                    cmd.Parameters.AddWithValue("@fecha", compra.fecha.Date);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    string response = InsertarRegistroCompraProducto(compra);
                    string rta = InsertarPedidosCompra(compra);
                    if (response == "Y" && rta == "Y")
                    {
                        conn.Close();
                        return true;
                    }
                    /// Si algo fallo en la insercion de los productos y las cantidades se borra el registro del documento de compra para que pueda ser exitoso en proximos intentos.
                    string cadena0 = $"delete from Compras where CodigoCompra = '{compra.codigo}';";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.ExecuteNonQuery();

                    return false;
                }
            }
            catch (Exception e)
            {
                if (e.Message.Length > 35 && e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    ///El metodo es llamado desde el servidro pero la compra ya ha sido registrada
                    MessageBox.Show("No se insero compra repetida");
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Inserta en la base de datos, en la tabla RegistroCompra los datos de dicho documento.
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="productos"></param>
        /// <returns></returns>
        public static string InsertarRegistroCompraProducto(ComprasModel compra)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    ///Si la operacion no termino exitosamente en un intento anterior, la realiza desde cero borrando los registros insertados en el anterior intento.
                    string cadena0 = $"delete from RegistroCompra where CodigoCompra = '{compra.codigo}';";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.ExecuteNonQuery();
                    foreach (ProductoModel producto in compra.sumaPedidos)
                    {
                        string cadena = "INSERT INTO RegistroCompra(CodigoCompra,CodigoProducto, Pedido,Estado) VALUES (@cod,@codProd, @pedido,'Pendiente' );";
                        SqlCommand cmd = new SqlCommand(cadena, conn);
                        cmd.Parameters.AddWithValue("@cod", compra.codigo);
                        cmd.Parameters.AddWithValue("@codProd", producto.codigoProducto);
                        cmd.Parameters.AddWithValue("@pedido", producto.sumaPedido);
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                    return "Y";
                }
            }
            catch (Exception e)
            {
                if (e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    return "Ya registrado.";
                }
                else
                {
                    return "Server " + e.Message;
                }
            }
        }

        /// <summary>
        /// Inserta en la base de datos, en la tabla CompraPedido la relacion entre un documento de compra y los pedidos que la componen.
        /// </summary>
        /// <param name="compra"></param>
        /// <returns></returns>
        public static string InsertarPedidosCompra(ComprasModel compra)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    ///Si la operacion no termino exitosamente en un intento anterior, la realiza desde cero borrando los registros insertados en el anterior intento.
                    string cadena0 = $"delete from CompraPedido where CodigoCompra = '{compra.codigo}';";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.ExecuteNonQuery();
                    foreach (string codigo in compra.codPedidos)
                    {
                        string cadena = "INSERT INTO CompraPedido(CodigoCompra,CodigoPedido) VALUES (@cod,@codPed);";
                        SqlCommand cmd = new SqlCommand(cadena, conn);
                        cmd.Parameters.AddWithValue("@cod", compra.codigo);
                        cmd.Parameters.AddWithValue("@codPed", codigo);

                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                    return "Y";
                }
            }
            catch (Exception e)
            {
                if (e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    return "Ya registrado.";
                }
                else
                {
                    return "Server " + e.Message;
                }
            }
        }

        /// <summary>
        /// Actualiza la informacion del registro de compra del producto dado como parametro.
        /// </summary>
        /// <param name="producto"></param>
        /// <returns></returns>
        public static bool UpdateRegistroCompra(ComprasModel compra)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    string cadena = $"UPDATE RegistroCompra SET CedulaProveedor = @prov, CantidadComprada = @cant, PrecioCompra= @precio  WHERE CodigoCompra='{compra.codigo}' and CodigoProducto = '{compra.productos[0].codigoProducto}'";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@prov", string.IsNullOrEmpty(compra.productos[0].proveedor.cedula) ? (object)DBNull.Value : compra.productos[0].proveedor.cedula);
                    cmd.Parameters.AddWithValue("@cant", string.IsNullOrEmpty(compra.productos[0].compra.ToString()) ? (object)DBNull.Value : compra.productos[0].compra);
                    cmd.Parameters.AddWithValue("@precio", string.IsNullOrEmpty(compra.productos[0].precioCompra.ToString()) ? (object)DBNull.Value : compra.productos[0].precioCompra);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    if (compra.codigo != null && compra.productos[0].codigoProducto != null)
                    {
                        registrarCambioLocal(Tipo: "Update", NombreMetodoLocal: "getRegistroCompra", PK: $"{compra.codigo}+{compra.productos[0].codigoProducto}", NombreMetodoServidor: "ServidorUpdateRegistroCompra", RespuestaExitosaServidor: "true");
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                return false;
            }
        }

        /// <summary>
        /// Actualiza la informacion del registro de compra del producto dado como parametro, este metodo es necesario para ser ejecutado desde el servidor.
        /// </summary>
        /// <param name="producto"></param>
        /// <returns></returns>
        public static bool UpdateRegistroCompraServidor(ProductoModel producto)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    string cadena = $"UPDATE RegistroCompra SET CedulaProveedor = @prov, CantidadComprada = @cant, PrecioCompra= @precio  WHERE CodigoCompra='{producto.codigoProducto.Split('+')[0]}' and CodigoProducto = '{producto.codigoProducto.Split('+')[1]}'";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@prov", string.IsNullOrEmpty(producto.proveedor.cedula) ? (object)DBNull.Value : producto.proveedor.cedula);
                    cmd.Parameters.AddWithValue("@cant", string.IsNullOrEmpty(producto.compra.ToString()) ? (object)DBNull.Value : producto.compra);
                    cmd.Parameters.AddWithValue("@precio", string.IsNullOrEmpty(producto.precioCompra.ToString()) ? (object)DBNull.Value : producto.precioCompra);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                return false;
            }
        }

        /// <summary>
        /// Retorna las coincidencias en las compras con de los caracteres dados comparados con los codigos de los compra o la fecha.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public static BindableCollection<ComprasModel> getCompras(string Caracteres)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<ComprasModel> cCompras = new BindableCollection<ComprasModel>();
                    DateTime fecha = new DateTime();
                    if (!DateTime.TryParse(Caracteres, out fecha))
                    {
                        fecha = DateTime.MinValue;
                    }

                    string cadena = $" select CodigoCompra, FechaCompra,NumeroCanastillas,peso,Empleado.CedulaEmpleado, Nombres, Apellidos from Compras join empleado on compras.CedulaEmpleado = Empleado.CedulaEmpleado where FechaCompra = '{fecha:yyyy-MM-dd}' or CodigoCompra like '%{Caracteres}%' ORDER BY CodigoCompra;";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cCompras.Clear();
                        while (reader.Read())
                        {
                            ComprasModel comp = new ComprasModel
                            {
                                codigo = reader["CodigoCompra"].ToString()
                            };
                            comp.responsable.cedula = reader["CedulaEmpleado"].ToString();
                            comp.responsable.firstName = reader["Nombres"].ToString();
                            comp.responsable.lastName = reader["Apellidos"].ToString();
                            comp.fecha = DateTime.Parse(reader["FechaCompra"].ToString());
                            Int32.TryParse(reader["NumeroCanastillas"].ToString(), out int i);
                            comp.numeroCanastillas = i;
                            decimal.TryParse(reader["peso"].ToString(), out decimal a);
                            comp.codPedidos = getPedidosCompra(comp.codigo);
                            comp.peso = a;

                            cCompras.Add(comp);
                        }
                    }
                    conn.Close();
                    return cCompras;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Retorna la compra del codigo dado como parametro, la informacion de los registros de compra y los documentos de pedido que conforman el documento de compra tambien es obtenida.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public static BindableCollection<ComprasModel> getComprasConProductos(string Caracteres)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<ComprasModel> cCompras = new BindableCollection<ComprasModel>();
                    DateTime fecha = new DateTime();
                    if (!DateTime.TryParse(Caracteres, out fecha))
                    {
                        fecha = DateTime.MinValue;
                    }

                    string cadena = $" select CodigoCompra, FechaCompra,NumeroCanastillas,peso,Empleado.CedulaEmpleado, Nombres, Apellidos from Compras join empleado on compras.CedulaEmpleado = Empleado.CedulaEmpleado where  CodigoCompra = '{Caracteres}';";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cCompras.Clear();
                        while (reader.Read())
                        {
                            ComprasModel comp = new ComprasModel
                            {
                                codigo = reader["CodigoCompra"].ToString()
                            };
                            comp.responsable.cedula = reader["CedulaEmpleado"].ToString();
                            comp.responsable.firstName = reader["Nombres"].ToString();
                            comp.responsable.lastName = reader["Apellidos"].ToString();
                            comp.fecha = DateTime.Parse(reader["FechaCompra"].ToString());
                            Int32.TryParse(reader["NumeroCanastillas"].ToString(), out int i);
                            comp.numeroCanastillas = i;
                            decimal.TryParse(reader["peso"].ToString(), out decimal a);
                            comp.peso = a;
                            comp.sumaPedidos = getProductoCompra(Caracteres);
                            comp.codPedidos = getPedidosCompra(Caracteres);
                            cCompras.Add(comp);
                        }
                    }
                    conn.Close();

                    return cCompras;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Devuelve el nombre, codigo y suma  de los productos relacionados con el codigo del compra dado como parametro.
        /// </summary>
        /// <param name="codigoCompra"></param>
        /// <returns></returns>
        public static BindableCollection<ProductoModel> getProductoCompra(string codigoCompra)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<ProductoModel> productos = new BindableCollection<ProductoModel>();
                    string cadena = $"select producto.codigoproducto, producto.Nombre, producto.unidadcompra,  pedido, CantidadComprada, PrecioCompra, RegistroCompra.CedulaProveedor, proveedor.Nombres, Apellidos  from RegistroCompra  join Producto on Producto.CodigoProducto = RegistroCompra.CodigoProducto left join Proveedor on RegistroCompra.CedulaProveedor = Proveedor.CedulaProveedor where CodigoCompra = '{codigoCompra}' and producto.Estado = 'Activo' order by codigoproducto ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        productos.Clear();
                        while (reader.Read())
                        {
                            ProductoModel producto = new ProductoModel
                            {
                                codigoProducto = reader["codigoproducto"].ToString(),
                                nombre = reader["nombre"].ToString(),
                                unidadCompra = reader["unidadcompra"].ToString(),
                                sumaPedido = decimal.Parse(reader["pedido"].ToString())
                            };

                            if (decimal.TryParse(reader["CantidadComprada"].ToString(), out decimal a))
                            {
                                producto.compra = a;
                            }
                            else
                            {
                                producto.compra = null;
                            }
                            if (decimal.TryParse(reader["PrecioCompra"].ToString(), out decimal b))
                            {
                                producto.precioCompra = b;
                            }
                            else
                            {
                                producto.precioCompra = null;
                            }
                            producto.proveedor.cedula = reader["CedulaProveedor"].ToString();
                            producto.proveedor.firstName = reader["Nombres"].ToString();
                            producto.proveedor.lastName = reader["Apellidos"].ToString();
                            productos.Add(producto);
                        }
                    }
                    conn.Close();
                    return productos;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }

        /// <summary>
        /// Devuelve el nombre, codigo y suma  de los productos relacionados con el codigo del compra dado como parametro.
        /// </summary>
        /// <param name="codigoCompra"></param>
        /// <returns></returns>
        public static BindableCollection<string> getPedidosCompra(string codigoCompra)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<string> pedidosCodPedidos = new BindableCollection<string>();
                    string cadena = $"select codigopedido from CompraPedido where CodigoCompra = '{codigoCompra}'";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        pedidosCodPedidos.Clear();
                        while (reader.Read())
                        {
                            pedidosCodPedidos.Add(reader["codigopedido"].ToString());

                        }
                    }
                    conn.Close();
                    return pedidosCodPedidos;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }

        /// <summary>
        /// Devuelve una instancia de la clase ComprasModel con un unico producto en la propiedad productos que contiene la informacion relacionada con el registro de la compra del producto dado como parametro.
        /// </summary>
        /// <param name="codigoCompra"></param>
        /// <returns></returns>
        public static BindableCollection<ComprasModel> getRegistroCompra(string codigoCompraCodigoProducto)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<ComprasModel> compras = new BindableCollection<ComprasModel>();
                    string cadena = $"select * from RegistroCompra where CodigoCompra = '{codigoCompraCodigoProducto.Split("+")[0]}' and CodigoProducto = '{codigoCompraCodigoProducto.Split("+")[1]}'";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        compras.Clear();
                        while (reader.Read())
                        {

                            ComprasModel compra = new ComprasModel { codigo = reader["CodigoCompra"].ToString() };
                            ProductoModel producto = new ProductoModel { codigoProducto = reader["CodigoProducto"].ToString() };
                            if (decimal.TryParse(reader["CantidadComprada"].ToString(), out decimal a)) { producto.compra = a; }
                            else { producto.compra = null; }
                            if (decimal.TryParse(reader["PrecioCompra"].ToString(), out decimal b)) { producto.precioCompra = b; }
                            else { producto.precioCompra = null; }
                            producto.proveedor.cedula = reader["CedulaProveedor"].ToString();
                            compra.productos.Add(producto);
                            compras.Add(compra);
                        }
                    }
                    conn.Close();
                    return compras;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }

        /// <summary>
        /// Retorna una lista de objetos de la clase ProductoModel con los registros de compra del proveedor o el producto dado como parametro.
        /// /// </summary>
        /// <param name="codigoProductoCedula"></param>
        /// <returns></returns>
        public static BindableCollection<ProductoModel> getRegistroCompraCodigoCedula(string codigoProductoCedula)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<ProductoModel> productos = new BindableCollection<ProductoModel>();
                    string cadena = $"select  RegistroCompra.CodigoCompra, Producto.CodigoProducto, Proveedor.CedulaProveedor, RegistroCompra.CantidadComprada, RegistroCompra.PrecioCompra, Producto.UnidadCompra, compras.fechacompra, RegistroCompra.FechaPagado, RegistroCompra.Soporte, Proveedor.Nombres, Proveedor.Apellidos, Producto.Nombre from RegistroCompra left join Proveedor on RegistroCompra.CedulaProveedor = Proveedor.CedulaProveedor left join Producto on RegistroCompra.CodigoProducto = Producto.CodigoProducto left join Compras on compras.codigocompra = registrocompra.codigocompra where (Proveedor.CedulaProveedor = '{codigoProductoCedula}' or Producto.CodigoProducto = '{codigoProductoCedula}') and registrocompra.Estado='Pendiente'";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        productos.Clear();
                        while (reader.Read())
                        {
                            ProductoModel producto = new ProductoModel
                            {
                                codigoProducto = reader["CodigoProducto"].ToString(),
                                nombre = reader["Nombre"].ToString()
                            };
                            producto.codigoCompra = reader["CodigoCompra"].ToString();
                            producto.proveedor.cedula = reader["CedulaProveedor"].ToString();
                            producto.proveedor.firstName = reader["Nombres"].ToString();
                            producto.proveedor.lastName = reader["Apellidos"].ToString();
                            producto.unidadCompra = reader["UnidadCompra"].ToString().Substring(0, 3);
                            if (DateTime.TryParse(reader["fechaCompra"].ToString(), out DateTime fecha)) { producto.fechaDeCompra = fecha; }
                            if (DateTime.TryParse(reader["FechaPagado"].ToString(), out DateTime date))
                            {
                                producto.fechaDePago = date;
                            }

                            producto.soportePago = reader["Soporte"].ToString();
                            if (decimal.TryParse(reader["CantidadComprada"].ToString(), out decimal a)) { producto.compra = a; }
                            else { producto.compra = null; }
                            if (decimal.TryParse(reader["PrecioCompra"].ToString(), out decimal b)) { producto.precioCompra = b; }
                            else { producto.precioCompra = null; }
                            productos.Add(producto);
                        }
                    }
                    conn.Close();
                    return productos;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }




        #endregion

        #region Envios

        /// <summary>
        /// Regista en la base de datos la informacion relacionada con el nuevo documento de envio.
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public static bool NuevoEnvioBool(EnvioModel envio)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"INSERT INTO Envio(CodigoEnvio,CedulaEmpleado,CodigoPuntoVenta,FechaEnvio,NombreChofer,PlacasCarro,Estado) VALUES (@codigo,@empleado,@pv,@fecha,@conductor,@placas,'Pendiente');";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigo", Statics.PrimeraAMayuscula(envio.codigo));
                    cmd.Parameters.AddWithValue("@empleado", Statics.PrimeraAMayuscula(envio.responsable.cedula));
                    cmd.Parameters.AddWithValue("@pv", Statics.PrimeraAMayuscula(envio.puntoVenta.codigo));
                    cmd.Parameters.AddWithValue("@fecha", envio.fechaEnvio.Date);
                    cmd.Parameters.AddWithValue("@conductor", Statics.PrimeraAMayuscula(envio.nombreConductor));
                    cmd.Parameters.AddWithValue("@placas", Statics.PrimeraAMayuscula(envio.placasCarro.ToUpper()));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    string response = InsertarEnvioProducto(envio);
                    if (response == "Y")
                    {
                        conn.Close();
                        registrarCambioLocal(Tipo: "Insert", NombreMetodoLocal: "getEnvioConProductos", PK: $"{envio.codigo}", NombreMetodoServidor: "ServidorNuevoEnvioBool", RespuestaExitosaServidor: "true");
                        return true;
                    }
                    /// Si algo fallo en la insercion de los productos y las cantidades se borra el registro del documento de existencias para que pueda ser exitoso en proximos intentos.
                    string cadena0 = $"delete from Envio where CodigoEnvio = '{envio.codigo}';";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.ExecuteNonQuery();
                    return false;
                }
            }
            catch (Exception e)
            {

                if (e.Message.Length > 35 && e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    return false;
                }
                else
                {
                    return false;
                }

            }
        }

        /// <summary>
        /// Regista en la base de datos la informacion relacionada con el nuevo documento de envio, coumple la mmisma funcion que NuevoEnvioBool, pero no gregistra el cambio local para evitar redundancia. .
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public static bool NuevoEnvioBoolServidor(EnvioModel envio)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"INSERT INTO Envio(CodigoEnvio,CedulaEmpleado,CodigoPuntoVenta,FechaEnvio,NombreChofer,PlacasCarro) VALUES (@codigo,@empleado,@pv,@fecha,@conductor,@placas);";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigo", Statics.PrimeraAMayuscula(envio.codigo));
                    cmd.Parameters.AddWithValue("@empleado", Statics.PrimeraAMayuscula(envio.responsable.cedula));
                    cmd.Parameters.AddWithValue("@pv", Statics.PrimeraAMayuscula(envio.puntoVenta.codigo));
                    cmd.Parameters.AddWithValue("@fecha", envio.fechaEnvio.Date);
                    cmd.Parameters.AddWithValue("@conductor", Statics.PrimeraAMayuscula(envio.nombreConductor));
                    cmd.Parameters.AddWithValue("@placas", Statics.PrimeraAMayuscula(envio.placasCarro.ToUpper()));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    string response = InsertarEnvioProducto(envio);
                    if (response == "Y")
                    {
                        conn.Close();
                        return true;
                    }
                    /// Si algo fallo en la insercion de los productos y las cantidades se borra el registro del documento de existencias para que pueda ser exitoso en proximos intentos.
                    string cadena0 = $"delete from Envio where CodigoEnvio = '{envio.codigo}';";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.ExecuteNonQuery();
                    return false;
                }
            }
            catch (Exception e)
            {

                if (e.Message.Length > 35 && e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    ///El envio ya esta registrado entonces no se aroja la excepcion
                MessageBox.Show("No se insero envio repetido");

                    return true;
                }
                else
                {
                    return false;
                }

            }
        }


        /// <summary>
        /// Inserta en la base de datos la cantidad envidada de cada producto
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public static string InsertarEnvioProducto(EnvioModel envio)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    ///Si la operacion no termino exitosamente en un intento anterior, la realiza desde cero borrando los registros insertados en el anterior intento.
                    string cadena0 = $"delete from EnvioProducto where CodigoEnvio = '{envio.codigo}';";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.ExecuteNonQuery();
                    foreach (ProductoModel producto in envio.productos)
                    {
                        string cadena = $"insert into EnvioProducto(CodigoEnvio,CodigoProducto,Cantidad) values (@codigoEnvio,@codigoProducto,@envio);";
                        SqlCommand cmd = new SqlCommand(cadena, conn);
                        cmd.Parameters.AddWithValue("@codigoEnvio", envio.codigo);
                        cmd.Parameters.AddWithValue("@codigoProducto", producto.codigoProducto);
                        cmd.Parameters.AddWithValue("@envio", string.IsNullOrEmpty(producto.compraPorLocal.ToString()) ? (object)DBNull.Value : producto.compraPorLocal);
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                    return "Y";
                }
            }
            catch (Exception e)
            {
                if (e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    return "Ya registrado.";
                }
                else
                {
                    return "Server " + e.Message;
                }
            }
        }

        /// <summary>
        /// Obtiene los productos con la cantidad enviada para el documento de envio.
        /// </summary>
        /// <param name="codigoEnvio"></param>
        /// <returns></returns>
        public static BindableCollection<ProductoModel> getProductoEnvio(string codigoEnvio)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<ProductoModel> productos = new BindableCollection<ProductoModel>();

                    string cadena = $"select producto.codigoproducto,producto.Nombre,producto.unidadventa,producto.unidadcompra,PedidoProducto.Cantidad,EnvioProducto.Cantidad as EnvioCantidad from producto join envioproducto on producto.codigoproducto = EnvioProducto.CodigoProducto join PedidoProducto on PedidoProducto.CodigoProducto = Producto.CodigoProducto where envioproducto.CodigoEnvio = '{codigoEnvio}' and CodigoPedido = '{codigoEnvio.Split(':')[0] + ":" + codigoEnvio.Split(':')[1]}' and estado = 'Activo' order by CodigoProducto;";


                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        productos.Clear();
                        while (reader.Read())
                        {
                            ProductoModel producto = new ProductoModel
                            {
                                codigoProducto = reader["codigoproducto"].ToString(),
                                nombre = reader["nombre"].ToString(),
                                unidadVenta = reader["unidadventa"].ToString().Substring(0, 3),
                                unidadCompra = reader["unidadcompra"].ToString().Substring(0, 3),
                                pedido = decimal.Parse(reader["Cantidad"].ToString()),

                            };
                            decimal.TryParse(reader["EnvioCantidad"].ToString(), out decimal a);
                            producto.compraPorLocal = a;
                            productos.Add(producto);
                        }
                    }
                    conn.Close();
                    return productos;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }

        /// <summary>
        /// Devuelve la instancia del pedido envio con los productos relacionados con el codigo de pedido dado como parametro.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public static BindableCollection<EnvioModel> getEnvioConProductos(string Caracteres)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<EnvioModel> cEnvios = new BindableCollection<EnvioModel>();
                    string cadena = $" select * from envio join PuntoVenta on Envio.CodigoPuntoVenta = PuntoVenta.CodigoPuntoVenta where CodigoEnvio = '{Caracteres}'  ORDER BY CodigoEnvio;";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cEnvios.Clear();
                        while (reader.Read())
                        {
                            EnvioModel envio = new EnvioModel
                            {
                                codigo = reader["CodigoEnvio"].ToString(),
                                nombreConductor = reader["NombreChofer"].ToString(),
                                placasCarro = reader["PlacasCarro"].ToString()


                            };
                            envio.responsable.cedula = reader["CedulaEmpleado"].ToString();
                            envio.fecha = DateTime.Parse(reader["FechaEnvio"].ToString());
                            envio.puntoVenta.codigo = reader["CodigoPuntoVenta"].ToString();
                            envio.puntoVenta.nombre = reader["Nombres"].ToString();
                            envio.productos = getProductoEnvio(envio.codigo);
                            cEnvios.Add(envio);
                        }
                    }
                    conn.Close();
                    return cEnvios;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Actualiza la informacion de un envio.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool updateEnvio(EnvioModel envio)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"UPDATE Envio SET CodigoEnvio = @codigo, CedulaEmpleado = @empleado,CodigoPuntoVenta = @pv,FechaEnvio = @fecha,NombreChofer = @conductoR,PlacasCarro=@placas WHERE CodigoEnvio = '{envio.codigo}';";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigo", Statics.PrimeraAMayuscula(envio.codigo));
                    cmd.Parameters.AddWithValue("@empleado", Statics.PrimeraAMayuscula(envio.responsable.cedula));
                    cmd.Parameters.AddWithValue("@pv", Statics.PrimeraAMayuscula(envio.puntoVenta.codigo));
                    cmd.Parameters.AddWithValue("@fecha", envio.fechaEnvio.Date);
                    cmd.Parameters.AddWithValue("@conductor", Statics.PrimeraAMayuscula(envio.nombreConductor));
                    cmd.Parameters.AddWithValue("@placas", Statics.PrimeraAMayuscula(envio.placasCarro.ToUpper()));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    string response = updateProductosEnvio(envio);
                    if (response == "Y")
                    {
                        conn.Close();
                        registrarCambioLocal(Tipo: "Update", NombreMetodoLocal: "getEnvioConProductos", PK: $"{envio.codigo}", NombreMetodoServidor: "ServidorupdateEnvio", RespuestaExitosaServidor: "true");
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception e)
            {

                if (e.Message.Length > 35 && e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    return false;
                }
                else
                {
                    return false;
                }

            }
        }

        /// <summary>
        /// Actualiza el estado del envio a:'Recibido'.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool updateEstadoEnvio(string caracteres)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"UPDATE Envio SET Estado='Recibido' WHERE CodigoEnvio = '{caracteres}';";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    //registrarCambioLocal(Tipo: "Update", NombreMetodoLocal: "getEnvioConProductos", PK: $"{envio.codigo}", NombreMetodoServidor: "ServidorupdateEnvio", RespuestaExitosaServidor: "true");
                    return true;

                }
            }
            catch (Exception e)
            {

                if (e.Message.Length > 35 && e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    return false;
                }
                else
                {
                    return false;
                }

            }
        }

        /// <summary>
        /// Actualiza la informacion de un envio, metodo necesario para usar desde el servidro y evitar redundancia.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool updateEnvioServidor(EnvioModel envio)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"UPDATE Envio SET CodigoEnvio = @codigo, CedulaEmpleado = @empleado,CodigoPuntoVenta = @pv,FechaEnvio = @fecha,NombreChofer = @conductoR,PlacasCarro=@placas WHERE CodigoEnvio = '{envio.codigo}';";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigo", Statics.PrimeraAMayuscula(envio.codigo));
                    cmd.Parameters.AddWithValue("@empleado", Statics.PrimeraAMayuscula(envio.responsable.cedula));
                    cmd.Parameters.AddWithValue("@pv", Statics.PrimeraAMayuscula(envio.puntoVenta.codigo));
                    cmd.Parameters.AddWithValue("@fecha", envio.fechaEnvio.Date);
                    cmd.Parameters.AddWithValue("@conductor", Statics.PrimeraAMayuscula(envio.nombreConductor));
                    cmd.Parameters.AddWithValue("@placas", Statics.PrimeraAMayuscula(envio.placasCarro.ToUpper()));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    string response = updateProductosEnvio(envio);
                    if (response == "Y")
                    {
                        conn.Close();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception e)
            {

                if (e.Message.Length > 35 && e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    return false;
                }
                else
                {
                    return false;
                }

            }
        }

        /// <summary>
        /// Actualiza la informacion de la cantidad enviada de un producto.
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public static string updateProductosEnvio(EnvioModel envio)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    foreach (ProductoModel producto in envio.productos)
                    {
                        string cadena = $"UPDATE  EnvioProducto SET Cantidad = @envio WHERE CodigoEnvio = @codigoEnvio AND CodigoProducto = @codigoProducto;";
                        SqlCommand cmd = new SqlCommand(cadena, conn);
                        cmd.Parameters.AddWithValue("@codigoEnvio", envio.codigo);
                        cmd.Parameters.AddWithValue("@codigoProducto", producto.codigoProducto);
                        cmd.Parameters.AddWithValue("@envio", string.IsNullOrEmpty(producto.compraPorLocal.ToString()) ? (object)DBNull.Value : producto.compraPorLocal);
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                    return "Y";
                }
            }
            catch (Exception e)
            {
                if (e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    return "Ya registrado.";
                }
                else
                {
                    return e.Message;
                }
            }
        }

        /// <summary>
        /// Devuelve todas las instancias de envios por local registradas en la base de datos.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public static BindableCollection<EnvioModel> getTodosLosEnviosPorLocal(string ccEmpleado)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<EnvioModel> cEnvios = new BindableCollection<EnvioModel>();
                    string cadena = $" select envio.CodigoEnvio, envio.NombreChofer,envio.CodigoPuntoVenta,envio.CedulaEmpleado,envio.FechaEnvio,Envio.PlacasCarro,Puntoventa.Nombres as NombrePuntoVenta,Empleado.Nombres,Empleado.Apellidos from envio join PuntoVenta on Envio.CodigoPuntoVenta = PuntoVenta.CodigoPuntoVenta join Empleado on Empleado.CedulaEmpleado = '{ccEmpleado}' where Envio.Estado = 'Pendiente' and envio.cedulaempleado = '{ccEmpleado}' ORDER BY CodigoEnvio;";

                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cEnvios.Clear();
                        while (reader.Read())
                        {
                            EnvioModel envio = new EnvioModel
                            {
                                codigo = reader["CodigoEnvio"].ToString(),
                                nombreConductor = reader["NombreChofer"].ToString(),
                                placasCarro = reader["PlacasCarro"].ToString()


                            };
                            envio.responsable.cedula = reader["CedulaEmpleado"].ToString();
                            envio.responsable.firstName = reader["Nombres"].ToString();
                            envio.responsable.lastName = reader["Apellidos"].ToString();
                            envio.fecha = DateTime.Parse(reader["FechaEnvio"].ToString());
                            envio.puntoVenta.codigo = reader["CodigoPuntoVenta"].ToString();
                            envio.puntoVenta.nombre = reader["NombrePuntoVenta"].ToString();
                            cEnvios.Add(envio);
                        }
                    }
                    conn.Close();
                    return cEnvios;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }




        #endregion

        #region Recibidos

        /// <summary>
        /// Regista en la base de datos la informacion relacionada con el nuevo documento de envio.
        /// </summary>
        /// <param name="recibido"></param>
        /// <returns></returns>
        public static bool NuevoRecibidoBool(RecibidoModel recibido, string tipo)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"INSERT INTO Recibido(CodigoRecibido,CodigoPuntoVenta,CedulaEmpleado,Fecha,NombreConductor,Peso,PlacasCarro) VALUES (@codigo,@pv,@empleado,@fecha,@conductor,@peso,@placa);";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigo", Statics.PrimeraAMayuscula(recibido.codigo));
                    cmd.Parameters.AddWithValue("@pv", Statics.PrimeraAMayuscula(recibido.puntoVenta.codigo));
                    cmd.Parameters.AddWithValue("@empleado", Statics.PrimeraAMayuscula(recibido.responsable.cedula));
                    cmd.Parameters.AddWithValue("@fecha", recibido.fechaRecibido.Date);
                    cmd.Parameters.AddWithValue("@conductor", Statics.PrimeraAMayuscula(recibido.nombreConductor));
                    cmd.Parameters.AddWithValue("@peso", string.IsNullOrEmpty(recibido.peso.ToString()) ? (object)DBNull.Value : recibido.peso);
                    cmd.Parameters.AddWithValue("@placa", Statics.PrimeraAMayuscula(recibido.placas.ToString()));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    string response = InsertarRecibidoProducto(recibido,cambiarInventario:true);
                    if (response == "Y")
                    {
                        updateEstadoEnvio(recibido.codigo);
                        conn.Close();
                        registrarCambioLocal(Tipo: "Insert", NombreMetodoLocal: "getRecibidoConProductos", PK: $"{recibido.codigo}", NombreMetodoServidor: "ServidorNuevoRecibidoBoolNoInventario", RespuestaExitosaServidor: "true");
                        return true;
                    }
                    /// Si algo fallo en la insercion de los productos y las cantidades se borra el registro del documento de existencias para que pueda ser exitoso en proximos intentos.
                    string cadena0 = $"delete from recibido where CodigoRecibido = '{recibido.codigo}';";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.ExecuteNonQuery();
                    return false;
                }
            }
            catch (Exception e)
            {

                if (e.Message.Length > 35 && e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    MessageBox.Show("Ya se ha registrado un recibido para este envio, edite ese documento");
                    return false;
                }
                else
                {
                    return false;
                }

            }
        }

        /// <summary>
        /// Metodo necesario para invocarse desde el servidor y evitar redundancia al registrar el recibido. Regista en la base de datos la informacion relacionada con el nuevo documento de envio.
        /// </summary>
        /// <param name="recibido"></param>
        /// <returns></returns>
        public static bool NuevoRecibidoBoolServidor(RecibidoModel recibido)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena1 = $"select * from Recibido where CodigoRecibido  = '{recibido.codigo}'";
                    SqlCommand cmd1 = new SqlCommand(cadena1, conn);
                    conn.Open();
                    SqlDataReader reader = cmd1.ExecuteReader();
                    if (reader.HasRows)
                    { conn.Close(); MessageBox.Show("No se insero recibido repetido"); return true; }
                    conn.Close();

                    string cadena = $"INSERT INTO Recibido(CodigoRecibido,CodigoPuntoVenta,CedulaEmpleado,Fecha,NombreConductor,Peso,PlacasCarro) VALUES (@codigo,@pv,@empleado,@fecha,@conductor,@peso,@placa);";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigo", Statics.PrimeraAMayuscula(recibido.codigo));
                    cmd.Parameters.AddWithValue("@pv", Statics.PrimeraAMayuscula(recibido.puntoVenta.codigo));
                    cmd.Parameters.AddWithValue("@empleado", Statics.PrimeraAMayuscula(recibido.responsable.cedula));
                    cmd.Parameters.AddWithValue("@fecha", recibido.fechaRecibido.Date);
                    cmd.Parameters.AddWithValue("@conductor", Statics.PrimeraAMayuscula(recibido.nombreConductor));
                    cmd.Parameters.AddWithValue("@peso", string.IsNullOrEmpty(recibido.peso.ToString()) ? (object)DBNull.Value : recibido.peso);
                    cmd.Parameters.AddWithValue("@placa", Statics.PrimeraAMayuscula(recibido.placas.ToString()));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    string response = InsertarRecibidoProducto(recibido, cambiarInventario:false);
                    if (response == "Y")
                    {
                        updateEstadoEnvio(recibido.codigo);
                        conn.Close();
                        return true;
                    }
                    /// Si algo fallo en la insercion de los productos y las cantidades se borra el registro del documento de existencias para que pueda ser exitoso en proximos intentos.
                    string cadena0 = $"delete from recibido where CodigoRecibido = '{recibido.codigo}';";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.ExecuteNonQuery();
                    return false;
                }
            }
            catch (Exception e)
            {

                if (e.Message.Length > 35 && e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    MessageBox.Show("Ya se ha registrado un recibido para este envio, edite ese documento");
                    return false;
                }
                else
                {
                    return false;
                }

            }
        }

        /// <summary>
        /// Inserta en la base de datos la cantidad recibidas de cada producto
        /// </summary>
        /// <param name="recibido"></param>
        /// <returns></returns>
        public static string InsertarRecibidoProducto(RecibidoModel recibido, bool cambiarInventario)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    ///Si la operacion no termino exitosamente en un intento anterior, la realiza desde cero borrando los registros insertados en el anterior intento.
                    string cadena0 = $"delete from RecibidoProducto where CodigoRecibido = '{recibido.codigo}';";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.ExecuteNonQuery();
                    foreach (ProductoModel producto in recibido.productos)
                    {
                        string cadena = $"insert into RecibidoProducto(CodigoRecibido,CodigoProducto,Cantidad) values (@codigoRecibido,@codigoProducto,@recibido);";
                        SqlCommand cmd = new SqlCommand(cadena, conn);
                        cmd.Parameters.AddWithValue("@codigoRecibido", recibido.codigo);
                        cmd.Parameters.AddWithValue("@codigoProducto", producto.codigoProducto);
                        cmd.Parameters.AddWithValue("@recibido", string.IsNullOrEmpty(producto.recibido.ToString()) ? (object)DBNull.Value : producto.recibido);
                        cmd.ExecuteNonQuery();
                        if (cambiarInventario)
                        {
                            InventarioModel inv = new InventarioModel()
                            {
                                codigoDelInventarioDelLocal = getIdInventario(recibido.codigo.Split(':')[0].Substring(14)),
                                tipo = $"Nuevo recibido",
                                recibido = recibido.codigo,
                                codigoProducto = producto.codigoProducto,
                                aumentoDisminucion = producto.recibido
                            };
                            inv.responsable.cedula = recibido.responsable.cedula;
                            NuevoRegistroCambioEnInventario(inv); 
                        }
                    }
                    conn.Close();
                    return "Y";
                }
            }
            catch (Exception e)
            {
                if (e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    return "Ya registrado.";
                }
                else
                {
                    return "Cliente " + e.Message;
                }
            }
        }


        /// <summary>
        /// Retorna las coincidencias en los recibidos del local dado como parametro, los codigos y fechas de los recibidos.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <param name="codigoPuntoVenta"></param>
        /// <returns></returns>
        public static BindableCollection<RecibidoModel> getRecibidos(string Caracteres, string codigoPuntoVenta)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<RecibidoModel> cRecibidos = new BindableCollection<RecibidoModel>();
                    DateTime fecha = new DateTime();
                    if (!DateTime.TryParse(Caracteres, out fecha))
                    {
                        fecha = DateTime.MinValue;
                    }

                    string cadena = $" select * from Recibido where (Fecha = '{fecha:yyyy-MM-dd}' or CodigoRecibido like '%{Caracteres}%') and CodigoPuntoVenta = '{codigoPuntoVenta}'  ORDER BY CodigoRecibido;";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cRecibidos.Clear();
                        while (reader.Read())
                        {
                            RecibidoModel rec = new RecibidoModel
                            {
                                codigo = reader["CodigoRecibido"].ToString(),
                                nombreConductor = reader["NombreConductor"].ToString()
                            };
                            rec.responsable.cedula = reader["CedulaEmpleado"].ToString();
                            rec.fecha = DateTime.Parse(reader["Fecha"].ToString());
                            rec.puntoVenta.codigo = reader["CodigoPuntoVenta"].ToString();
                            rec.placas = reader["PlacasCarro"].ToString();
                            cRecibidos.Add(rec);
                        }
                    }
                    conn.Close();
                    return cRecibidos;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Devuelve la instancia del recibido -incluidos los poductos con la cantidad recibida- cuyo codigo de recibido es dado como parametro.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public static BindableCollection<RecibidoModel> getRecibidoConProductos(string Caracteres)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<RecibidoModel> cRecibidos = new BindableCollection<RecibidoModel>();
                    string cadena = $" select * from Recibido join PuntoVenta on Recibido.CodigoPuntoVenta = PuntoVenta.CodigoPuntoVenta where Codigorecibido = '{Caracteres}'  ORDER BY CodigoRecibido;";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cRecibidos.Clear();
                        while (reader.Read())
                        {
                            RecibidoModel recibido = new RecibidoModel
                            {
                                codigo = reader["CodigoRecibido"].ToString(),
                                nombreConductor = reader["NombreConductor"].ToString(),
                                placas = reader["PlacasCarro"].ToString()


                            };
                            recibido.responsable.cedula = reader["CedulaEmpleado"].ToString();
                            ///El inventario se actualiza en el servidor en el local con este codigo de local, 
                            ///que corresponde al codigo del local donde esta fisicamente la maquina donde se registro el cambio en el inventario
                            recibido.responsable.puntoDeVenta.codigo = getLocalUbicacion();
                            recibido.fecha = DateTime.Parse(reader["Fecha"].ToString());
                            recibido.puntoVenta.codigo = reader["CodigoPuntoVenta"].ToString();
                            recibido.puntoVenta.nombre = reader["Nombres"].ToString();
                            recibido.productos = getProductosRecibido(recibido.codigo);
                            cRecibidos.Add(recibido);
                        }
                    }
                    conn.Close();
                    return cRecibidos;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los productos con la cantidad enviada y la recibida en el documento de recibido con codigo igual al dado como parametro
        /// </summary>
        /// <param name="codigoRecibido"></param>
        /// <returns></returns>
        public static BindableCollection<ProductoModel> getProductosRecibido(string codigoRecibido)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<ProductoModel> productos = new BindableCollection<ProductoModel>();

                    string cadena = $"select producto.codigoproducto,producto.Nombre,producto.unidadventa,producto.unidadcompra,EnvioProducto.Cantidad,RecibidoProducto.Cantidad as recibidoCantidad from producto join RecibidoProducto on producto.codigoproducto = RecibidoProducto.CodigoProducto join EnvioProducto on EnvioProducto.CodigoProducto = Producto.CodigoProducto where RecibidoProducto.CodigoRecibido = '{codigoRecibido}' and CodigoEnvio = '{codigoRecibido}' and estado = 'Activo' order by CodigoProducto;";


                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        productos.Clear();
                        while (reader.Read())
                        {
                            ProductoModel producto = new ProductoModel
                            {
                                codigoProducto = reader["codigoproducto"].ToString(),
                                nombre = reader["nombre"].ToString(),
                                unidadVenta = reader["unidadventa"].ToString().Substring(0, 3),
                                unidadCompra = reader["unidadcompra"].ToString().Substring(0, 3)
                            };
                            decimal.TryParse(reader["Cantidad"].ToString(), out decimal b);
                            producto.compraPorLocal = b;
                            decimal.TryParse(reader["recibidoCantidad"].ToString(), out decimal a);
                            producto.recibido = a;
                            productos.Add(producto);
                        }
                    }
                    conn.Close();
                    return productos;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }

        /// <summary>
        /// Actualiza la informacion de un recibido.
        /// </summary>
        /// <param name="recibido"></param>
        /// <returns></returns>
        public static bool updateRecibido(RecibidoModel recibido)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"UPDATE Recibido SET CodigoRecibido = @codigo, CedulaEmpleado = @empleado,CodigoPuntoVenta = @pv,Fecha = @fecha,NombreConductor = @conductor,PlacasCarro=@placas WHERE CodigoRecibido = '{recibido.codigo}';";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigo", Statics.PrimeraAMayuscula(recibido.codigo));
                    cmd.Parameters.AddWithValue("@empleado", Statics.PrimeraAMayuscula(recibido.responsable.cedula));
                    cmd.Parameters.AddWithValue("@pv", Statics.PrimeraAMayuscula(recibido.puntoVenta.codigo));
                    cmd.Parameters.AddWithValue("@fecha", recibido.fecha.Date);
                    cmd.Parameters.AddWithValue("@conductor", Statics.PrimeraAMayuscula(recibido.nombreConductor));
                    cmd.Parameters.AddWithValue("@placas", Statics.PrimeraAMayuscula(recibido.placas.ToUpper()));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    string response = updateProductosRecibido(recibido,cambiarInventario:true);
                    if (response == "Y")
                    {                 
                        conn.Close();
                        registrarCambioLocal(Tipo: "Update", NombreMetodoLocal: "getRecibidoConProductos", PK: $"{recibido.codigo}", NombreMetodoServidor: "ServidorupdateRecibidoNoInventario", RespuestaExitosaServidor: "true");
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception e)
            {

                if (e.Message.Length > 35 && e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    return false;
                }
                else
                {
                    return false;
                }

            }
        }

        /// <summary>
        /// Metodo que se invoca desde el servidor. Actualiza la informacion de un recibido.
        /// </summary>
        /// <param name="recibido"></param>
        /// <returns></returns>
        public static bool updateRecibidoServidor(RecibidoModel recibido)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"UPDATE Recibido SET CodigoRecibido = @codigo, CedulaEmpleado = @empleado,CodigoPuntoVenta = @pv,Fecha = @fecha,NombreConductor = @conductor,PlacasCarro=@placas WHERE CodigoRecibido = '{recibido.codigo}';";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigo", Statics.PrimeraAMayuscula(recibido.codigo));
                    cmd.Parameters.AddWithValue("@empleado", Statics.PrimeraAMayuscula(recibido.responsable.cedula));
                    cmd.Parameters.AddWithValue("@pv", Statics.PrimeraAMayuscula(recibido.puntoVenta.codigo));
                    cmd.Parameters.AddWithValue("@fecha", recibido.fecha.Date);
                    cmd.Parameters.AddWithValue("@conductor", Statics.PrimeraAMayuscula(recibido.nombreConductor));
                    cmd.Parameters.AddWithValue("@placas", Statics.PrimeraAMayuscula(recibido.placas.ToUpper()));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    string response = updateProductosRecibido(recibido,cambiarInventario:false);
                    if (response == "Y")
                    {
                        conn.Close();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception e)
            {

                if (e.Message.Length > 35 && e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    return false;
                }
                else
                {
                    return false;
                }

            }
        }

        /// <summary>
        /// Actualiza la informacion de la cantidad recibida de un producto.
        /// </summary>
        /// <param name="recibido"></param>
        /// <returns></returns>
        public static string updateProductosRecibido(RecibidoModel recibido, bool cambiarInventario)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    foreach (ProductoModel producto in recibido.productos)
                    {
                        string cadena = $"UPDATE  RecibidoProducto SET Cantidad = @recibido WHERE CodigoRecibido = @codigo AND CodigoProducto = @codigoProducto;";
                        SqlCommand cmd = new SqlCommand(cadena, conn);
                        cmd.Parameters.AddWithValue("@codigo", recibido.codigo);
                        cmd.Parameters.AddWithValue("@codigoProducto", producto.codigoProducto);
                        cmd.Parameters.AddWithValue("@recibido", string.IsNullOrEmpty(producto.recibido.ToString()) ? (object)DBNull.Value : producto.recibido);
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                    ///Las actualizaciones en el inventario se iteran sobre la lista 'productos', la informacion del cambio de inventario esta en
                    ///'productosActualizados', por eso esta asignacion.      
                    ///recibido.productos = recibido.productosActualizados;
                    if (cambiarInventario)
                    {
                        foreach (ProductoModel productoModel in recibido.productosActualizados)
                        {
                            InventarioModel inv = new InventarioModel()
                            {
                                codigoDelInventarioDelLocal = getIdInventario(recibido.codigo.Split(':')[0].Substring(14)),
                                tipo = $"Cambio recibido",
                                recibido = recibido.codigo,
                                codigoProducto = productoModel.codigoProducto,
                                aumentoDisminucion = productoModel.recibido
                            };
                            inv.responsable.cedula = recibido.responsable.cedula;
                            NuevoRegistroCambioEnInventario(inv);
                        }
                    }
                    return "Y";
                }
            }
            catch (Exception e)
            {
                if (e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    return "Ya registrado.";
                }
                else
                {
                    return e.Message;
                }
            }
        }


        #endregion

        #region Inventario

        /// <summary>
        /// Registra los datos de creacion del nuevo inventario
        /// </summary>
        /// <param name="inventario"></param>
        public static bool nuevoInventario(InventarioModel inventario)
        {
            ///No tiene referencias pues  se llama solamente desde el servidro, ya que los locales nuevos y los inventarios nuevos solo se pueden crear cuando se esta conectao al servidor
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    string cadena = $"insert into Inventario(CodigoInventario,CodigoPuntoVenta) values ('{inventario.codigoDelInventarioDelLocal}','{inventario.puntoVenta.codigo}');";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return true;
                }
            }
            catch (Exception e)
            {

                MessageBox.Show("No se guardaron los datos del inventario del nuevo local");
                return false;
            }
        }

        /// <summary>
        /// Crera los nuevos campos de inventario para el nuevo producto.
        /// </summary>
        /// <param name="codigoProducto"></param>
        public static void nuevoInventarioProducto(string codigoProducto)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<LocalModel> locales = getLocales();
                    foreach (LocalModel local in locales)
                    {
                        string cadena = $"insert into InventarioProducto(codigoinventario, Codigoproducto,Cantidad) values ((select CodigoInventario from inventario where CodigoPuntoVenta = '{local.codigo}'), '{codigoProducto}',0) ";
                        SqlCommand cmd = new SqlCommand(cadena, conn);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Obtiene el registro de la tabla con los registros de cambios en inventario
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public static BindableCollection<InventarioModel> getCambioInventario(string codigo)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<InventarioModel> cInventarios = new BindableCollection<InventarioModel>();
                    string cadena = $"select * from HistorialIventario where Id = '{codigo}'";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cInventarios.Clear();
                        while (reader.Read())
                        {
                            InventarioModel inventario = new InventarioModel()
                            {
                                codigoDelInventarioDelLocal = reader["CodigoInventario"].ToString(),
                                codigoProducto = reader["CodigoProducto"].ToString(),
                                tipo = reader["Tipo"].ToString(),                                                                                          
                                fecha = DateTime.Parse(reader["Fecha"].ToString()),
                                factura = reader["Factura"].ToString(),
                                recibido = reader["Recibido"].ToString()
                            };
                            Int32.TryParse(reader["Id"].ToString(), out int regis);
                            inventario.idRegistroLocal = regis;
                            Int32.TryParse(reader["IdRegistroServidor"].ToString(), out int regisS);
                            inventario.idRegistroServidor = regisS;
                            decimal.TryParse(reader["AumentoDisminucion"].ToString(), out decimal valor);
                            inventario.aumentoDisminucion = valor;
                            decimal.TryParse(reader["Total"].ToString(), out decimal tot);
                            inventario.total = tot;
                            inventario.responsable.cedula = reader["CedulaEmpleado"].ToString();
                            cInventarios.Add(inventario);
                        }
                    }
                    conn.Close();
                    return cInventarios;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "getInventario");
                return null;
            }



        }

        /// <summary>
        /// Inserta en la base de datos el cambio en el inventario del local dado en el invetario
        /// </summary>
        /// <param name="inventario"></param>
        /// <returns></returns>
        public static bool NuevoRegistroCambioEnInventario(InventarioModel inventario)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    ///Si no hubo aumento o disminucion no se hace el registro en el inventario, esto debe estar o de otro modo se sumaria dos veces la cantidad actual, 
                    ///ademas que se ejecutaria el metodo inutilmente
                    if (inventario.aumentoDisminucion == 0 || inventario.aumentoDisminucion == null) return true;

                    //Hacer esta comprobacion pues el valor por defecto en la base de datos de idregistroservidor va a ser 0
                    if (inventario.idRegistroServidor == 0) { inventario.idRegistroServidor = null; }

                    ///Importante aqui es el idRegistroDelServidor, en la busqueda del servidor es diferente, los metodos no son exactamente iguales.
                    string cadena = $"select * from HistorialIventario where (IdRegistroServidor=@idRegistroServidor and CodigoInventario=@codigoDelInventarioDelLocal) or Id = @idRegistroLocal ;";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@idRegistroServidor", string.IsNullOrEmpty(inventario.idRegistroServidor.ToString()) ? (object)DBNull.Value : inventario.idRegistroServidor);
                    cmd.Parameters.AddWithValue("@idRegistroLocal", string.IsNullOrEmpty(inventario.idRegistroLocal.ToString()) ? (object)DBNull.Value : inventario.idRegistroLocal);
                    cmd.Parameters.AddWithValue("@codigoDelInventarioDelLocal", string.IsNullOrEmpty(inventario.codigoDelInventarioDelLocal.ToString()) ? (object)DBNull.Value : inventario.codigoDelInventarioDelLocal);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            //MessageBox.Show("No se inserto cambio inventario repetido");
                            return true; }
                    }
                    conn.Close();

                    ///Poner de nuevo el valor en su valor por defecto para que en otras iteraciones se pueda ejecutar el resto de codigo
                    if (inventario.idRegistroServidor == null) { inventario.idRegistroServidor = 0; }

                    decimal? cantidad = inventario.aumentoDisminucion;
                    string cadena1 = $"select Cantidad from InventarioProducto where codigoinventario = '{inventario.codigoDelInventarioDelLocal}' and CodigoProducto  = '{inventario.codigoProducto}';";
                    SqlCommand cmd1 = new SqlCommand(cadena1, conn);
                    conn.Open();
                    using (SqlDataReader reader1 = cmd1.ExecuteReader())
                    {
                        if (reader1.FieldCount > 1) { MessageBox.Show("Error en el iventario, se repiten productos en el inventario de un solo local"); return false; }

                        reader1.Read();
                        if (decimal.TryParse(reader1["Cantidad"].ToString(), out decimal valor))
                            cantidad = cantidad + valor;
                    }
                    conn.Close();

                    string Id;
                    string cadena0 = $"insert into HistorialIventario(CodigoInventario,CodigoProducto,Tipo,AumentoDisminucion,Total,CedulaEmpleado,Fecha,IdRegistroServidor,Factura,Recibido) OUTPUT INSERTED.Id values('{inventario.codigoDelInventarioDelLocal}','{inventario.codigoProducto}','{inventario.tipo}',@aumentodisminucion, @cantidad ,'{ inventario.responsable.cedula}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}',{inventario.idRegistroServidor},@factura,@recibido);";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.Parameters.AddWithValue("@aumentodisminucion", inventario.aumentoDisminucion);
                    cmd0.Parameters.AddWithValue("@cantidad", cantidad);
                    cmd0.Parameters.AddWithValue("@factura", string.IsNullOrEmpty(inventario.factura) ? (object)DBNull.Value : inventario.factura);
                    cmd0.Parameters.AddWithValue("@recibido", string.IsNullOrEmpty(inventario.recibido) ? (object)DBNull.Value : inventario.recibido);

                    conn.Open();
                    using (SqlDataReader reader0 = cmd0.ExecuteReader())
                    {
                        reader0.Read();
                        Id = reader0["Id"].ToString();

                    }
                    conn.Close();

                    string cadena2 = $"update InventarioProducto set Cantidad = @cantidad where codigoinventario = {inventario.codigoDelInventarioDelLocal} and CodigoProducto  = '{inventario.codigoProducto}';";
                    SqlCommand cmd2 = new SqlCommand(cadena2, conn);
                    cmd2.Parameters.AddWithValue("@cantidad", cantidad);
                    conn.Open();
                    cmd2.ExecuteNonQuery();
                    conn.Close(); 

                    ///Evitar que se registre en el registro de cambios locales la variacion en el inventario a fin de evitar que al conectarce al servidor se genere un bucle 
                    if(inventario.idRegistroServidor == 0 | inventario.idRegistroServidor == null)
                        registrarCambioLocal(NombreMetodoLocal: "getCambioInventario", PK: $"{Id}", NombreMetodoServidor: "ServidorNuevoRegistroCambioEnInventario", RespuestaExitosaServidor: "true", Tipo: "Insert");

                    return true;


                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + " NuevoRegistroCambioEnInventario");
                return false;
            }



        }

        /// <summary>
        /// Retorna el codigo del inventario del local con el nombre dado
        /// </summary>
        /// <param name="nombreLocal"></param>
        /// <returns></returns>
        public static string getIdInventario(string nombreLocal_codigoLocal)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    Int32.TryParse(nombreLocal_codigoLocal, out int codigoNumerico);
                    string codigo;
                    string cadena = $"select * from Inventario where CodigoPuntoVenta  = (select codigoPuntoVenta from Puntoventa where Nombres = '{nombreLocal_codigoLocal}') or CodigoPuntoVenta  = '{codigoNumerico}'";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        reader.Read();
                        codigo = reader["CodigoInventario"].ToString();

                    }
                    conn.Close();
                    return codigo;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + " getIdInventario - Cliente");
                return null;
            }
        }

        #endregion

        #region Clientes

        /// <summary>
        /// Metodo encargado de ejecutar el query insert del nuevo cliente en la base de datos local.
        /// </summary>
        /// <param name="Cliente">Instancia de la clase ClientesModel.</param>
        /// <returns></returns>        
        public static bool AddClient(ClientesModel Cliente)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = "INSERT INTO Clientes(Nombres,Apellidos,CedulaCliente,Email,Telefono,Puntos,Estado) VALUES (@name,@lastname,@cedula,@correo,@telefono, 100 ,'Activo')";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@name", Statics.PrimeraAMayuscula(Cliente.firstName));
                    cmd.Parameters.AddWithValue("@lastname", Statics.PrimeraAMayuscula(Cliente.lastName));
                    cmd.Parameters.AddWithValue("@cedula", Cliente.cedula);
                    cmd.Parameters.AddWithValue("@correo", string.IsNullOrEmpty(Cliente.correo) ? (object)DBNull.Value : Cliente.correo);
                    cmd.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(Cliente.telefono) ? (object)DBNull.Value : Cliente.telefono);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    registrarCambioLocal(Tipo: "Insert", NombreMetodoLocal: "getClienteCedula", PK: $"{Cliente.cedula}", NombreMetodoServidor: "ServidorAddClient", RespuestaExitosaServidor: "true");
                    return true;

                }
            }
            catch (Exception e)
            {
                if (e.Message.Substring(0, 50) == $"Violation of PRIMARY KEY constraint 'PK_Clientes'.")
                {
                    MessageBox.Show($"La cedula {Cliente.cedula} ya esta registrada.");
                    return false;
                }
                else
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
            }

        }

        /// <summary>
        /// Metodo encargado de ejecutar el query insert del nuevo cliente en la base de datos local.
        /// </summary>
        /// <param name="Cliente">Instancia de la clase ClientesModel.</param>
        /// <returns></returns>        
        public static bool AddClientServidor(ClientesModel Cliente)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = "INSERT INTO Clientes(Nombres,Apellidos,CedulaCliente,Email,Telefono,Puntos,Estado) VALUES (@name,@lastname,@cedula,@correo,@telefono, 100 ,'Activo')";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@name", Statics.PrimeraAMayuscula(Cliente.firstName));
                    cmd.Parameters.AddWithValue("@lastname", Statics.PrimeraAMayuscula(Cliente.lastName));
                    cmd.Parameters.AddWithValue("@cedula", Cliente.cedula);
                    cmd.Parameters.AddWithValue("@correo", string.IsNullOrEmpty(Cliente.correo) ? (object)DBNull.Value : Cliente.correo);
                    cmd.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(Cliente.telefono) ? (object)DBNull.Value : Cliente.telefono);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return true;

                }
            }
            catch (Exception e)
            {
                if (e.Message.Substring(0, 50) == $"Violation of PRIMARY KEY constraint 'PK_Clientes'.")
                {
                    /*MessageBox.Show($"La cedula {Cliente.cedula} ya esta registrada.");*/
                    return true;
                }
                else
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
            }

        }

        /// <summary>
        /// Method that does a select query against the 'Clientes' table at the local database and get the result of searching the coincidences into the cedula, nombre or apellido of the given characters.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public static BindableCollection<ClientesModel> getClientes(string Caracteres)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<ClientesModel> cli = new BindableCollection<ClientesModel>();
                    string cadena = $"SELECT * FROM Clientes WHERE( ( CedulaCliente like '%{Caracteres}%' ) or ( Nombres like '%{Caracteres}%' ) or ( Apellidos like '%{Caracteres}%' )) and Estado = 'Activo' ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cli.Clear();
                        while (reader.Read())
                        {
                            ClientesModel cliente = new ClientesModel
                            {
                                firstName = reader["Nombres"].ToString(),
                                lastName = reader["Apellidos"].ToString(),
                                cedula = reader["CedulaCliente"].ToString(),
                                correo = reader["Email"].ToString(),
                                telefono = reader["Telefono"].ToString(),
                                puntos = Int32.Parse(reader["Puntos"].ToString()),
                                estado = reader["Estado"].ToString()
                            };
                            cli.Add(cliente);
                        }
                    }
                    conn.Close();
                    return cli;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Method that does a select query against the 'Clientes' table at the local database and get the result of searching the coincidences into the cedula, nombre or apellido of the given characters.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public static BindableCollection<ClientesModel> getClienteCedula(string Caracteres)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<ClientesModel> cli = new BindableCollection<ClientesModel>();
                    string cadena = $"SELECT * FROM Clientes WHERE CedulaCliente = @cedula; ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@cedula", Caracteres);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cli.Clear();
                        while (reader.Read())
                        {
                            ClientesModel cliente = new ClientesModel
                            {
                                firstName = reader["Nombres"].ToString(),
                                lastName = reader["Apellidos"].ToString(),
                                cedula = reader["CedulaCliente"].ToString(),
                                correo = reader["Email"].ToString(),
                                telefono = reader["Telefono"].ToString(),
                                puntos = Int32.Parse(reader["Puntos"].ToString()),
                                estado = reader["Estado"].ToString()
                            };
                            cli.Add(cliente);
                        }
                    }
                    conn.Close();
                    return cli;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Actualiza los datos del usuario dado.
        /// </summary>
        /// <param name="Cliente"></param>
        /// <returns></returns>
        public static bool ActualizarCliente(ClientesModel Cliente)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {


                    string cadena = "UPDATE Clientes SET Nombres=@name, Apellidos=@lastname, Estado = @estado,  Email=@correo, Telefono=@telefono, Puntos=@puntos WHERE CedulaCliente = @cedula ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@name", Statics.PrimeraAMayuscula(Cliente.firstName));
                    cmd.Parameters.AddWithValue("@lastname", Statics.PrimeraAMayuscula(Cliente.lastName));
                    cmd.Parameters.AddWithValue("@cedula", Cliente.cedula);
                    cmd.Parameters.AddWithValue("@correo", string.IsNullOrEmpty(Cliente.correo) ? (object)DBNull.Value : Cliente.correo);
                    cmd.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(Cliente.telefono) ? (object)DBNull.Value : Cliente.telefono);
                    cmd.Parameters.AddWithValue("@puntos", Cliente.puntos);
                    cmd.Parameters.AddWithValue("@estado", Cliente.estado);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    registrarCambioLocal(Tipo: "Update", NombreMetodoLocal: "getClienteCedula", PK: $"{Cliente.cedula}", NombreMetodoServidor: "ServidorActualizarCliente", RespuestaExitosaServidor: "true");

                    return true;

                }
            }
            catch (Exception e)
            {
                if (e.Message.Substring(0, 50) == $"Violation of PRIMARY KEY constraint 'PK_Clientes'.")
                {
                    MessageBox.Show($"La cedula {Cliente.cedula} ya esta registrada.");
                    return false;
                }
                else
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// Actualiza los datos del cliente dado.
        /// </summary>
        /// <param name="Cliente"></param>
        /// <returns></returns>
        public static bool ActualizarClienteServidor(ClientesModel Cliente)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {


                    string cadena = "UPDATE Clientes SET Nombres=@name, Apellidos=@lastname, Estado = @estado, Email=@correo, Telefono=@telefono, Puntos=@puntos WHERE CedulaCliente = @cedula ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@name", Statics.PrimeraAMayuscula(Cliente.firstName));
                    cmd.Parameters.AddWithValue("@lastname", Statics.PrimeraAMayuscula(Cliente.lastName));
                    cmd.Parameters.AddWithValue("@cedula", Cliente.cedula);
                    cmd.Parameters.AddWithValue("@correo", string.IsNullOrEmpty(Cliente.correo) ? (object)DBNull.Value : Cliente.correo);
                    cmd.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(Cliente.telefono) ? (object)DBNull.Value : Cliente.telefono);
                    cmd.Parameters.AddWithValue("@puntos", Cliente.puntos);
                    cmd.Parameters.AddWithValue("@estado", Cliente.estado);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return true;

                }
            }
            catch (Exception e)
            {
                if (e.Message.Substring(0, 50) == $"Violation of PRIMARY KEY constraint 'PK_Clientes'.")
                {
                    MessageBox.Show($"La cedula {Cliente.cedula} ya esta registrada.");
                    return false;
                }
                else
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// Actualiza los puntos del cliente dado.
        /// </summary>
        /// <param name="Cliente"></param>
        /// <returns></returns>
        public static bool ActualizarPuntosCliente(ClientesModel Cliente, decimal? puntos)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {


                    string cadena = "UPDATE Clientes SET Puntos = Puntos + @puntos WHERE CedulaCliente = @cedula ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@cedula", Cliente.cedula);
                    cmd.Parameters.AddWithValue("@puntos", puntos);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return true;

                }
            }
            catch (Exception e)
            {
  
                MessageBox.Show(e.Message);
                return false;
                
            }
        }

        /// <summary>
        ///  Elimina el cliente con el número de cédula dado.
        /// </summary>
        /// <param name="Cedula">Número de cédula del cliente a eliminar.</param>
        /// <returns></returns>
        public static bool deleteCliente(string Cedula)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {

                    string cadena = $"update Clientes set Estado  = 'Inactivo' Where CedulaCliente = '{Cedula}' ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    //Eliminar de la base de datos solo cambia el estado del ciente, por eso es un update
                    registrarCambioLocal(Tipo: "Update", NombreMetodoLocal: "getClienteCedula", PK: $"{Cedula}", NombreMetodoServidor: "ServidorActualizarCliente", RespuestaExitosaServidor: "true");
                    return true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }

        }



        #endregion

        #region Facturas
        /// <summary>
        /// Registra en la base de datos la informacion relacionada con la nueva factura
        /// </summary>
        /// <param name="factura">Datos de la factura que se va a registrar</param>
        /// <returns></returns>
        public static bool NuevaFacturaBool(FacturaModel factura)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"INSERT INTO FacturasVenta (CodigoFactura,CedulaCliente,CedulaEmpleado,CodigoPuntoPago,Fecha,ValorTotal,IVA,Descuento) VALUES (@codigoFactura,@cliente,@empleado,@puntoPago,@fecha,@total,@iva,@dto);";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigoFactura", factura.codigo);
                    cmd.Parameters.AddWithValue("@cliente", string.IsNullOrEmpty(factura.cliente.cedula) ? (object) DBNull.Value : factura.cliente.cedula);
                    cmd.Parameters.AddWithValue("@empleado", factura.responsable.cedula);
                    cmd.Parameters.AddWithValue("@puntoPago", factura.puntoDePago);
                    /*La hora de creacion de la factura que queda registrada en el codigo es la hora de instanciacion de la clase, la hora que se registra aqui corresponde 
                     con el instante en que la factura es registrada en la base de datos, es MUY importante pues de esta hora depende la generacion de los ingresos*/
                    cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
                    cmd.Parameters.AddWithValue("@total", factura.valorTotal);
                    cmd.Parameters.AddWithValue("@iva", factura.ivaTotal);
                    cmd.Parameters.AddWithValue("@dto", factura.descuentoTotal);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    string response = InsertarFacturaProductos(factura:factura);
                    if (factura.cliente.cedula != null)  ActualizarPuntosCliente( Cliente: factura.cliente, puntos: factura.valorTotal / 1000);
                    if (response == "Y")
                    {
                        conn.Close();
                        registrarCambioLocal(Tipo: "Insert", NombreMetodoLocal: "getFacturaConProductos", PK: $"{factura.codigo}", NombreMetodoServidor: "ServidorNuevaFacturaBool", RespuestaExitosaServidor: "true");
                        return true;
                    }
                    else { return false; }
                }
            }
            catch (Exception e)
            {

                if (e.Message.Length > 35 && e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    MessageBox.Show("Error: Ya se ha registrado este id de factura. Informe a un administrador.");
                    return false;
                }
                else
                {
                    MessageBox.Show(e.Message);

                    return false;
                }

            }
        }
        /// <summary>
        /// Inserta en la base de datos los datos de cada uno de los productos listados en la factura
        /// </summary>
        /// <param name="factura"></param>
        /// <returns></returns>
        public static string InsertarFacturaProductos(FacturaModel factura)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    foreach (ProductoModel producto in factura.productos)
                    {
                        string cadena = $"insert into FacturaVentaProducto(codigoFactura,CodigoProducto,Cantidad) values (@codigoFactura,@codigoProducto,@cantidad);";
                        SqlCommand cmd = new SqlCommand(cadena, conn);
                        cmd.Parameters.AddWithValue("@codigoFactura", factura.codigo);
                        cmd.Parameters.AddWithValue("@codigoProducto", producto.codigoProducto);
                        cmd.Parameters.AddWithValue("@cantidad", producto.cantidadVenta);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        InventarioModel inv = new InventarioModel()
                        {
                            codigoDelInventarioDelLocal = getIdInventario(factura.puntoVenta.codigo),
                            tipo = $"Factura Venta",
                            factura = factura.codigo,
                            codigoProducto = producto.codigoProducto,
                            ///Es negativo pues disminuye el valor del inventario del producto
                            aumentoDisminucion = -(producto.cantidadVenta)
                        };
                        inv.responsable.cedula = factura.responsable.cedula;
                        NuevoRegistroCambioEnInventario(inv);
                        
                    }
                    conn.Close();
                    return "Y";
                }
            }
            catch (Exception e)
            {
                if (e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    return "Ya registrado.";
                }
                else
                {
                    MessageBox.Show(e.Message);

                    return "Cliente " + e.Message;
                }
            }
        }
        /// <summary>
        /// Obtiene los datos de la factura cuyo codigo es dado com parametro
        /// </summary>
        /// <param name="codigoFactura"></param>
        /// <returns></returns>
        public static BindableCollection<FacturaModel> getFacturaConProductos(string codigoFactura) 
        {
            try
            {
                
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<FacturaModel> cFactura = new BindableCollection<FacturaModel>();
                    string cadena = $" select Distinct * from FacturasVenta where CodigoFactura = @codigoFactura order by Fecha;";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigoFactura", codigoFactura);
                    conn.Open();
                    bool soloUnValor = true;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!soloUnValor) { MessageBox.Show("Error: Código de factura repetido. Contacte a un administrador."); return null; }
                        cFactura.Clear();
                        while (reader.Read())
                        {
                            soloUnValor = false;
                            FacturaModel factura = new FacturaModel
                            {
                                codigo = reader["CodigoFactura"].ToString(),
                            };
                            factura.cliente.cedula = reader["CedulaCliente"].ToString();
                            factura.responsable.cedula = reader["CedulaEmpleado"].ToString();
                            factura.puntoDePago = reader["CodigoPuntoPago"].ToString();
                            ///El inventario se actualiza en el servidor en el local con este codigo de local, 
                            ///que corresponde al codigo del local donde esta fisicamente la maquina donde se registro el cambio en el inventario
                            factura.puntoVenta.codigo = factura.puntoDePago.Split(':')[0];
                            factura.fecha = DateTime.Parse(reader["Fecha"].ToString());
                            decimal.TryParse(reader["ValorTotal"].ToString(), out decimal total);
                            factura.valorTotal = total;
                            factura.productos = getProductosFactura(factura.codigo);
                            cFactura.Add(factura);
                        }
                    }
                    conn.Close();
                    return cFactura;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                Console.WriteLine(e.Message);
                return null;
            }
        }
        /// <summary>
        /// Obtiene los datos de los productos de la factura con el codigo dado como parametro
        /// </summary>
        /// <param name="codigoFactura"></param>
        /// <returns></returns>
        public static BindableCollection<ProductoModel> getProductosFactura(string codigoFactura)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<ProductoModel> productos = new BindableCollection<ProductoModel>();

                    string cadena = $"select * from FacturaVentaProducto  where CodigoFactura = @codigoFactura;";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigoFactura", codigoFactura);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        productos.Clear();
                        while (reader.Read())
                        {
                            ProductoModel producto = new ProductoModel
                            {
                                codigoProducto = reader["codigoproducto"].ToString(),
                            };
                            Decimal.TryParse(reader["Cantidad"].ToString(), out decimal b);
                            producto.cantidadVenta = b;
                            productos.Add(producto);
                        }
                    }
                    conn.Close();
                    return productos;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                Console.WriteLine(e.Message);
                return null;
            }

        }

        /// <summary>
        /// Registra en la base de datos la informacion relacionada con la  factura borrada
        /// </summary>
        /// <param name="factura">Datos de la factura que se va a registrar</param>
        /// <returns></returns>
        public static bool NuevaFacturaBorradaBool(FacturaModel factura)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"INSERT INTO FacturasVentaBorradas (CodigoFactura,CedulaCliente,CedulaCajero,CedulaSupervisor,CodigoPuntoPago,Fecha,ValorTotal,Descripcion) VALUES (@codigoFactura,@cliente,@empleadoCajero,@empleadoSupervisor,@puntoPago,@fecha,@total,@obs);";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigoFactura", factura.codigo);
                    cmd.Parameters.AddWithValue("@cliente", string.IsNullOrEmpty(factura.cliente.cedula) ? (object)DBNull.Value : factura.cliente.cedula);
                    cmd.Parameters.AddWithValue("@empleadoCajero", factura.responsable.cedula);
                    cmd.Parameters.AddWithValue("@empleadoSupervisor", factura.superAuto.cedula);
                    cmd.Parameters.AddWithValue("@puntoPago", factura.puntoDePago);
                    cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
                    cmd.Parameters.AddWithValue("@total", factura.valorTotal);
                    cmd.Parameters.AddWithValue("@obs", factura.observaciones);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    string response = InsertarFacturaBorradaProductos(factura: factura);
                    if (response == "Y")
                    {
                        conn.Close();
                        registrarCambioLocal(Tipo: "Insert", NombreMetodoLocal: "getFacturaBorradaConProductos", PK: $"{factura.codigo}", NombreMetodoServidor: "ServidorNuevaFacturaBorradaBool", RespuestaExitosaServidor: "true");
                        return true;
                    }
                    else { return false; }
                }
            }
            catch (Exception e)
            {

                if (e.Message.Length > 35 && e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    MessageBox.Show("Error: Ya se ha registrado este id de factura. Informe a un administrador.");
                    return false;
                }
                else
                {
                    MessageBox.Show(e.Message);

                    return false;
                }

            }
        }

        /// <summary>
        /// Inserta en la base de datos los datos de cada uno de los productos listados en la factura
        /// </summary>
        /// <param name="factura"></param>
        /// <returns></returns>
        public static string InsertarFacturaBorradaProductos(FacturaModel factura)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    foreach (ProductoModel producto in factura.productos)
                    {
                        string cadena = $"insert into FacturaVentaBorradaProducto(codigoFactura,CodigoProducto,Cantidad) values (@codigoFactura,@codigoProducto,@cantidad);";
                        SqlCommand cmd = new SqlCommand(cadena, conn);
                        cmd.Parameters.AddWithValue("@codigoFactura", factura.codigo);
                        cmd.Parameters.AddWithValue("@codigoProducto", producto.codigoProducto);
                        cmd.Parameters.AddWithValue("@cantidad", producto.cantidadVenta);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    conn.Close();
                    return "Y";
                }
            }
            catch (Exception e)
            {
                if (e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    return "Ya registrado.";
                }
                else
                {
                    MessageBox.Show(e.Message);

                    return "Cliente " + e.Message;
                }
            }
        }
        /// <summary>
        /// Obtiene los datos de la factura cuyo codigo es dado com parametro
        /// </summary>
        /// <param name="codigoFactura"></param>
        /// <returns></returns>
        public static BindableCollection<FacturaModel> getFacturaBorradaConProductos(string codigoFactura)
        {
            try
            {

                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<FacturaModel> cFactura = new BindableCollection<FacturaModel>();
                    string cadena = $" select Distinct * from FacturasVentaBorradas where CodigoFactura = @codigoFactura order by Fecha;";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigoFactura", codigoFactura);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cFactura.Clear();
                        while (reader.Read())
                        {
                            FacturaModel factura = new FacturaModel
                            {
                                codigo = reader["CodigoFactura"].ToString(),
                            };
                            factura.cliente.cedula = reader["CedulaCliente"].ToString();
                            factura.responsable.cedula = reader["CedulaCajero"].ToString();
                            factura.superAuto.cedula = reader["CedulaSupervisor"].ToString();
                            factura.puntoDePago = reader["CodigoPuntoPago"].ToString();
                            factura.puntoVenta.codigo = factura.puntoDePago.Split(':')[0];
                            factura.fecha = DateTime.Parse(reader["Fecha"].ToString());
                            decimal.TryParse(reader["ValorTotal"].ToString(), out decimal total);
                            factura.valorTotal = total;
                            factura.observaciones = reader["Descripcion"].ToString();
                            factura.productos = getProductosFactura(factura.codigo);
                            cFactura.Add(factura);
                        }
                    }
                    conn.Close();
                    return cFactura;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                Console.WriteLine(e.Message);
                return null;
            }
        }
        /// <summary>
        /// Obtiene los datos de los productos de la factura con el codigo dado como parametro
        /// </summary>
        /// <param name="codigoFactura"></param>
        /// <returns></returns>
        public static BindableCollection<ProductoModel> getProductosBorradosFactura(string codigoFactura)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<ProductoModel> productos = new BindableCollection<ProductoModel>();

                    string cadena = $"select * from FacturasVentaBorradas  where CodigoFactura = @codigoFactura;";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigoFactura", codigoFactura);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        productos.Clear();
                        while (reader.Read())
                        {
                            ProductoModel producto = new ProductoModel
                            {
                                codigoProducto = reader["codigoproducto"].ToString(),
                            };
                            Decimal.TryParse(reader["Cantidad"].ToString(), out decimal b);
                            producto.cantidadVenta = b;
                            productos.Add(producto);
                        }
                    }
                    conn.Close();
                    return productos;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                Console.WriteLine(e.Message);
                return null;
            }

        }
         
        /// <summary>
        /// Registra el id de la que sera la primera factura para el arqueo de caja
        /// </summary>
        /// <param name="codigoFactura"></param>
        /// <returns></returns>
        public static bool primeraFactura(string codigoFactura, string inicioFinal)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"INSERT INTO PrimeraFactura (CodigoFactura, InicioFinal,FechaHora) VALUES ('{codigoFactura}','{inicioFinal}',@fechahora);";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@fechaHora", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return true;
                }
            }
            catch (Exception e)
            {

                if (e.Message.Length > 35 && e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    return false;
                }
                else
                {
                    MessageBox.Show(e.Message);

                    return false;
                }

            }
        }

        /// <summary>
        /// Verifica el último registro a tener en cuenta para la suma de facturas del ingeso, tambien es necesario en el funcionamento del POS
        /// </summary>
        /// <returns></returns>
        public static bool arqueoPendiente()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<ProductoModel> productos = new BindableCollection<ProductoModel>();

                    string cadena = $"select * from PrimeraFactura where id = (Select max(id) from PrimeraFactura);";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    string[] rta = new string[3];
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rta =  new string[] { reader["Id"].ToString() , reader["CodigoFactura"].ToString(), reader["InicioFinal"].ToString()  };
                        }
                    }
                    conn.Close();

                    if (rta[2] == "Inicio") return true;
                    if (rta[2] == "Final") return false;
                    return true;


                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Verifica si se han creado facturas despues de la fecha y hora registrada como ultima
        /// </summary>
        /// <returns></returns>
        public static bool faturasPendientes()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<ProductoModel> productos = new BindableCollection<ProductoModel>();

                    string cadena = $"select CodigoFactura as Codigo from FacturasVenta where fecha > (select MAX(fechahora) from PrimeraFactura );";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        //string rta = "";
                        while (reader.Read())
                        {
                            return true;
                           // rta = (string)reader["Codigo"];
                        }
                        
                       // if (rta != null) { return true; }
                        conn.Close();
                        return false;
                    }
 
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Suma el valor de las facturas pendientes para ergistrarlo como el nuevo ingreso
        /// </summary>
        /// <returns></returns>
        public static decimal? valorTotalFacturas()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<ProductoModel> productos = new BindableCollection<ProductoModel>();

                    string cadena = $"select sum(ValorTotal) as Total from FacturasVenta where fecha > (select fechahora from PrimeraFactura where id  = (select max(id) from PrimeraFactura));";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();                    
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        if (string.IsNullOrEmpty(reader["Total"].ToString())) return 0;
                        decimal? resultado = decimal.Parse(reader["Total"].ToString()); 
                        conn.Close();
                        return resultado;                    
                    }
                    
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                Console.WriteLine(e.Message);
                return 0;
            }


        }

        #endregion

        #region Ingresos

        /// <summary>
        /// Registra el nuevo ingreso
        /// </summary>
        /// <param name="Ingreso"></param>
        /// <returns></returns>
        public static bool NuevoIngreso(IngresoModel Ingreso)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"INSERT INTO Ingresos (CodigoIngreso,CodigoPuntoPago,Fecha,Valor,Empleado,PuntoVenta,Efectivo,Diferencia,Supervisor) VALUES (@codigo,@puntoPago,@fecha,@valor,@empleado,@puntoVenta,@efectivo,@diferencia,@supervisor);";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigo", Ingreso.id);
                    cmd.Parameters.AddWithValue("@puntoPago", Ingreso.puntoPago);
                    cmd.Parameters.AddWithValue("@fecha", Ingreso.fecha);
                    cmd.Parameters.AddWithValue("@valor", Ingreso.valor); 
                    cmd.Parameters.AddWithValue("@empleado", Ingreso.cajero.cedula);
                    cmd.Parameters.AddWithValue("@puntoVenta", Ingreso.puntoVenta.codigo);
                    cmd.Parameters.AddWithValue("@efectivo", Ingreso.efectivo);
                    cmd.Parameters.AddWithValue("@diferencia", Ingreso.diferencia);
                    cmd.Parameters.AddWithValue("@supervisor", Ingreso.supervisor.cedula);
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    ///Registra las facturas que componen el ingreso
                    string cadena0 = "insert into ingresoFacturas select dbo.Ingresos.CodigoIngreso, dbo.FacturasVenta.CodigoFactura as f from FacturasVenta, Ingresos where  CodigoIngreso = @ingreso and dbo.FacturasVenta.fecha > (select fechahora from PrimeraFactura where id  = (select max(id) from PrimeraFactura))";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.Parameters.AddWithValue("@ingreso", Ingreso.id);
                    cmd0.ExecuteNonQuery();


                    ///Registra la ultima factura para el proximo arqueo
                    string cadena1 = "insert into PrimeraFactura(CodigoFactura, InicioFinal, FechaHora) values (( select CodigoFactura from FacturasVenta where fecha =  (select MAX(Fecha) from facturasVenta where fecha  > (select fechahora from PrimeraFactura where id  = (select max(id) from PrimeraFactura))) ),'Final', @fechaHora); ";
                    SqlCommand cmd1 = new SqlCommand(cadena1, conn);
                    cmd1.Parameters.AddWithValue("@fechaHora", DateTime.Now);
                    cmd1.ExecuteNonQuery();


                    conn.Close();

                    registrarCambioLocal(Tipo: "Insert", NombreMetodoLocal: "getIngresoConFacturas", PK: $"{Ingreso.id}", NombreMetodoServidor: "ServidorNuevoIngreso", RespuestaExitosaServidor: "true");
                    return true;




                }
            }
            catch (Exception e)
            {

                if (e.Message.Length > 35 && e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    MessageBox.Show("Error: registrando el ingreso.");
                    return false;
                }
                else
                {
                    MessageBox.Show("Error: Local. NuevoIngreso " + e.Message);

                    return false;
                }

            }
        }

        /// <summary>
        /// Obtiene el ingreso registrado con las correspondientes facturas
        /// </summary>
        /// <param name="codigoIngreso"></param>
        /// <returns></returns>
        public static BindableCollection<IngresoModel> getIngresoConFacturas(string codigoIngreso)
        {
            try
            {

                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<IngresoModel> cIngresos = new BindableCollection<IngresoModel>();
                    string cadena = $" select Distinct * from Ingresos where CodigoIngreso = @codigoIngreso order by Fecha;";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigoIngreso", codigoIngreso);
                    conn.Open();
                    bool soloUnValor = true;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!soloUnValor) { MessageBox.Show("Error: Código de ingreso repetido. Contacte a un administrador."); return null; }
                        cIngresos.Clear();
                        while (reader.Read())
                        {
                            soloUnValor = false;
                            IngresoModel ingreso = new IngresoModel
                            {
                                id = reader["CodigoIngreso"].ToString(),   
                                puntoPago = reader["CodigoPuntoPago"].ToString()
                            };
                            ingreso.fecha = DateTime.Parse(reader["Fecha"].ToString());
                            decimal.TryParse(reader["Valor"].ToString(), out decimal total);
                            ingreso.valor = total;
                            ingreso.cajero.cedula = reader["Empleado"].ToString();
                            ingreso.puntoVenta.codigo = reader["PuntoVenta"].ToString();
                            decimal.TryParse(reader["Efectivo"].ToString(), out decimal efectivo);
                            ingreso.efectivo = efectivo;
                            decimal.TryParse(reader["Diferencia"].ToString(), out decimal diferencia);
                            ingreso.diferencia = diferencia;
                            ingreso.supervisor.cedula = reader["Supervisor"].ToString(); 
                            ingreso.facturas = getFacturasIngreso(codigoIngreso);
                            cIngresos.Add(ingreso);
                        }
                    }
                    conn.Close();
                    return cIngresos;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los datos de los productos de la factura con el codigo dado como parametro
        /// </summary>
        /// <param name="codigoFactura"></param>
        /// <returns></returns>
        public static BindableCollection<FacturaModel> getFacturasIngreso(string codigoIngreso)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<FacturaModel> facturas = new BindableCollection<FacturaModel>();

                    string cadena = $"select * from ingresofacturas  where Ingreso = @codigoIngreso;";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigoIngreso", codigoIngreso);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        facturas.Clear();
                        while (reader.Read())
                        {
                            FacturaModel factura = new FacturaModel
                            {
                                codigo = reader["factura"].ToString(),
                            };
                            facturas.Add(factura);
                        }
                    }
                    conn.Close();
                    return facturas;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                Console.WriteLine(e.Message);
                return null;
            }

        }

        #endregion

        #region Registros - Sincornizacion
        /// <summary>
        /// Consulta en la base de datos el ultimo registro actualizado desde el servidor
        /// </summary>
        /// <returns></returns>
        public static int ultimoRegistro()
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {

                    string cadena = $"select MAX(IDUltimoRegistro) from UltimoRegistro";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        //No hay nada entre las comillas pues cuando se utliza la funcion MAX() la columna que muestra  no tiene nombre
                        int respuesta = int.Parse(reader[""].ToString());
                        conn.Close();
                        return respuesta;
                    }

                }
            }
            catch (Exception)
            {
                ////MessageBox.Show(e.Message);
                return 0;
            }
        }

        /// <summary>
        /// Consulta en la base de datos el ultimo registro actualizado desde el servidor
        /// </summary>
        /// <returns></returns>
        public static bool actualizarUltimoRegistro(int a)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {

                    string cadena = $"INSERT INTO UltimoRegistro(IDUltimoRegistro,Fecha) VALUES ({a}, @fecha);";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return true;

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Obtiene los registros locales que se deben actualizar 
        /// </summary>
        /// <returns></returns>
        public static BindableCollection<string[]> registrosLocalesPorActualizar()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"select * from RegistrosCambiosLocales where ID > (select MAX(IdUltimoRegistroSubidoServidor) from RegistrosSubidosServidor ) order BY Id";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    BindableCollection<string[]> resultado = new BindableCollection<string[]>();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string[] reg = new string[6]
                            {
                                reader["ID"].ToString(),
                                reader["Tipo"].ToString(),
                                reader["NombreMetodoLocal"].ToString(),
                                reader["PK"].ToString(),
                                reader["NombreMetodoServidor"].ToString(),
                                reader["RespuestaExitosaServidor"].ToString(),

                            };

                            resultado.Add(reg);
                        }
                    }
                    conn.Close();
                    return resultado;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Guarda en la base de datos el id del nuevo registro actualizado
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static bool registroSubidoAlServidor(int a)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {

                    string cadena = $"INSERT INTO RegistrosSubidosServidor(IdUltimoRegistroSubidoServidor,Fecha) VALUES ({a}, @fecha);";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return true;

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }


        /// <summary>
        /// Registrar los cambios echos en la base de datos local.
        /// </summary>
        /// <param name="NombreMetodoLocal"></param>
        /// <param name="PK"></param>
        /// <param name="NombreMetodoServidor"></param>
        /// <param name="RespuestaExitosaServidor"></param>
        /// <param name="Tipo"></param>
        /// <returns></returns>
        public static bool registrarCambioLocal(string NombreMetodoLocal, string PK, string NombreMetodoServidor, string RespuestaExitosaServidor, string Tipo)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"INSERT INTO RegistrosCambiosLocales(NombreMetodoLocal,PK,NombreMetodoServidor,RespuestaExitosaServidor,Tipo) VALUES ('{NombreMetodoLocal}','{PK}','{NombreMetodoServidor}','{RespuestaExitosaServidor}','{Tipo}');";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return true;
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
                return false;

            }
        }

        /// <summary>
        /// Inserta en la base de datos el ID de un registro que no pudo ser actualizado.
        /// </summary>
        /// <param name="CodigoRegistro"></param>
        /// <returns></returns>
        public static bool registrarCambioSinGuardar(string CodigoRegistro, string tipo)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"INSERT INTO RegistrosSinGuardar(ID,Tipo) VALUES ('{CodigoRegistro}','{tipo}');";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return true;
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
                return false;

            }
        }


        #endregion

        /// <summary>
        /// Return the connection string from the App.config file of the giving name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static string ConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        /// <summary>
        /// Retorna el codigo del local al que pertenece la maquina, esto afectara las ventas por local y el inventario del local con el codigo especificao, dicho codigo se almacena en el archivo App.config a fin de que pueda especificarce
        /// el local en el que esta ubicada la maquina.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string getLocalUbicacion()
        {
            return ConfigurationManager.AppSettings["CodigoDelLocalCambiosInventario"];
        }

    }
}


