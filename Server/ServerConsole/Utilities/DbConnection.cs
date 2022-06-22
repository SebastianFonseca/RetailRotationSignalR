﻿using Caliburn.Micro;
using ServerConsole.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace ServerConsole.Utilities
{
    class DbConnection
    {
        /// <summary>
        /// Cadena de conexion obtenida del archivo de configuracion.
        /// </summary>
        private static string _connString = ConnectionString("RetailRotationServerDataBase");

        /// <summary>
        ///  Metodo responsable de verificar en la base de datos  si el usuario que intenta ingresar esta o no registrado.
        /// </summary>
        /// <param name="User">Usuario</param>
        /// <param name="Password">Contraseña</param>
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
                Console.WriteLine(e.Message);
                return new[] { "Exception" };
            }

            //try
            //{

            //    using (IDbConnection conn = new SqlConnection(_connString))
            //    {

            //        var parameters = new DynamicParameters();
            //        parameters.Add("@cedula", User);
            //        parameters.Add("@password", Password);
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
            //    Console.WriteLine(e.Message);
            //    return false;
            //}

        }

        #region Producto
        /// <summary>
        /// Registra en la base de datos el nuevo producto.
        /// </summary>
        /// <param name="Producto">Instancia de la clase ProductoModel</param>
        /// <returns></returns>
        public static string NuevoProducto(ProductoModel Producto)
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
                            return "Codigo de producto ya registrado.";
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
                            return "Codigo de barras ya registrado.";
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
                            return "Nombre de producto ya registrado.";
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
                            cmd2.Parameters.AddWithValue("@fv", Producto.fechaVencimiento == DateTime.Today ? (object)DBNull.Value : Producto.fechaVencimiento);
                            cmd2.Parameters.AddWithValue("@iva", Producto.iva);
                            cmd2.Parameters.AddWithValue("@cb", string.IsNullOrEmpty(Producto.codigoBarras) ? (object)DBNull.Value : Producto.codigoBarras);
                            cmd2.Parameters.AddWithValue("@fc", Producto.factorConversion);
                            cmd2.ExecuteNonQuery();
                            Registrar("Insert", "ServidorGetProductos", Producto.codigoProducto, "ProductoModel[]", "NuevoProducto", "codigoProducto");
                            conn.Close();
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("\n\t" + DateTime.Now + ": ");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine($"El usuario {RetailHUB.usuarioConectado} registro un nuevo producto: {Producto.codigoProducto} - {Producto.nombre}.");
                            return $"El usuario {RetailHUB.usuarioConectado} registro un nuevo producto: {Producto.codigoProducto} - {Producto.nombre}.";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write("\n\t" + DateTime.Now + ": ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(e.Message);
                return e.Message;
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
                   // Console.WriteLine("Se consultaron los productos.");
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
        /// Obtener datos de los productos en la base de datos que coincide con los caracteres dados.
        /// </summary>
        /// <returns></returns>
        public static BindableCollection<ProductoModel> getProductos(string caracteres)
        {

            productos.Clear();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"SELECT Distinct * FROM Producto WHERE ((CodigoProducto LIKE '%{caracteres}%') OR (Nombre LIKE '%{caracteres}%')) AND Estado = 'Activo' ORDER BY Nombre ;";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ProductoModel producto = new ProductoModel();
                            producto.codigoProducto = reader["CodigoProducto"].ToString();
                            producto.nombre = reader["Nombre"].ToString();
                            producto.unidadVenta = reader["UnidadVenta"].ToString();
                            producto.unidadCompra = reader["UnidadCompra"].ToString();
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
                            //Console.WriteLine($"se consulto el producto {producto.nombre}");
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
        /// Elimina de la base de datos la informacion del producto con el ID dado como parametro.
        /// </summary>
        /// <param name="idProducto"></param>
        /// <returns></returns>
        public static string deleteProducto(string idProducto)
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
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write("\n\t" + DateTime.Now + "--");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($" {RetailHUB.usuarioConectado}: Ha eliminado al producto con codigo: {idProducto}");

                    Registrar("Delete", "", idProducto, "", "deleteProducto", "codigoProducto");
                    return "Se ha eliminado al producto.";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return e.Message;
            }

        }

        /// <summary>
        /// Actualiza en la base de datos la informacion relacionada al producto dado.
        /// </summary>
        /// <param name="Producto"></param>
        /// <returns></returns>
        public static string actualizarProducto(ProductoModel Producto)
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
                            return "Codigo de barras ya registrado.";
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
                            return "Nombre de producto ya registrado.";
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
                            cmd.Parameters.AddWithValue("@fv", Producto.fechaVencimiento == DateTime.Today ? (object)DBNull.Value : Producto.fechaVencimiento);
                            cmd.Parameters.AddWithValue("@iva", Producto.iva);
                            cmd.Parameters.AddWithValue("@cb", string.IsNullOrEmpty(Producto.codigoBarras) ? (object)DBNull.Value : Producto.codigoBarras);
                            cmd.Parameters.AddWithValue("@fc", Producto.factorConversion);
                            cmd.ExecuteNonQuery();
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("\n\t" + DateTime.Now + "--");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine($"{RetailHUB.usuarioConectado}: Ha Actualizado al producto {Producto.codigoProducto} - {Producto.nombre}");
                            conn.Close();
                            Registrar("Update", "ServidorGetProductos", Producto.codigoProducto, "ProductoModel[]", "actualizarProducto", "codigoProducto");
                            return "Producto actualizado.";
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return e.Message;
            }
        }

        #endregion

        #region Proveedor

        /// <summary>
        ///     Metodo encargado de ejecutar el query insert del nuevo proveedor en la base de datos.
        /// </summary>
        /// <param name="proveedor"></param>
        /// <returns></returns>
        public static string NuevoProveedor(ProveedorModel proveedor)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = "INSERT INTO Proveedor(CedulaProveedor,Nombres,Apellidos,Telefono,Ciudad,Direccion, Estado) VALUES " +
                                                        "(@cedula,@nombre,@apellidos,@telefono,@ciudad,@direccion,'Activo');";
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
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write("\n\t" + DateTime.Now + "--");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($" {RetailHUB.usuarioConectado}: Ha registrado un nuevo proveedor {proveedor.firstName} {proveedor.lastName}");
                        Registrar("Insert", "ServidorGetProveedor", proveedor.cedula, "ProveedorModel[]", "NuevoProveedor", "cedula");
                        conn.Close();
                        return "Se ha registrado al nuevo proveedor.";
                    }
                    ///Si fallo la incersion de los productos se borra el registro del proveedor para que en un siguiente intento de registro no se repita la clave primaria.
                    string cadena0 = $"delete from Proveedor Where CedulaProveedor = '{proveedor.cedula}' ";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return response;
                }
            }
            catch (Exception e)
            {
                if (e.Message.Substring(0, 50) == $"Violation of PRIMARY KEY constraint 'PK_Proveedor'")
                {
                    return "Proveedor ya registrado.";
                }
                else
                {
                    return e.Message;
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

                    string cadena0 = $"SELECT Distinct Proveedor.CedulaProveedor,Proveedor.Nombres, Proveedor.Apellidos, Proveedor.Telefono, Proveedor.Ciudad, Proveedor.Direccion FROM proveedor  JOIN ProveedorProducto ON ProveedorProducto.CedulaProveedor = Proveedor.CedulaProveedor JOIN Producto ON ProveedorProducto.CodigoProducto = Producto.CodigoProducto WHERE Producto.Nombre LIKE '%{Caracteres}%' AND Proveedor.Estado = 'Activo';";
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
        public static BindableCollection<ProveedorModel> getProveedor(string Cedula)
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
                    return new BindableCollection<ProveedorModel>() { proveedor };
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
                    string cadena = $"SELECT * FROM Proveedor where Estado = 'Activo' ORDER BY Nombres ";
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
        /// Elimina de la base de datos el proveedor con el número de cédula dado.
        /// </summary>
        /// <param name="cedula"></param>
        /// <returns></returns>
        public static string deleteProveedor(string Cedula)
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
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write("\n\t" + DateTime.Now + "--");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($" {RetailHUB.usuarioConectado}: Ha eliminado al proveedor {Cedula}");
                    Registrar("Delete", "", Cedula, "", "deleteProveedor", "cedula");
                    return "Se ha eliminado al proveedor.";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return e.Message;
            }
        }

        /// <summary>
        /// Actualiza en la base de datos la informacion relacionada con el proveedor proporcionado como parametro,
        /// </summary>
        /// <param name="proveedor"></param>
        /// <returns></returns>
        public static string actualizarProveedor(ProveedorModel proveedor)
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
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write("\n\t" + DateTime.Now + "--");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($" {RetailHUB.usuarioConectado}: Ha editado la información del proveedor {proveedor.firstName} {proveedor.lastName}");
                        conn.Close();
                        Registrar("Update", "ServidorGetProveedor", proveedor.cedula, "ProveedorModel[]", "actualizarProveedor", "cedula");
                        return "Se ha editado la informacion.";
                    }
                    return response;
                }
            }
            catch (Exception e)
            {
                if (e.Message.Substring(0, 50) == $"Violation of PRIMARY KEY constraint 'PK_Proveedor'")
                {
                    return "Proveedor ya registrado.";
                }
                else
                {
                    return e.Message;
                }
            }
        }

        #endregion

        #region Usuarios

        /// <summary>
        /// Metodo encargado de ejecutar el query insert del nuevo empleado en la base de datos local.
        /// </summary>
        /// <param name="Empleado">Instancia de la clase Empleado.</param>
        /// <returns></returns>
        public static string NuevoUsuario(EmpleadoModel Empleado)
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
                    cmd.Parameters.AddWithValue("@contraseña", Statics.Hash(Empleado.password));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write("\n\t" + DateTime.Now + "--");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($" {RetailHUB.usuarioConectado}: Ha registrado un nuevo usuario {Empleado.firstName} {Empleado.lastName}");
                    conn.Close();
                    Registrar("Insert", "ServidorGetUsuarios", Empleado.cedula, "EmpleadoModel[]", "NuevoUsuario", "cedula");
                    return "Se ha registrado al nuevo usuario.";
                }
            }
            catch (Exception e)
            {
                if (e.Message.Substring(0, 50) == $"Violation of PRIMARY KEY constraint 'PK_Empleado'.")
                {
                    return "Empleado ya registrado.";
                }
                else
                {
                    return e.Message;
                }
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
                    string cadena = $"SELECT Distinct * FROM EMPLEADO WHERE (( CedulaEmpleado like '%{Caracteres}%' ) or ( Nombres like '%{Caracteres}%' ) or ( Apellidos like '%{Caracteres}%' )) AND Estado = 'Activo' ORDER BY Nombres;";
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
                            persona.salario = Convert.ToDecimal(reader["Salario"].ToString());
                            persona.telefono = reader["Telefono"].ToString();
                            persona.cargo = reader["Cargo"].ToString();
                            persona.direccion = reader["Direccion"].ToString();
                            persona.password = reader["Password"].ToString();
                            emp.Add(persona);
                        }
                    }
                    conn.Close();
                    return emp;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Elimina al empleado del número de cedúla dado.
        /// </summary>
        /// <param name="Cedula">Número de cédula del empleado a eliminar. </param>
        /// <returns></returns>
        public static string DeleteEmpleado(string Cedula)
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
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write("\n\t" + DateTime.Now + "--");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($" {RetailHUB.usuarioConectado}: Ha eliminado al usuario {Cedula}");
                    Registrar("Delete", "", Cedula, "", "deleteEmpleado", "cedula");
                    return "Se ha eliminado al usuario.";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return e.Message;
            }
        }

        /// <summary>
        /// Actualiza el empleado del numero de cedula dado.
        /// </summary>
        /// <param name="Empleado"></param>
        /// <param name="CC"></param>
        /// <returns></returns>
        public static string ActualizarUsuario(EmpleadoModel Empleado, string CC)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {


                    string cadena = "UPDATE Empleado SET CedulaEmpleado = @NuevoCedula, CodigoPuntoVenta = @puntodeventa, Nombres = @nombre, Apellidos = @apellidos, FechaContratacion = @fecha, Salario =@salario, Telefono = @telefono, Cargo = @cargo," +
                                    "Password = @contraseña, Direccion = @direccion WHERE CedulaEmpleado = @AntiguoCedula ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@NuevoCedula", Empleado.cedula);
                    cmd.Parameters.AddWithValue("@puntodeventa", Empleado.puntoDeVenta.codigo);
                    cmd.Parameters.AddWithValue("@nombre", Statics.PrimeraAMayuscula(Empleado.firstName));
                    cmd.Parameters.AddWithValue("@apellidos", Statics.PrimeraAMayuscula(Empleado.lastName));
                    cmd.Parameters.AddWithValue("@fecha", Empleado.fechaDeContratacion.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@salario", Empleado.salario);
                    cmd.Parameters.AddWithValue("@telefono", Empleado.telefono);
                    cmd.Parameters.AddWithValue("@direccion", Empleado.direccion);
                    cmd.Parameters.AddWithValue("@cargo", Empleado.cargo.Substring(37));
                    cmd.Parameters.AddWithValue("@contraseña", Statics.Hash(Empleado.password));
                    cmd.Parameters.AddWithValue("@AntiguoCedula", CC);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write("\n\t" + DateTime.Now + "--");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{RetailHUB.usuarioConectado}: Ha Actualizado al usuario {Empleado.firstName} {Empleado.lastName}");
                    conn.Close();
                    Registrar("Update", "ServidorGetUsuarios", CC, "EmpleadoModel[]", "ActualizarUsuario", "cedula");
                    return "Usuario actualizado";

                }
            }
            catch (Exception e)
            {

                if (e.Message.Substring(0, 50) == $"Violation of PRIMARY KEY constraint 'PK_Empleado'.")
                {
                    return "Cedula ya registrada.";
                }
                else
                {
                    return e.Message;
                }
            }
        }
        #endregion

        #region Local

        /// <summary>
        /// Registra en la base de datos un uevo local.
        /// </summary>
        /// <param name="NuevoLocal">Instancia de la clase LocalModel</param>
        /// <returns></returns>
        public static string NuevoLocal(LocalModel NuevoLocal)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {

                    string cadena0 = "SELECT *  FROM PuntoVenta where [Nombres]=@nombre";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.Parameters.AddWithValue("@nombre", NuevoLocal.nombre);
                    conn.Open();
                    using (SqlDataReader reader0 = cmd0.ExecuteReader())
                    {
                        if (reader0.HasRows)
                        {
                            conn.Close();
                            return "El nombre ya registrado.";
                        }
                    }
                    string cadena = "INSERT INTO PuntoVenta(Nombres,Direccion,Telefono,Ciudad,NumeroCanastillas,FechaDeApertura,Estado) " +
                                    "VALUES (@nombre,@direccion,@telefono,@ciudad,@nrocanastillas,@fechaapertura,'Activo')";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    //cmd.Parameters.AddWithValue("@codigo", NuevoIdLocal());
                    cmd.Parameters.AddWithValue("@nombre", Statics.PrimeraAMayuscula(NuevoLocal.nombre));
                    cmd.Parameters.AddWithValue("@direccion", Statics.PrimeraAMayuscula(NuevoLocal.direccion));
                    cmd.Parameters.AddWithValue("@telefono", Statics.PrimeraAMayuscula(NuevoLocal.telefono));
                    cmd.Parameters.AddWithValue("@ciudad", Statics.PrimeraAMayuscula(NuevoLocal.ciudad));
                    cmd.Parameters.AddWithValue("@nrocanastillas", Statics.PrimeraAMayuscula(NuevoLocal.numeroDeCanastillas.ToString()));
                    cmd.Parameters.AddWithValue("@fechaapertura", Statics.PrimeraAMayuscula(NuevoLocal.fechaDeApertura.Date.ToString("yyyy-MM-dd")));
                    //conn.Open();
                    cmd.ExecuteNonQuery();
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write("\n\t" + DateTime.Now + "-- ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{RetailHUB.usuarioConectado} Ha registrado el nuevo local: {NuevoLocal.nombre }");
                    conn.Close();
                    Registrar("Insert", "ServidorGetLocales", LocalId(NuevoLocal.nombre), "LocalModel[]", "NuevoLocal", "codigo");

                    return $"Se ha registrado el nuevo local: {NuevoLocal.nombre }";

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                if (e.Message.Substring(0, 52) == $"Violation of PRIMARY KEY constraint 'PK_PuntoVenta'.")
                {
                    //Console.WriteLine($"El nombre {NuevoLocal.Nombre} ya esta registrado.");
                    return $"El codigo ya esta registrado.";
                }
                else
                {
                    //Console.WriteLine(e.Message);
                    return e.Message;
                }
            }
        }

        /// <summary>
        /// Obtiene el id del local con el nombre dado como parametro.
        /// </summary>
        /// <param name="Caracteres"></param>
        /// <returns></returns>
        public static string LocalId(string Caracteres)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {

                    string cadena = $"SELECT codigoPuntoVenta FROM PuntoVenta WHERE Nombres = '{Caracteres}' ANd Estado = 'Activo' ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    string rta;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        rta = reader["codigoPuntoVenta"].ToString();

                    }
                    conn.Close();
                    return rta;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }


        /// <summary>
        /// Variable que retorna el metodo getLocales().
        /// </summary>
        public static BindableCollection<LocalModel> locales = new BindableCollection<LocalModel>();

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

                    string cadena = $"SELECT Distinct * FROM PuntoVenta WHERE (( CodigoPuntoVenta like '%{Caracteres}%' ) or ( Nombres like '%{Caracteres}%' )) AND Estado = 'Activo' ORDER BY codigoPuntoVenta ";
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
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Elimina de la base de datos el punto de venta del codigo dado.
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        public static object deleteLocal(string Codigo, string nombre)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena0 = $"SELECT [CedulaEmpleado], [Nombres], [Apellidos]  FROM Empleado where CodigoPuntoVenta = {Codigo} ";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd0.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            BindableCollection<EmpleadoModel> empleados = new BindableCollection<EmpleadoModel>();
                            while (reader.Read())
                            {
                                EmpleadoModel empleado = new EmpleadoModel();
                                empleado.cedula = reader["CedulaEmpleado"].ToString();
                                empleado.firstName = reader["Nombres"].ToString();
                                empleado.lastName = reader["Apellidos"].ToString();
                                empleados.Add(empleado);
                            }
                            conn.Close();
                            reader.Close();
                            return new object[] { "Local con empleados.", empleados };
                        }
                        else
                        {
                            reader.Close();
                            string cadena = $"Update PuntoVenta SET Estado = 'Inactivo' Where CodigoPuntoVenta = '{Codigo}' ";
                            SqlCommand cmd = new SqlCommand(cadena, conn);
                            //conn.Open();
                            cmd.ExecuteNonQuery();
                            conn.Close();
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("\n\t" + DateTime.Now + "-- ");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine($" {RetailHUB.usuarioConectado}: Ha eliminado al local {Codigo} - {nombre}. \n");
                            Registrar("Delete", "", Codigo, "", "deleteLocal", "codigo");
                            return new object[] { "Local eliminado" };
                        }

                    }



                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return e.Message;
            }

        }

        /// <summary>
        /// Actualiza la inforacion de un local en la base de datos.
        /// </summary>
        /// <param name="Local">Nuevo objeto de la clase LocalModel que se va a guardar en la base de datos. </param>
        /// <param name="IdAnterior">String del id del Local a actualizar.</param>
        /// <returns></returns>
        public static string ActualizarLocal(LocalModel Local, string IdAnterior)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {

                    string cadena = "UPDATE PuntoVenta SET  Nombres= @nombre, Direccion=@direccion, Telefono=@telefono, Ciudad=@ciudad, NumeroCanastillas=@nrocanastillas, FechaDeApertura=@fechaapertura WHERE codigoPuntoVenta=@codigoAntiguo";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigo", Local.codigo);
                    cmd.Parameters.AddWithValue("@nombre", Statics.PrimeraAMayuscula(Local.nombre));
                    cmd.Parameters.AddWithValue("@direccion", Statics.PrimeraAMayuscula(Local.direccion));
                    cmd.Parameters.AddWithValue("@telefono", Statics.PrimeraAMayuscula(Local.telefono));
                    cmd.Parameters.AddWithValue("@ciudad", Statics.PrimeraAMayuscula(Local.ciudad));
                    cmd.Parameters.AddWithValue("@nrocanastillas", Statics.PrimeraAMayuscula(Local.numeroDeCanastillas.ToString()));
                    cmd.Parameters.AddWithValue("@fechaapertura", Statics.PrimeraAMayuscula(Local.fechaDeApertura.Date.ToString("yyyy-MM-dd")));
                    cmd.Parameters.AddWithValue("@codigoAntiguo", IdAnterior);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    Console.WriteLine($"Se ha editado la informacion del local: {Local.nombre }");
                    conn.Close();
                    Registrar("Update", "ServidorGetLocales", Local.codigo, "LocalModel[]", "ActualizarLocal", "codigo");
                    return "El local se ha actualizado.";

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                if (e.Message.Substring(0, 52) == $"Violation of PRIMARY KEY constraint 'PK_PuntoVenta'.")
                {
                    return ($"El nombre {Local.nombre} ya esta registrado.");
                }
                else
                {
                    Console.WriteLine(e.Message);
                    return e.Message;
                }
            }
        }

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
                    string cadena = "SELECT Distinct [CodigoPuntoVenta], [Nombres]  FROM PuntoVenta ORDER BY Nombres";
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
                    //Console.WriteLine("Se consultaron los locales.");
                    return locales;
                }
            }
            catch (Exception)
            {
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
                    Registrar(Tipo:"Insert", NombreMetodoServidor:"servidorGetExistenciasConProductos", ClavePrimaria: existencia.codigo, TipoRetornoMetodoServidor: "ExistenciasModel[]", NombreMetodoCliente: "NuevaExistenciaBool", NombrePK: "codigo");
                    string response = InsertarExistenciaProducto(existencia);
                    if (response == "Y")
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write("\n\t" + DateTime.Now + "--");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($" {RetailHUB.usuarioConectado}: Ha registrado un nuevo documento: Existencias con fecha {existencia.fecha.ToString("dd'/'MM'/'yyyy")}.");
                        //Registrar("Insert", "ServidorGetLocales", NuevoLocal.codigo, "LocalModel[]", "NuevoLocal");
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
                        fecha = DateTime.MinValue;
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
                    string cadena = $"select distinct producto.codigoproducto, producto.Nombre, producto.unidadventa, ExistenciaProducto.Cantidad from  producto join existenciaproducto on producto.codigoproducto = existenciaproducto.codigoproducto where existenciaproducto.codigoexistencia = '{codigoExistencia}'; ";
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
                            producto.existencia = Int16.Parse( reader["cantidad"].ToString());


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
                    Registrar("Insert", "ServidorGetPedidoConProductos", pedido.codigo, "PedidoModel[]", "NuevoPedidoBool", "codigo");
                    string response = InsertarPedidoProducto(pedido);
                    if (response == "Y")
                    {
                        conn.Close();
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write("\n\t" + DateTime.Now + "--");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($" {RetailHUB.usuarioConectado}: Ha registrado un nuevo documento: Pedido con fecha {pedido.fecha.ToString("dd'/'MM'/'yyyy")}.");

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
                    Console.WriteLine(e.Message);
                    return e.Message;
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
                                existencia = Int32.Parse(reader["ExistenciaCantidad"].ToString()),
                                pedido = Int32.Parse(reader["cantidad"].ToString()),
                                //compraPorLocal = Int32.Parse(reader["CantidadEnviada"].ToString())

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
        public static string NuevaCompra(ComprasModel compra)
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
                    Registrar(Tipo: "Insert", NombreMetodoServidor: "ServidorGetComprasConProductos", ClavePrimaria: compra.codigo, TipoRetornoMetodoServidor: "ComprasModel[]", NombreMetodoCliente: "NuevaCompraBoolServidor", NombrePK: "codigo");
                    string response = InsertarRegistroCompraProducto(compra);
                    string rta = InsertarPedidosCompra(compra);
                    if (response == "Y" && rta == "Y")
                    {
                        conn.Close();
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write("\n\t" + DateTime.Now + "--");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($" {RetailHUB.usuarioConectado}: Ha registrado un nuevo documento: Compra con fecha {compra .fecha.ToString("dd'/'MM'/'yyyy")}.");

                        return "true";
                    }
                    /// Si algo fallo en la insercion de los productos y las cantidades se borra el registro del documento de compra para que pueda ser exitoso en proximos intentos.
                    string cadena0 = $"delete from Compras where CodigoCompra = '{compra.codigo}';";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.ExecuteNonQuery();

                    return "false";
                }
            }
            catch (Exception e)
            {
                if (e.Message.Length > 35 && e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    Console.WriteLine(e.Message);
                    return "false";
                }
                else
                {
                    Console.WriteLine(e.Message);
                    return  e.Message ;
                }
            }
        }

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
                    Console.WriteLine(e.Message);
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
                    Console.WriteLine(e.Message);
                    return "Server " + e.Message;
     
                }
            }
        }

        /// <summary>
        /// Actualiza la informacion del registro de compra del producto dado como parametro.
        /// </summary>
        /// <param name="producto"></param>
        /// <returns></returns>
        public static string UpdateRegistroCompra(ComprasModel compra)
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
                        Registrar(Tipo: "Update", NombreMetodoServidor: "ServidorgetRegistroCompra", ClavePrimaria: $"{compra.codigo}+{compra.productos[0].codigoProducto}", TipoRetornoMetodoServidor: "ProductoModel[]", NombreMetodoCliente: "UpdateRegistroCompraServidor", NombrePK: "codigoProducto");

                    return "true";

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return e.Message +"false";

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
                Console.WriteLine(e.Message);
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
                            Int32.TryParse(reader["peso"].ToString(), out int a);
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
                            Int32.TryParse(reader["peso"].ToString(), out int a);
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
                                sumaPedido = Int32.Parse(reader["pedido"].ToString())
                            };

                            if (Int32.TryParse(reader["CantidadComprada"].ToString(), out int a))
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
        public static BindableCollection<ProductoModel> getRegistroCompra(string codigoCompraCodigoProducto)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    BindableCollection<ProductoModel> productos = new BindableCollection<ProductoModel>();
                    string cadena = $"select * from RegistroCompra where CodigoCompra = '{codigoCompraCodigoProducto.Split("+")[0]}' and CodigoProducto = '{codigoCompraCodigoProducto.Split("+")[1]}'";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        productos.Clear();
                        while (reader.Read())
                        {
                            ProductoModel producto = new ProductoModel { codigoProducto = reader["CodigoCompra"].ToString()+ "+" + reader["CodigoProducto"].ToString() };
                            if (Int32.TryParse(reader["CantidadComprada"].ToString(), out int a)) { producto.compra = a; }
                            else { producto.compra = null; }
                            if (decimal.TryParse(reader["PrecioCompra"].ToString(), out decimal b)) { producto.precioCompra = b; }
                            else { producto.precioCompra = null; }
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

        #region Envios

        /// <summary>
        /// Regista en la base de datos la informacion relacionada con el nuevo documento de envio.
        /// </summary>
        /// <param name="envio"></param>
        /// <returns></returns>
        public static string NuevoEnvioBool(EnvioModel envio)
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
                    Registrar(Tipo: "Insert", NombreMetodoServidor: "ServidorgetEnvioConProductos", ClavePrimaria: envio.codigo, TipoRetornoMetodoServidor: "EnvioModel[]", NombreMetodoCliente: "NuevoEnvioBoolServidor", NombrePK: "codigo");
                    string response = InsertarEnvioProducto(envio);
                    if (response == "Y")
                    {
                        conn.Close();
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write("\n\t" + DateTime.Now + "--");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($" {RetailHUB.usuarioConectado}: Ha registrado un nuevo documento: Envio con fecha {envio.fechaEnvio.ToString("dd'/'MM'/'yyyy")}.");
                        return "true";
                    }
                    /// Si algo fallo en la insercion de los productos y las cantidades se borra el registro del documento de existencias para que pueda ser exitoso en proximos intentos.
                    string cadena0 = $"delete from Envio where CodigoEnvio = '{envio.codigo}';";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.ExecuteNonQuery();
                    return "false";
                }
            }
            catch (Exception e)
            {

                if (e.Message.Length > 35 && e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    //Console.WriteLine(e.Message);
                    return "false";
                }
                else
                {
                    Console.WriteLine(e.Message);
                    return "false";
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
                                pedido = Int32.Parse(reader["Cantidad"].ToString()),

                            };
                            Int32.TryParse(reader["EnvioCantidad"].ToString(), out int a);
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
        /// Actualiza la informacion de un envio.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string updateEnvio(EnvioModel envio)
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
                        Registrar(Tipo: "Update", NombreMetodoServidor: "ServidorgetEnvioConProductos", ClavePrimaria: envio.codigo, TipoRetornoMetodoServidor: "EnvioModel[]", NombreMetodoCliente: "updateEnvioServidor", NombrePK: "codigo");
                        Console.WriteLine("Registrando update");
                        return "true";
                    }
                    return "false";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                if (e.Message.Length > 35 && e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    return "false";
                }
                else
                {
                    return "false: "+ e.Message;
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
        public static string NuevoRecibidoBool(RecibidoModel recibido)
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
                    cmd.Parameters.AddWithValue("@peso", Statics.PrimeraAMayuscula(recibido.peso.ToString()));
                    cmd.Parameters.AddWithValue("@placa", Statics.PrimeraAMayuscula(recibido.placas.ToString()));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    string response = InsertarRecibidoProducto(recibido);
                    if (response == "Y")
                    {
                        updateEstadoEnvio(recibido.codigo);
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write("\n\t" + DateTime.Now + "--");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($" {RetailHUB.usuarioConectado}: Ha registrado un nuevo documento: Recibido con fecha {recibido.fechaRecibido.ToString("dd'/'MM'/'yyyy")}.");
                        conn.Close();
                        Registrar(Tipo: "Insert", NombreMetodoServidor: "ServidorgetRecibidoConProductos", ClavePrimaria: recibido.codigo, TipoRetornoMetodoServidor: "RecibidoModel[]", NombreMetodoCliente: "NuevoRecibidoBoolServidor", NombrePK: "codigo");
                        return "true";
                    }
                    /// Si algo fallo en la insercion de los productos y las cantidades se borra el registro del documento de existencias para que pueda ser exitoso en proximos intentos.
                    string cadena0 = $"delete from recibido where CodigoRecibido = '{recibido.codigo}';";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.ExecuteNonQuery();
                    return "false";
                }
            }
            catch (Exception e)
            {

                if (e.Message.Length > 35 && e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    //Console.WriteLine("Ya se ha registrado un recibido para este envio, edite ese documento");
                    return "false";
                }
                else
                {
                    return "false";
                }

            }
        }

        /// <summary>
        /// Inserta en la base de datos la cantidad recibidas de cada producto
        /// </summary>
        /// <param name="recibido"></param>
        /// <returns></returns>
        public static string InsertarRecibidoProducto(RecibidoModel recibido)
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
        /// Devuelve la instancia del recibido -incluidos los poductos con la cantidad recibida- cuyo codigo de pedido es dado como parametro.
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
                            Int32.TryParse(reader["Cantidad"].ToString(), out int b);
                            producto.compraPorLocal = b;
                            Int32.TryParse(reader["recibidoCantidad"].ToString(), out int a);
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
        public static string updateRecibido(RecibidoModel recibido)
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
                    string response = updateProductosRecibido(recibido);
                    if (response == "Y")
                    {
                        conn.Close();
                        Registrar(Tipo: "Update", NombreMetodoServidor: "ServidorgetRecibidoConProductos", ClavePrimaria: recibido.codigo, TipoRetornoMetodoServidor: "RecibidoModel[]", NombreMetodoCliente: "updateRecibidoServidor", NombrePK: "codigo");
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write("\n\t" + DateTime.Now + "--");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($" {RetailHUB.usuarioConectado}: Ha registrado un documento: Recibido con fecha {recibido.fechaRecibido.ToString("dd'/'MM'/'yyyy")}.");
                        return "true";
                    }
                    return "false";
                }
            }
            catch (Exception e)
            {

                if (e.Message.Length > 35 && e.Message.Substring(0, 24) == $"Violation of PRIMARY KEY")
                {
                    return "false";
                }
                else
                {
                    return "false";
                }

            }
        }

        /// <summary>
        /// Actualiza la informacion de la cantidad recibida de un producto.
        /// </summary>
        /// <param name="recibido"></param>
        /// <returns></returns>
        public static string updateProductosRecibido(RecibidoModel recibido)
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

        #region Clientes
        /// <summary>
        /// Metodo encargado de ejecutar el query insert del nuevo cliente en la base de datos local.
        /// </summary>
        /// <param name="Cliente">Instancia de la clase ClientesModel.</param>
        /// <returns></returns>
        public static string AddClient(ClientesModel Cliente)
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
                    return "true";
                }
            }
            catch (Exception e)
            {
                if (e.Message.Substring(0, 50) == $"Violation of PRIMARY KEY constraint 'PK_Clientes'.")
                {
                    return "Cliente ya existe";
                }
                else
                {
                    Console.WriteLine(e.Message);
                    return "false";
                }
            }
        }
        #endregion

        /// <summary>
        /// Crea los registros en la base de datos local de los cambios realizados para que los clientes puedan despues replicarlos.
        /// </summary>
        /// <param name="Tipo"></param>
        /// <param name="NombreMetodoServidor"></param>
        /// <param name="ClavePrimaria"></param>
        /// <param name="TipoRetornoMetodoServidor"></param>
        /// <param name="NombreMetodoCliente"></param>
        /// <returns></returns>
        public static bool Registrar(string Tipo, string NombreMetodoServidor,string ClavePrimaria, string TipoRetornoMetodoServidor, string NombreMetodoCliente, string NombrePK)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"INSERT INTO Registros(Tipo,NombreMetodoServidor,ClavePrimaria,TipoRetornoMetodoServidor,NombreMetodoCliente, NombrePK) VALUES ('{Tipo}','{NombreMetodoServidor}','{ClavePrimaria}','{TipoRetornoMetodoServidor}','{NombreMetodoCliente}', '{NombrePK}');";
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
        /// Retorna los registros que han cambiado de acuerdo a la informacion en cada cliente pertinenete el ultimo registro actualizado.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static BindableCollection<string[]> registroCambios(int a)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"SELECT * FROM Registros WHERE ID > {a} ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    BindableCollection<string[]> resultado = new BindableCollection<string[]>();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string[] reg = new string[7]
                            {
                                reader["Id"].ToString(),
                                reader["Tipo"].ToString(),
                                reader["NombreMetodoServidor"].ToString(),
                                reader["ClavePrimaria"].ToString(),
                                reader["TipoRetornoMetodoServidor"].ToString(),
                                reader["NombreMetodoCliente"].ToString(),
                                reader["NombrePK"].ToString()

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
