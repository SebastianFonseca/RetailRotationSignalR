using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Text;
using System.Windows;
using Client.Main.Models;
using static Client.Main.ViewModels.AddClientViewModel;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Replication;
using System.Threading.Tasks;
using Dotmim.Sync.SqlServer;
using Dotmim.Sync;

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
                            string cadena2 = "INSERT INTO Producto(CodigoProducto, Nombre, UnidadVenta,	UnidadCompra, PrecioVenta, Seccion, FechaVencimiento, IVA, CodigoBarras, Estado)" +
                                " VALUES (@codigo,@nombre,@univenta,@unicompra,@precio,@seccion,@fv,@iva,@cb, 'Activo')";
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
                            string cadena = $"UPDATE Producto SET  Nombre=@nombre, UnidadVenta=@univenta,	UnidadCompra=@unicompra, PrecioVenta=@precio, Seccion=@seccion, FechaVencimiento=@fv, IVA=@iva, CodigoBarras=@cb WHERE CodigoProducto = '{Producto.codigoProducto}' ";
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
                            local.numeroDeCanastillas = Int16.Parse(reader["NumeroCanastillas"].ToString());
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
                    string response = InsertarExistenciaProducto(existencia.codigo, existencia.productos);
                    if (response == "Y")
                    {
                        conn.Close();
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
        /// Inserta en la base de datos, en la tabla ExisteciaProducto los datos de dichos obejetos.
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="productos"></param>
        /// <returns></returns>
        public static string InsertarExistenciaProducto(string codigo, BindableCollection<ProductoModel> productos)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    ///Si la operacion no termino exitosamente en un intento anterior, la realiza desde cero borrando los registros insertados en el anterior intento.
                    string cadena0 = $"delete from ExistenciaProducto where CodigoExistencia = '{codigo}';";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.ExecuteNonQuery();
                    foreach (ProductoModel producto in productos)
                    {
                        string cadena = "INSERT INTO ExistenciaProducto(CodigoExistencia,CodigoProducto, Cantidad) VALUES (@cod,@codProd,@existencia);";
                        SqlCommand cmd = new SqlCommand(cadena, conn);
                        cmd.Parameters.AddWithValue("@cod", codigo);
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

                    string cadena = $"INSERT INTO UltimoRegistro(IDUltimoRegistro) VALUES ({a});";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return true;

                }
            }
            catch (Exception)
            {
                return false;               
            }
        }

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


