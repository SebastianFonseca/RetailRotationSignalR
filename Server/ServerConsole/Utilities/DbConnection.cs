using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using ServerConsole.Models;
using Caliburn.Micro;

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
        public static String[] Login(string User, string Password)
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
                                return new[] {"Registrado", reader["Cargo"].ToString() };
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
                    cmd.Parameters.AddWithValue("@name", Statics.PrimeraAMayuscula(Cliente.FirstName));
                    cmd.Parameters.AddWithValue("@lastname", Statics.PrimeraAMayuscula(Cliente.LastName));
                    cmd.Parameters.AddWithValue("@cedula", Cliente.Cedula);
                    cmd.Parameters.AddWithValue("@correo", string.IsNullOrEmpty(Cliente.Correo) ? (object)DBNull.Value : Cliente.Correo);
                    cmd.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(Cliente.Telefono) ? (object)DBNull.Value : Cliente.Telefono);
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
                            cmd2.Parameters.AddWithValue("@fv", Producto.FechaVencimiento == DateTime.Today ? (object)DBNull.Value : Producto.FechaVencimiento) ;
                            cmd2.Parameters.AddWithValue("@iva", Producto.IVA);
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
                    cmd.Parameters.AddWithValue("@cedula", Empleado.Cedula);
                    cmd.Parameters.AddWithValue("@puntodeventa", Empleado.PuntoDeVenta.codigo);
                    cmd.Parameters.AddWithValue("@nombre", Statics.PrimeraAMayuscula(Empleado.FirstName));
                    cmd.Parameters.AddWithValue("@apellidos", Statics.PrimeraAMayuscula(Empleado.LastName));
                    cmd.Parameters.AddWithValue("@fecha", Empleado.FechaDeContratacion.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@salario", Empleado.Salario);
                    cmd.Parameters.AddWithValue("@telefono", Empleado.Telefono);
                    cmd.Parameters.AddWithValue("@direccion", Empleado.Direccion);
                    cmd.Parameters.AddWithValue("@cargo", Empleado.Cargo.Substring(37));
                    cmd.Parameters.AddWithValue("@contraseña", Statics.Hash(Empleado.Password));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write("\n\t" + DateTime.Now + "--");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($" {RetailHUB.usuarioConectado}: Ha registrado un nuevo usuario {Empleado.FirstName} {Empleado.LastName}");
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

                    string cadena = $"SELECT * FROM EMPLEADO WHERE ( CedulaEmpleado like '%{Caracteres}%' ) or ( Nombres like '%{Caracteres}%' ) or ( Apellidos like '%{Caracteres}%' )  ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        emp.Clear();
                        while (reader.Read())
                        {
                            EmpleadoModel persona = new EmpleadoModel();
                            persona.Cedula = reader["CedulaEmpleado"].ToString();
                            persona.PuntoDeVenta.codigo = reader["CodigoPuntoVenta"].ToString();
                            persona.FirstName = reader["Nombres"].ToString();
                            persona.LastName = reader["Apellidos"].ToString();
                            persona.FechaDeContratacion = DateTime.Parse(reader["FechaContratacion"].ToString());
                            persona.Salario = Decimal.Parse(reader["Salario"].ToString());
                            persona.Telefono = reader["Telefono"].ToString();
                            persona.Cargo = reader["Cargo"].ToString();
                            persona.Direccion = reader["Direccion"].ToString();
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
                    Console.WriteLine($"Se ha registrado el nuevo local: {NuevoLocal.nombre }");
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

                    string cadena = $"SELECT * FROM PuntoVenta WHERE ( CodigoPuntoVenta like '%{Caracteres}%' ) or ( Nombres like '%{Caracteres}%' ) ";
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
        public static string deleteLocal(string Codigo)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {

                    string cadena = $"delete from PuntoVenta Where CodigoPuntoVenta = '{Codigo}' ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return "Local eliminado";
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
                    Console.WriteLine("Se consultaron los locales.");
                    return locales;
                }
            }
            catch (Exception e)
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
