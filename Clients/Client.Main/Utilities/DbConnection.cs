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
                    string cadena = "SELECT *  FROM EMPLEADO where [CedulaEmpleado]=@user";
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
                                EmpleadoModel persona = new EmpleadoModel();
                                persona.cedula = reader["CedulaEmpleado"].ToString();
                                persona.puntoDeVenta.codigo = reader["CodigoPuntoVenta"].ToString();
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
                return new[] { "Exception" };
            }







            //try {

            //    using (IDbConnection conn = new SqlConnection(_connString))
            //    {

            //        var parameters = new DynamicParameters();
            //        parameters.Add("@cedula", User);
            //        MessageBox.Show(Statics.Hash(Password));
            //        parameters.Add("@password", Statics.Hash(Password));
            //        parameters.Add(name: "@RetVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            //        var returnCode = conn.Execute(sql: "Login", param: parameters, commandType: CommandType.StoredProcedure);
            //        if (parameters.Get<Int32>("@RetVal") == 0)
            //            return false;
            //        else
            //            return true;
            //    }

            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message);
            //    return false;
            //}

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
                            cmd2.Parameters.AddWithValue("@precio", Producto.precioVenta.ToString());
                            cmd2.Parameters.AddWithValue("@seccion", Statics.PrimeraAMayuscula(Producto.seccion));
                            cmd2.Parameters.AddWithValue("@fv", Producto.fechaVencimiento == DateTime.MinValue ? (object)DBNull.Value : Producto.fechaVencimiento);
                            cmd2.Parameters.AddWithValue("@iva", Producto.iva);
                            cmd2.Parameters.AddWithValue("@cb", string.IsNullOrEmpty(Producto.codigoBarras) ? (object)DBNull.Value : Producto.codigoBarras);
                            cmd2.Parameters.AddWithValue("@fc", Producto.factorConversion);
                            cmd2.ExecuteNonQuery();
                            conn.Close();
                            return true;
                            ///return $"Nuevo producto: {Producto.codigoProducto}- {Producto.nombre}.";
                        }
                    }
                }
            }
            catch (Exception e)
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
                            string cadena = $"UPDATE Producto SET  Nombre=@nombre, UnidadVenta=@univenta,	UnidadCompra=@unicompra, PrecioVenta=@precio, Seccion=@seccion, FechaVencimiento=@fv, IVA=@iva, CodigoBarras=@cb, FactorConversion = @fc WHERE CodigoProducto = '{Producto.codigoProducto}' ";
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
                            cmd.ExecuteNonQuery();
                            conn.Close();
                            return true;
                            //return "Producto actualizado.";
                        }
                    }
                }

            }
            catch (Exception e)
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
                            ProductoModel producto = new ProductoModel();
                            producto.codigoProducto = reader["CodigoProducto"].ToString();
                            producto.nombre = reader["Nombre"].ToString();
                            producto.unidadCompra = reader["UnidadCompra"].ToString();
                            producto.unidadVenta = reader["UnidadVenta"].ToString();
                            producto.precioVenta = Convert.ToDecimal(reader["PrecioVenta"].ToString());
                            producto.seccion = reader["Seccion"].ToString();
                            producto.iva = Convert.ToDecimal(reader["IVA"].ToString());
                            producto.codigoBarras = reader["CodigoBarras"].ToString();
                            producto.factorConversion = decimal.Parse(reader["FactorConversion"].ToString());
                            if (reader["FechaVencimiento"].ToString() == "")
                            {
                                producto.fechaVencimiento = DateTime.MinValue;
                            }
                            else
                            {
                                producto.fechaVencimiento = DateTime.Parse(reader["FechaVencimiento"].ToString());
                            }
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
                    cmd.Parameters.AddWithValue("@cedula", proveedor.cedula);
                    cmd.Parameters.AddWithValue("@nombre", proveedor.firstName);
                    cmd.Parameters.AddWithValue("@apellidos", proveedor.lastName);
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
                            ProveedorModel proveedor = new ProveedorModel();
                            proveedor.cedula = reader["CedulaProveedor"].ToString();
                            proveedor.firstName = reader["Nombres"].ToString();
                            proveedor.lastName = reader["Apellidos"].ToString();
                            proveedor.telefono = reader["Telefono"].ToString();
                            proveedor.ciudad = reader["Ciudad"].ToString();
                            proveedor.direccion = reader["Direccion"].ToString();
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
                            ProveedorModel proveedor = new ProveedorModel();
                            proveedor.cedula = reader0["CedulaProveedor"].ToString();
                            proveedor.firstName = reader0["Nombres"].ToString();
                            proveedor.lastName = reader0["Apellidos"].ToString();
                            proveedor.telefono = reader0["Telefono"].ToString();
                            proveedor.ciudad = reader0["Ciudad"].ToString();
                            proveedor.direccion = reader0["Direccion"].ToString();
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
         /// Obtiene la lsita de todos los proveedores.
         /// </summary>
         /// <returns></returns>
        public static BindableCollection<ProveedorModel> getProveedores()
        {

            try
            {
                BindableCollection<ProveedorModel> proveedores = new BindableCollection<ProveedorModel>();
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"SELECT Distinct * FROM Proveedor ORDER BY Nombres ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        proveedores.Clear();
                        while (reader.Read())
                        {
                            ProveedorModel proveedor = new ProveedorModel();
                            proveedor.cedula = reader["CedulaProveedor"].ToString();
                            proveedor.firstName = reader["Nombres"].ToString();
                            proveedor.lastName = reader["Apellidos"].ToString();
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

                            ProductoModel producto = new ProductoModel();
                            producto.codigoProducto = reader0["CodigoProducto"].ToString();
                            producto.nombre = reader0["Nombre"].ToString();
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
                    cmd.Parameters.AddWithValue("@nombre", proveedor.firstName);
                    cmd.Parameters.AddWithValue("@apellidos", proveedor.lastName);
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
                            EmpleadoModel persona = new EmpleadoModel();
                            persona.cedula = reader["CedulaEmpleado"].ToString();
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
                            LocalModel local = new LocalModel();
                            local.codigo = reader["codigoPuntoVenta"].ToString();
                            local.nombre = reader["Nombres"].ToString();
                            local.direccion = reader["Direccion"].ToString();
                            local.telefono = reader["Telefono"].ToString();
                            local.ciudad = reader["Ciudad"].ToString();
                            local.numeroDeCanastillas = Int32.Parse(reader["NumeroCanastillas"].ToString());
                            local.fechaDeApertura = DateTime.Parse(reader["FechaDeApertura"].ToString());
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
                    string cadena = "SELECT [CodigoPuntoVenta], [Nombres]  FROM PuntoVenta";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            LocalModel local = new LocalModel();
                            local.codigo = reader["CodigoPuntoVenta"].ToString();
                            local.nombre = reader["Nombres"].ToString();
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
                        registrarCambioLocal(NombreMetodoLocal: "getExistenciasConProductos", PK: $"{existencia.codigo}", NombreMetodoServidor: "ServidorNuevaExistencia", RespuestaExitosaServidor: "Se ha registrado el nuevo documento.", Tipo:"Insert");
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
                    return false;
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

                    string cadena = $" select * from ExistenciasFisicas where FechaExistencia = '{fecha.ToString("yyyy-MM-dd")}' or CodigoExistencia like '%{Caracteres}%'  or CodigoExistencia like '______________{Caracteres}' ORDER BY CodigoExistencia;";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cExistencias.Clear();
                        while (reader.Read())
                        {
                            ExistenciasModel exist = new ExistenciasModel();
                            exist.codigo = reader["CodigoExistencia"].ToString();
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
                            ProductoModel producto = new ProductoModel();
                            producto.codigoProducto = reader["codigoproducto"].ToString();
                            producto.nombre = reader["nombre"].ToString();
                            producto.unidadVenta = reader["unidadventa"].ToString();
                            producto.existencia = Int32.Parse(reader["cantidad"].ToString());

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
                            ExistenciasModel exist = new ExistenciasModel();
                            exist.codigo = reader["CodigoExistencia"].ToString();
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
                            ExistenciasModel exist = new ExistenciasModel();
                            exist.codigo = reader["CodigoExistencia"].ToString();
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
                        registrarCambioLocal(NombreMetodoLocal: "getPedidoConProductos", PK: $"{pedido.codigo}", NombreMetodoServidor: "ServidorNuevoPedido", RespuestaExitosaServidor: "Se ha registrado el nuevo documento.", Tipo: "Insert");
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
                    return false;
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

                    string cadena = $" select * from Pedido where FechaPedido = '{fecha.ToString("yyyy-MM-dd")}' or CodigoPedido like '%{Caracteres}%'  or CodigoPedido like '______________{Caracteres}' ORDER BY CodigoPedido;";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cPedidos.Clear();
                        while (reader.Read())
                        {
                            PedidoModel ped = new PedidoModel();
                            ped.codigo = reader["CodigoPedido"].ToString();
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
                    string cadena = $"select  producto.codigoproducto, producto.Nombre, producto.unidadventa, producto.unidadcompra, PedidoProducto.Cantidad, producto.factorconversion,ExistenciaProducto.Cantidad as ExistenciaCantidad from  producto join pedidoproducto on producto.codigoproducto = PedidoProducto.CodigoProducto join ExistenciaProducto on ExistenciaProducto.CodigoProducto = Producto.CodigoProducto where pedidoproducto.codigopedido = '{codigoPedido}' and CodigoExistencia = '{codigoPedido.Split(':')[0]}' and estado = 'Activo' order by ExistenciaProducto.CodigoProducto; ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        productos.Clear();
                        while (reader.Read())
                        {
                            ProductoModel producto = new ProductoModel();
                            producto.codigoProducto = reader["codigoproducto"].ToString();
                            producto.nombre = reader["nombre"].ToString();
                            producto.unidadVenta = reader["unidadventa"].ToString();
                            producto.unidadCompra = reader["unidadcompra"].ToString();
                            producto.factorConversion = decimal.Parse(reader["factorconversion"].ToString());
                            producto.existencia = Int32.Parse(reader["ExistenciaCantidad"].ToString());
                            producto.pedido = Int32.Parse(reader["cantidad"].ToString());

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
                            PedidoModel ped = new PedidoModel();
                            ped.codigo = reader["CodigoPedido"].ToString();
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
        public static BindableCollection<PedidoModel> getPedidoConProductos()
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
                            PedidoModel ped = new PedidoModel();
                            ped.codigo = reader["CodigoPedido"].ToString();
                            ped.responsable.cedula = reader["CedulaEmpleado"].ToString();
                            ped.responsable.firstName = reader["Nombres"].ToString();
                            ped.responsable.lastName = reader["Apellidos"].ToString();

                            ped.fecha = DateTime.Parse(reader["FechaPedido"].ToString());
                            ped.puntoVenta.codigo = reader["CodigoPuntoVenta"].ToString();
                            ped.puntoVenta.nombre = reader["name"].ToString();

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
        /// Inserta en la base de datos, en la tabla RegistroCompra los datos de dicho documento.
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="productos"></param>
        /// <returns></returns>
        public static string InsertarRegistroCompraProducto(ComprasModel compra )
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

                    string cadena = $"UPDATE RegistroCompra SET CedulaProveedor = @prov, CantidadComprada = @cant, PrecioCompra= @precio WHERE CodigoCompra='{compra.codigo}' and CodigoProducto = '{compra.productos[0].codigoProducto}'";
                    SqlCommand cmd = new SqlCommand(cadena, conn);

                    cmd.Parameters.AddWithValue("@prov", string.IsNullOrEmpty(compra.productos[0].proveedor.cedula)? (object)DBNull.Value : compra.productos[0].proveedor.cedula);
                    cmd.Parameters.AddWithValue("@cant", string.IsNullOrEmpty(compra.productos[0].compra.ToString()) ? (object)DBNull.Value : compra.productos[0].compra);
                    cmd.Parameters.AddWithValue("@precio", string.IsNullOrEmpty(compra.productos[0].precioCompra.ToString()) ? (object)DBNull.Value : compra.productos[0].precioCompra.ToString());

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

                    MessageBox.Show(e.Message);
                    return false;
                
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

                    string cadena = $" select CodigoCompra, FechaCompra,NumeroCanastillas,peso,Empleado.CedulaEmpleado, Nombres, Apellidos from Compras join empleado on compras.CedulaEmpleado = Empleado.CedulaEmpleado where FechaCompra = '{fecha.ToString("yyyy-MM-dd")}' or CodigoCompra like '%{Caracteres}%' ORDER BY CodigoCompra;";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cCompras.Clear();
                        while (reader.Read())
                        {
                            ComprasModel comp = new ComprasModel();

                            comp.codigo = reader["CodigoCompra"].ToString();
                            comp.responsable.cedula = reader["CedulaEmpleado"].ToString();
                            comp.responsable.firstName = reader["Nombres"].ToString();
                            comp.responsable.lastName = reader["Apellidos"].ToString();
                            comp.fecha = DateTime.Parse(reader["FechaCompra"].ToString());
                            int i;
                            Int32.TryParse(reader["NumeroCanastillas"].ToString(), out i);
                            comp.numeroCanastillas = i;
                            Int32.TryParse(reader["peso"].ToString(),out i);
                            comp.peso = i;
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
                    string cadena = $"select producto.codigoproducto, producto.Nombre, producto.unidadcompra,  pedido, CantidadComprada, PrecioCompra, CedulaProveedor from producto join RegistroCompra on Producto.CodigoProducto = RegistroCompra.CodigoProducto where CodigoCompra = '{codigoCompra}' and producto.Estado = 'Activo' order by codigoproducto ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        productos.Clear();
                        while (reader.Read())
                        {
                            ProductoModel producto = new ProductoModel();
                            producto.codigoProducto = reader["codigoproducto"].ToString();
                            producto.nombre = reader["nombre"].ToString();
                            producto.unidadCompra = reader["unidadcompra"].ToString();
                            producto.sumaPedido = Int32.Parse(reader["pedido"].ToString());
                            MessageBox.Show(reader["CantidadComprada"].ToString());
                            
                            int a;
                            if (Int32.TryParse(reader["CantidadComprada"].ToString(), out a))
                            {
                                producto.compra = a;
                            }
                            else
                            {
                                producto.compra = null;
                            }

                            decimal b;
                            if (decimal.TryParse(reader["PrecioCompra"].ToString(), out b))
                            {
                                producto.precioCompra = b;
                            }
                            else
                            {
                                producto.precioCompra = null;
                            }
                            producto.proveedor.cedula = reader["CedulaProveedor"].ToString();
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
                    string cadena = "INSERT INTO Clientes(Nombres,Apellidos,CedulaCliente,Email,Telefono,Puntos) VALUES (@name,@lastname,@cedula,@correo,@telefono, 100 )";
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
        /// Variable que retorna el metodo getClientes().
        /// </summary>
        public static BindableCollection<ClientesModel> cli = new BindableCollection<ClientesModel>();

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

                    string cadena = $"SELECT * FROM Clientes WHERE ( CedulaCliente like '%{Caracteres}%' ) or ( Nombres like '%{Caracteres}%' ) or ( Apellidos like '%{Caracteres}%' )  ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cli.Clear();
                        while (reader.Read())
                        {
                            ClientesModel cliente = new ClientesModel();
                            cliente.firstName = reader["Nombres"].ToString();
                            cliente.lastName = reader["Apellidos"].ToString();
                            cliente.cedula = reader["CedulaCliente"].ToString();
                            cliente.correo = reader["Email"].ToString();
                            cliente.telefono = reader["Telefono"].ToString();
                            cliente.Puntos = Int32.Parse(reader["Puntos"].ToString());
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
        /// <param name="CC"></param>
        /// <returns></returns>
        public static bool ActualizarCliente(ClientesModel Cliente, string CC)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {


                    string cadena = "UPDATE Clientes SET Nombres=@name, Apellidos=@lastname, CedulaCliente=@cedula, Email=@correo, Telefono=@telefono, Puntos=@puntos WHERE CedulaCliente = @AntiguaCedula ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@name", Statics.PrimeraAMayuscula(Cliente.firstName));
                    cmd.Parameters.AddWithValue("@lastname", Statics.PrimeraAMayuscula(Cliente.lastName));
                    cmd.Parameters.AddWithValue("@cedula", Cliente.cedula);
                    cmd.Parameters.AddWithValue("@correo", string.IsNullOrEmpty(Cliente.correo) ? (object)DBNull.Value : Cliente.correo);
                    cmd.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(Cliente.telefono) ? (object)DBNull.Value : Cliente.telefono);
                    cmd.Parameters.AddWithValue("@puntos", Cliente.Puntos);
                    cmd.Parameters.AddWithValue("@AntiguaCedula", CC);
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

                    string cadena = $"delete from Clientes Where CedulaCliente = '{Cedula}' ";
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
            catch (Exception e)
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
                    string cadena = $"select * from RegistrosCambiosLocales where ID > (select MAX(IdUltimoRegistroSubidoServidor) from RegistrosSubidosServidor )";
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



    }
}


