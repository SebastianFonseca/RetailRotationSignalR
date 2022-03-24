using Caliburn.Micro;
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
                    cmd0.Parameters.AddWithValue("@codigo", Producto.CodigoProducto);
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
                    cmd1.Parameters.AddWithValue("@codigob", Producto.CodigoBarras);
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
                    cmd.Parameters.AddWithValue("@nombre", Producto.Nombre);
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
                            string cadena2 = "INSERT INTO Producto(CodigoProducto, Nombre, UnidadVenta,	UnidadCompra, PrecioVenta, Seccion, FechaVencimiento, IVA, CodigoBarras)" +
                                " VALUES (@codigo,@nombre,@univenta,@unicompra,@precio,@seccion,@fv,@iva,@cb)";
                            SqlCommand cmd2 = new SqlCommand(cadena2, conn);
                            cmd2.Parameters.AddWithValue("@codigo", Statics.PrimeraAMayuscula(Producto.CodigoProducto));
                            cmd2.Parameters.AddWithValue("@nombre", Statics.PrimeraAMayuscula(Producto.Nombre));
                            cmd2.Parameters.AddWithValue("@univenta", Statics.PrimeraAMayuscula(Producto.UnidadVenta));
                            cmd2.Parameters.AddWithValue("@unicompra", Statics.PrimeraAMayuscula(Producto.UnidadCompra));
                            cmd2.Parameters.AddWithValue("@precio", Producto.PrecioVenta.ToString());
                            cmd2.Parameters.AddWithValue("@seccion", Statics.PrimeraAMayuscula(Producto.Seccion));
                            cmd2.Parameters.AddWithValue("@fv", Producto.FechaVencimiento == DateTime.Today ? (object)DBNull.Value : Producto.FechaVencimiento);
                            cmd2.Parameters.AddWithValue("@iva", Producto.iva);
                            cmd2.Parameters.AddWithValue("@cb", string.IsNullOrEmpty(Producto.CodigoBarras) ? (object)DBNull.Value : Producto.CodigoBarras);
                            cmd2.ExecuteNonQuery();
                            conn.Close();
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("\n\t" + DateTime.Now + ": ");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine($"El usuario {RetailHUB.usuarioConectado} registro un nuevo producto: {Producto.CodigoProducto} - {Producto.Nombre}.");
                            return $"El usuario {RetailHUB.usuarioConectado} registro un nuevo producto: {Producto.CodigoProducto}- {Producto.Nombre}.";
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
                    string cadena = "SELECT *  FROM Producto ORDER BY Nombre";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ProductoModel producto = new ProductoModel();
                            producto.CodigoProducto = reader["CodigoProducto"].ToString();
                            producto.Nombre = reader["Nombre"].ToString();
                            producto.UnidadCompra = reader["UnidadCompra"].ToString();
                            producto.UnidadCompra = reader["UnidadVenta"].ToString();
                            producto.PrecioVenta = Convert.ToDecimal(reader["PrecioVenta"].ToString());
                            producto.Seccion = reader["Seccion"].ToString();
                            producto.iva = Convert.ToDecimal(reader["IVA"].ToString());
                            producto.CodigoBarras = reader["CodigoBarras"].ToString();
                            if (reader["FechaVencimiento"].ToString() == "")
                            {
                                producto.FechaVencimiento = DateTime.MinValue;
                            }
                            else
                            {
                                producto.FechaVencimiento = DateTime.Parse(reader["FechaVencimiento"].ToString());
                            }
                            productos.Add(producto);
                        }
                    }
                    conn.Close();
                    //Console.WriteLine("Se consultaron los locales.");
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
                    string cadena = $"SELECT Distinct * FROM Producto WHERE (CodigoProducto LIKE '%{caracteres}%') OR (Nombre LIKE '%{caracteres}%') ORDER BY Nombre;";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ProductoModel producto = new ProductoModel();
                            producto.CodigoProducto = reader["CodigoProducto"].ToString();
                            producto.Nombre = reader["Nombre"].ToString();
                            producto.UnidadVenta = reader["UnidadVenta"].ToString();
                            producto.UnidadCompra = reader["UnidadCompra"].ToString();
                            producto.PrecioVenta = Convert.ToDecimal(reader["PrecioVenta"].ToString());
                            producto.Seccion = reader["Seccion"].ToString();
                            producto.iva = Convert.ToDecimal(reader["IVA"].ToString());
                            producto.CodigoBarras = reader["CodigoBarras"].ToString();
                            if (reader["FechaVencimiento"].ToString() == "")
                            {
                                producto.FechaVencimiento = DateTime.MinValue;
                            }
                            else
                            {
                                producto.FechaVencimiento = DateTime.Parse(reader["FechaVencimiento"].ToString());
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
                    string cadena = $"delete from Producto Where CodigoProducto = '{idProducto}' ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write("\n\t" + DateTime.Now + "--");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($" {RetailHUB.usuarioConectado}: Ha eliminado al producto con codigo: {idProducto}");
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
                    string cadena1 = $"select * from Producto where CodigoBarras = @codigob except select * from Producto where CodigoProducto = '{Producto.CodigoProducto}'";
                    SqlCommand cmd1 = new SqlCommand(cadena1, conn);
                    cmd1.Parameters.AddWithValue("@codigob", Producto.CodigoBarras);
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
                    string cadena2 = $"select * from Producto where Nombre = @nombre except select * from Producto where CodigoProducto = '{Producto.CodigoProducto}'";
                    SqlCommand cmd2 = new SqlCommand(cadena2, conn);
                    cmd2.Parameters.AddWithValue("@nombre", Producto.Nombre);
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
                            string cadena = $"UPDATE Producto SET  Nombre=@nombre, UnidadVenta=@univenta,	UnidadCompra=@unicompra, PrecioVenta=@precio, Seccion=@seccion, FechaVencimiento=@fv, IVA=@iva, CodigoBarras=@cb WHERE CodigoProducto = '{Producto.CodigoProducto}' ";
                            SqlCommand cmd = new SqlCommand(cadena, conn);
                            cmd.Parameters.AddWithValue("@codigo", Statics.PrimeraAMayuscula(Producto.CodigoProducto));
                            cmd.Parameters.AddWithValue("@nombre", Statics.PrimeraAMayuscula(Producto.Nombre));
                            cmd.Parameters.AddWithValue("@univenta", Statics.PrimeraAMayuscula(Producto.UnidadVenta));
                            cmd.Parameters.AddWithValue("@unicompra", Statics.PrimeraAMayuscula(Producto.UnidadCompra));
                            cmd.Parameters.AddWithValue("@precio", Producto.PrecioVenta.ToString());
                            cmd.Parameters.AddWithValue("@seccion", Statics.PrimeraAMayuscula(Producto.Seccion));
                            cmd.Parameters.AddWithValue("@fv", Producto.FechaVencimiento == DateTime.Today ? (object)DBNull.Value : Producto.FechaVencimiento);
                            cmd.Parameters.AddWithValue("@iva", Producto.iva);
                            cmd.Parameters.AddWithValue("@cb", string.IsNullOrEmpty(Producto.CodigoBarras) ? (object)DBNull.Value : Producto.CodigoBarras);
                            cmd.ExecuteNonQuery();
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("\n\t" + DateTime.Now + "--");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine($"{RetailHUB.usuarioConectado}: Ha Actualizado al producto {Producto.CodigoProducto} - {Producto.Nombre}");
                            conn.Close();
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
                    string cadena = "INSERT INTO Proveedor(CedulaProveedor,Nombres,Apellidos,Telefono,Ciudad,Direccion) VALUES " +
                                                        "(@cedula,@nombre,@apellidos,@telefono,@ciudad,@direccion);";
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
                        Console.WriteLine($" {RetailHUB.usuarioConectado}: Ha registrado un nuevo proveedor {proveedor.firstName} {proveedor.lastName}");
                        conn.Close();
                        return "Se ha registrado al nuevo proveedor.";
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

                    string cadena0 = $"delete from ProveedorProducto where CedulaProveedor = '{idProveedor}';";
                    SqlCommand cmd0 = new SqlCommand(cadena0, conn);
                    cmd0.ExecuteNonQuery();
                    foreach (ProductoModel producto in productos)
                    {
                        string cadena = "INSERT INTO ProveedorProducto(CedulaProveedor,CodigoProducto) VALUES (@prov,@prod);";
                        SqlCommand cmd = new SqlCommand(cadena, conn);
                        cmd.Parameters.AddWithValue("@prov", idProveedor);
                        cmd.Parameters.AddWithValue("@prod", producto.CodigoProducto);
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
                    string cadena = $"SELECT Distinct * FROM Proveedor WHERE ( CedulaProveedor like '%{Caracteres}%' ) or ( Nombres like '%{Caracteres}%' ) or ( Apellidos like '%{Caracteres}%' ) ORDER BY Nombres ";
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
                    string cadena = $"SELECT Distinct * FROM Proveedor WHERE CedulaProveedor = '{Cedula}' ORDER BY Nombres";
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
                            producto.CodigoProducto = reader0["CodigoProducto"].ToString();
                            producto.Nombre = reader0["Nombre"].ToString();
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
        public static string deleteProveedor(string Cedula)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = $"delete from Proveedor Where CedulaProveedor = '{Cedula}' ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write("\n\t" + DateTime.Now + "--");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($" {RetailHUB.usuarioConectado}: Ha eliminado al proveedor {Cedula}");
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
                    string cadena = "INSERT INTO Empleado(CedulaEmpleado,CodigoPuntoVenta,Nombres,Apellidos,FechaContratacion,Salario,Telefono,Cargo,Password,Direccion) VALUES " +
                                                        "(@cedula,@puntodeventa,@nombre,@apellidos,@fecha,@salario,@telefono,@cargo,@contraseña,@direccion);";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@cedula", Empleado.cedula);
                    cmd.Parameters.AddWithValue("@puntodeventa", Empleado.puntoDeVenta.codigo);
                    cmd.Parameters.AddWithValue("@nombre", Statics.PrimeraAMayuscula(Empleado.firstName));
                    cmd.Parameters.AddWithValue("@apellidos", Statics.PrimeraAMayuscula(Empleado.lastName));
                    cmd.Parameters.AddWithValue("@fecha", Empleado.fechaDeContratacion.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@salario", Empleado.salario);
                    cmd.Parameters.AddWithValue("@telefono", Empleado.telefono);
                    cmd.Parameters.AddWithValue("@direccion", Empleado.direccion);
                    cmd.Parameters.AddWithValue("@cargo", Empleado.cargo.Substring(37));
                    cmd.Parameters.AddWithValue("@contraseña", Statics.Hash(Empleado.password));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write("\n\t" + DateTime.Now + "--");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($" {RetailHUB.usuarioConectado}: Ha registrado un nuevo usuario {Empleado.firstName} {Empleado.lastName}");
                    conn.Close();
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
                    string cadena = $"SELECT Distinct * FROM EMPLEADO WHERE ( CedulaEmpleado like '%{Caracteres}%' ) or ( Nombres like '%{Caracteres}%' ) or ( Apellidos like '%{Caracteres}%' ) ORDER BY Nombres;";
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
                    string cadena = $"delete from empleado Where CedulaEmpleado = '{Cedula}' ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write("\n\t" + DateTime.Now + "--");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($" {RetailHUB.usuarioConectado}: Ha eliminado al usuario {Cedula}");
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


                    //string cadena = "INSERT INTO PuntoVenta(codigoPuntoVenta,Nombres,Direccion,Telefono,Ciudad,NumeroCanastillas,FechaDeApertura) " +
                    //"VALUES (@codigo,@nombre,@direccion,@telefono,@ciudad,@nrocanastillas,@fechaapertura)";

                    string cadena = "INSERT INTO PuntoVenta(Nombres,Direccion,Telefono,Ciudad,NumeroCanastillas,FechaDeApertura) " +
                                    "VALUES (@nombre,@direccion,@telefono,@ciudad,@nrocanastillas,@fechaapertura)";
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

                    string cadena = $"SELECT Distinct * FROM PuntoVenta WHERE ( CodigoPuntoVenta like '%{Caracteres}%' ) or ( Nombres like '%{Caracteres}%' ) ORDER BY Nombres ";
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
                            string cadena = $"delete from PuntoVenta Where CodigoPuntoVenta = '{Codigo}' ";
                            SqlCommand cmd = new SqlCommand(cadena, conn);
                            //conn.Open();
                            cmd.ExecuteNonQuery();
                            conn.Close();
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("\n\t" + DateTime.Now + "-- ");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine($" {RetailHUB.usuarioConectado}: Ha eliminado al local {Codigo} - {nombre}. \n");
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
