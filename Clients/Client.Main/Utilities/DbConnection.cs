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
        ///         
        public static string[] Login(string User, string Password)
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
                                return new[] { "Registrado", reader["Cargo"].ToString() };
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

        /// <summary>
        /// Metodo encargado de ejecutar el query insert del nuevo cliente en la base de datos local.
        /// </summary>
        /// <param name="Cliente">Instancia de la clase cliente.</param>
        /// <returns></returns>        
        public static bool AddClient(ClientesModel Cliente)
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
                    return true;

                }
            }
            catch (Exception e)
            {
                if (e.Message.Substring(0, 50) == $"Violation of PRIMARY KEY constraint 'PK_Clientes'.")
                {
                    MessageBox.Show($"La cedula {Cliente.Cedula} ya esta registrada.");
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
                            cliente.FirstName = reader["Nombres"].ToString();
                            cliente.LastName = reader["Apellidos"].ToString();
                            cliente.Cedula = reader["CedulaCliente"].ToString();
                            cliente.Correo = reader["Email"].ToString();
                            cliente.Telefono = reader["Telefono"].ToString();
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
                    cmd.Parameters.AddWithValue("@name", Statics.PrimeraAMayuscula(Cliente.FirstName));
                    cmd.Parameters.AddWithValue("@lastname", Statics.PrimeraAMayuscula(Cliente.LastName));
                    cmd.Parameters.AddWithValue("@cedula", Cliente.Cedula);
                    cmd.Parameters.AddWithValue("@correo", string.IsNullOrEmpty(Cliente.Correo) ? (object)DBNull.Value : Cliente.Correo);
                    cmd.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(Cliente.Telefono) ? (object)DBNull.Value : Cliente.Telefono);
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
                    MessageBox.Show($"La cedula {Cliente.Cedula} ya esta registrada.");
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

                    string cadena = "INSERT INTO Empleado(CedulaEmpleado,CodigoPuntoVenta,Nombres,Apellidos,FechaContratacion,Salario,Telefono,Cargo,Password,Direccion) VALUES " +
                                                        "(@cedula,@puntodeventa,@nombre,@apellidos,@fecha,@salario,@telefono,@cargo,@contraseña,@direccion);";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@cedula", Empleado.Cedula);
                    cmd.Parameters.AddWithValue("@puntodeventa", Empleado.PuntoDeVenta.Codigo);
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
                    MessageBox.Show($"Se ha registrado al nuevo usuario {Empleado.FirstName} {Empleado.LastName}");
                    conn.Close();
                    return true;

                }
            }
            catch (Exception e)
            {

                if ( e.Message.Substring(0,50) == $"Violation of PRIMARY KEY constraint 'PK_Empleado'.")
                {
                    MessageBox.Show($"La cedula {Empleado.Cedula} ya esta registrada.");
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
        /// Actualiza el empleado del numero de cedila dado.
        /// </summary>
        /// <param name="Empleado"></param>
        /// <param name="CC"></param>
        /// <returns></returns>
        public static bool ActualizarUsuario(EmpleadoModel Empleado, string CC)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {


                    string cadena = "UPDATE Empleado SET CedulaEmpleado = @NuevoCedula, CodigoPuntoVenta = @puntodeventa, Nombres = @nombre, Apellidos = @apellidos, FechaContratacion = @fecha, Salario =@salario, Telefono = @telefono, Cargo = @cargo," +
                                    "Password = @contraseña, Direccion = @direccion WHERE CedulaEmpleado = @AntiguoCedula ";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@NuevoCedula", Empleado.Cedula);
                    cmd.Parameters.AddWithValue("@puntodeventa", Empleado.PuntoDeVenta.Codigo);
                    cmd.Parameters.AddWithValue("@nombre", Statics.PrimeraAMayuscula(Empleado.FirstName));
                    cmd.Parameters.AddWithValue("@apellidos", Statics.PrimeraAMayuscula(Empleado.LastName));
                    cmd.Parameters.AddWithValue("@fecha", Empleado.FechaDeContratacion.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@salario", Empleado.Salario);
                    cmd.Parameters.AddWithValue("@telefono", Empleado.Telefono);
                    cmd.Parameters.AddWithValue("@direccion", Empleado.Direccion);
                    cmd.Parameters.AddWithValue("@cargo", Empleado.Cargo.Substring(37));
                    cmd.Parameters.AddWithValue("@contraseña", Statics.Hash(Empleado.Password));
                    cmd.Parameters.AddWithValue("@AntiguoCedula", CC);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show($"Se ha Actualizado el usuario {Empleado.FirstName} {Empleado.LastName}");
                    conn.Close();
                    return true;

                }
            }
            catch (Exception e)
            {

                if (e.Message.Substring(0, 50) == $"Violation of PRIMARY KEY constraint 'PK_Empleado'.")
                {
                    MessageBox.Show($"La cedula {Empleado.Cedula} ya esta registrada.");
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
                            persona.PuntoDeVenta.Codigo = reader["CodigoPuntoVenta"].ToString();
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
                MessageBox.Show(e.Message);
                return null;
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

                    string cadena = $"delete from empleado Where CedulaEmpleado = '{Cedula}' ";
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

        public static BindableCollection<EmpleadoModel> empleados = new BindableCollection<EmpleadoModel>();
        /// <summary>
        /// Metodo que obtiene los empelados con cargo de "Administrador de sede"
        /// </summary>
        /// <returns></returns>
        public static BindableCollection<EmpleadoModel> getAdministradores(){
            EmpleadoModel empleado = new EmpleadoModel();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {                    
                    string cadena = "SELECT [CedulaEmpleado], [Nombres], [Apellidos]  FROM EMPLEADO where [Cargo]=@Admin";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@Admin", " Administrador de sede"); 
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            empleado.Cedula = reader["CedulaEmpleado"].ToString();
                            empleado.FirstName = reader["Nombres"].ToString();
                            empleado.LastName = reader["Apellidos"].ToString();
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

                    string cadena = "INSERT INTO PuntoVenta(codigoPuntoVenta,Nombres,Direccion,Telefono,Ciudad,NumeroCanastillas,FechaDeApertura) " +
                                                   "VALUES (@codigo,@nombre,@direccion,@telefono,@ciudad,@nrocanastillas,@fechaapertura)";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigo", NuevoIdLocal());
                    cmd.Parameters.AddWithValue("@nombre", Statics.PrimeraAMayuscula(NuevoLocal.Nombre));
                    cmd.Parameters.AddWithValue("@direccion", Statics.PrimeraAMayuscula(NuevoLocal.Direccion));
                    cmd.Parameters.AddWithValue("@telefono", Statics.PrimeraAMayuscula(NuevoLocal.Telefono));
                    cmd.Parameters.AddWithValue("@ciudad", Statics.PrimeraAMayuscula(NuevoLocal.Ciudad));
                    cmd.Parameters.AddWithValue("@nrocanastillas", Statics.PrimeraAMayuscula(NuevoLocal.NumeroDeCanastillas.ToString()));
                    cmd.Parameters.AddWithValue("@fechaapertura", Statics.PrimeraAMayuscula(NuevoLocal.FechaDeApertura.Date.ToString("yyyy-MM-dd")));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show($"Se ha registrado al nuevo local: {NuevoLocal.Nombre }");
                    conn.Close();
                    return true;

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                if (e.Message.Substring(0, 52) == $"Violation of PRIMARY KEY constraint 'PK_PuntoVenta'.")
                {
                    MessageBox.Show($"El nombre {NuevoLocal.Nombre} ya esta registrado.");
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
                            local.Codigo = reader["CodigoPuntoVenta"].ToString();
                            local.Nombre = reader["Nombres"].ToString();
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
        /// Obtiene el siguiente ID para una nueva instancia de NuevoLocal
        /// </summary>
        /// <returns></returns>
        public static string NuevoIdLocal()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = "select MAX(CodigoPuntoVenta) from PuntoVenta";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        if(reader[0] != null) 
                        { 
                        int rta = Convert.ToInt32(reader[0].ToString()) + 1;
                        conn.Close();
                        return rta.ToString();
                        }
                        else { return "1"; }
                    }


                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + " exception");
                return "0";
            }
        }
        /// <summary>
        /// Sincronizar una tabla local con una del servidor.
        /// </summary>
        public static async void SincronizarReplicacionMerge()
        {
            //try
            //{
            //    using (SqlConnection conn = new SqlConnection(_connString))
            //    {

            //        string cadena = "EXEC msdb.dbo.sp_start_job N'DESKTOP-RB7GS8A-RetailRotationClientD-MergeClientesFromClie-DESKTOP-RB7GS8A-3'";
            //        SqlCommand cmd = new SqlCommand(cadena, conn);
            //        conn.Open();
            //        cmd.ExecuteNonQuery();
            //        conn.Close();


            //    }
            //}
            //catch (Exception e)
            //{

            //        MessageBox.Show(e.Message);


            //}





            //string subscriberName = "DESKTOP-RB7GS8A"; /*\\MSSQLSERVER*/
            //string publisherName = "DESKTOP-RB7GS8A";
            //string publicationName = "MergeClientesFromClients";
            //string subscriptionDbName = "RetailRotationClientDataBase";
            //string publicationDbName = "RetailRotationServerDataBase";



            //try
            //{
            //    using (SqlConnection conn = new System.Data.SqlClient.SqlConnection(_connString))
            //    {
            //        // Create a connection to the Publisher.
            //        ////conn.Open();
            //     ServerConnection serverconn = new Microsoft.SqlServer.Management.Common.ServerConnection(publisherName, userName:"sebas",password:"");
            //    //ServerConnection serverconn = new ServerConnection(conn);


            //    MergeSubscription subscription;

            //       // conn.Open();
            //    // Connect to the Publisher
            //    serverconn.Connect();
            //    MessageBox.Show(serverconn.Login.ToString()); 

            //    // Define the subscription.
            //    subscription = new MergeSubscription();
            //    subscription.ConnectionContext = serverconn;
            //    subscription.DatabaseName = publicationDbName;
            //    subscription.PublicationName = publicationName;
            //    subscription.SubscriptionDBName = subscriptionDbName;
            //    subscription.SubscriberName = subscriberName;
            //        subscription.SynchronizationAgent.Synchronize();

            //        // If the push subscription exists, start the synchronization.
            //        if (subscription.LoadProperties())
            //    {
            //        // Check that we have enough metadata to start the agent.
            //        if (subscription.SubscriberSecurity != null)
            //        {
            //            // Synchronously start the Merge Agent for the subscription.
            //            subscription.SynchronizationAgent.Synchronize();
            //        }
            //        else
            //        {
            //            throw new ApplicationException("There is insufficent metadata to " +
            //                "synchronize the subscription. Recreate the subscription with " +
            //                "the agent job or supply the required agent properties at run time.");
            //        }
            //    }
            //    else
            //    {
            //        // Do something here if the push subscription does not exist.
            //        throw new ApplicationException(String.Format(
            //            "The subscription to '{0}' does not exist on {1}",
            //            publicationName, subscriberName));
            //    }
            //    }

            //}
            //catch (Exception ex)
            //{
            //    // Implement appropriate error handling here.
            //    MessageBox.Show(ex.Message);
            //   // throw new ApplicationException("The subscription could not be synchronized.", ex);
            //}
            ////finally
            ////{
            ////    //conn.Disconnect();
            ////    //}

            ////}
            ///



            // // Define the server, publication, and database names.
            // string subscriberName = "DESKTOP-RB7GS8A";
            //// string publisherName = "DESKTOP-RB7GS8A";
            // string publicationName = "publi";
            // string subscriptionDbName = "RetailRotationClientDataBase";
            // string publicationDbName = "RetailRotationServerDataBase";

            // // Create a connection to the Publisher.
            // SqlConnection sql = new SqlConnection(@"Server= DESKTOP-RB7GS8A; Database=RetailRotationClientDataBase; Trusted_Connection= True;Persist Security Info=True;");
            // ServerConnection conn = new ServerConnection(sqlConnection:sql);



            // MergeSubscription subscription;

            // try
            // {
            //     // Connect to the Publisher
            //     conn.Connect();
            //     MessageBox.Show("db name" + conn.DatabaseName+" currentdb" + conn.CurrentDatabase +" con is open "+ conn.IsOpen.ToString() + " Acces token" + conn.AccessToken );
            //     // Define the subscription.
            //     subscription = new MergeSubscription();
            //     subscription.ConnectionContext = conn;
            //     subscription.DatabaseName = publicationDbName;
            //     subscription.PublicationName = publicationName;
            //     subscription.SubscriptionDBName = subscriptionDbName;
            //     subscription.SubscriberName = subscriberName;

            //     // If the push subscription exists, start the synchronization.
            //     if (subscription.LoadProperties())
            //     {
            //         // Check that we have enough metadata to start the agent.
            //         if (subscription.SubscriberSecurity != null)
            //         {
            //             // Synchronously start the Merge Agent for the subscription.
            //             subscription.SynchronizationAgent.Synchronize();
            //         }
            //         else
            //         {
            //             throw new ApplicationException("There is insufficent metadata to " +
            //                 "synchronize the subscription. Recreate the subscription with " +
            //                 "the agent job or supply the required agent properties at run time.");
            //         }
            //     }
            //     else
            //     {
            //         // Do something here if the push subscription does not exist.
            //         throw new ApplicationException(String.Format(
            //             "The subscription to '{0}' does not exist on {1}",
            //             publicationName, subscriberName));
            //     }
            // }
            // catch (Exception ex)
            // {
            //     // Implement appropriate error handling here.
            //     ///throw new ApplicationException("The subscription could not be synchronized.", e
            //     MessageBox.Show(ex.InnerException.ToString());
            // }
            // finally
            // {
            //     conn.Disconnect();
            // }

            //try
            //{

            //    using (IDbConnection conn = new SqlConnection("Server= DESKTOP-RB7GS8A; Database= msdb; Trusted_Connection= True;"))
            //    {
            //        conn.Open();
            //        string sql = "sp_start_job";
            //        var parameters = new DynamicParameters();
            //        parameters.Add("@job_name", "DESKTOP-RB7GS8A-RetailRotationClientD-MergeClientesFromClie-DESKTOP-RB7GS8A-3");
            //        parameters.Add(name: "@RetVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
            //         Task result =  conn.ExecuteAsync(sql, commandType: CommandType.StoredProcedure, param : parameters);
            //        result.Wait();
            //        MessageBox.Show( parameters.Get<Int32>("@RetVal").ToString());

            //    }

            //}
            //catch (Exception e)
            //{
            // MessageBox.Show(e.Message);

            //}

            var serverProvider = new SqlSyncProvider("Server = DESKTOP-RB7GS8A; Database = RetailRotationClientDataBase; Trusted_Connection = True; Persist Security Info = True;");
            var clientProvider = new SqlSyncProvider("Server = DESKTOP-RB7GS8A; Database = RetailRotationServerDataBase; Trusted_Connection = True; Persist Security Info = True;");

            var agent = new SyncAgent(clientProvider, serverProvider, new string[]{ "Empleado"});

            var progress = new SynchronousProgress<ProgressArgs>(s =>MessageBox.Show($"{s.Context.SyncStage}:\t{s.Message}"));  
            
            try
            {
                do
                {
                    var syncContext = await agent.SynchronizeAsync(progress);
                    MessageBox.Show(syncContext.ToString());
                } while (agent.SessionState == Dotmim.Sync.Enumerations.SyncSessionState.Synchronizing);
                MessageBox.Show("Hola");
            }
            catch (Exception e)
            {
                MessageBox.Show (e.Message);
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

    
