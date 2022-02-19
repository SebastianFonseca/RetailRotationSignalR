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

        #region Empleados

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
                    MessageBox.Show($"Se ha registrado al nuevo usuario {Empleado.firstName} {Empleado.lastName}");
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
        public static bool ActualizarUsuario(EmpleadoModel Empleado, string CC)
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
                    MessageBox.Show($"Se ha Actualizado el usuario {Empleado.firstName} {Empleado.lastName}");
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
                    string cadena = "SELECT [CedulaEmpleado], [Nombres], [Apellidos]  FROM EMPLEADO where [Cargo]=@Admin";
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

                    string cadena = "INSERT INTO PuntoVenta(codigoPuntoVenta,Nombres,Direccion,Telefono,Ciudad,NumeroCanastillas,FechaDeApertura) " +
                                                   "VALUES (@codigo,@nombre,@direccion,@telefono,@ciudad,@nrocanastillas,@fechaapertura)";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    ///cmd.Parameters.AddWithValue("@codigo", NuevoIdLocal());
                    cmd.Parameters.AddWithValue("@nombre", Statics.PrimeraAMayuscula(NuevoLocal.nombre));
                    cmd.Parameters.AddWithValue("@direccion", Statics.PrimeraAMayuscula(NuevoLocal.direccion));
                    cmd.Parameters.AddWithValue("@telefono", Statics.PrimeraAMayuscula(NuevoLocal.telefono));
                    cmd.Parameters.AddWithValue("@ciudad", Statics.PrimeraAMayuscula(NuevoLocal.ciudad));
                    cmd.Parameters.AddWithValue("@nrocanastillas", Statics.PrimeraAMayuscula(NuevoLocal.numeroDeCanastillas.ToString()));
                    cmd.Parameters.AddWithValue("@fechaapertura", Statics.PrimeraAMayuscula(NuevoLocal.fechaDeApertura.Date.ToString("yyyy-MM-dd")));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show($"Se ha registrado al nuevo local: {NuevoLocal.nombre }");
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

                    string cadena = $"SELECT * FROM PuntoVenta WHERE ( codigoPuntoVenta like '%{Caracteres}%' ) or ( Nombres like '%{Caracteres}%' ) ";
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
        /// Actualiza la inforacion de un local en la base de datos.
        /// </summary>
        /// <param name="Local">Nuevo objeto de la clase LocalModel que se va a guardar en la base de datos. </param>
        /// <param name="IdAnterior">String del id del Local a actualizar.</param>
        /// <returns></returns>
        public static bool ActualizarLocal(LocalModel Local, string IdAnterior) {

            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {

                    string cadena = "UPDATE PuntoVenta SET codigoPuntoVenta = @codigo, Nombres= @nombre, Direccion=@direccion, Telefono=@telefono, Ciudad=@ciudad, NumeroCanastillas=@nrocanastillas, FechaDeApertura=@fechaapertura WHERE codigoPuntoVenta=@codigoAntiguo";
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
                    MessageBox.Show($"Se ha editado la informacion del local: {Local.nombre }");
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

                    string cadena = $"delete from PuntoVenta Where CodigoPuntoVenta = '{Codigo}' ";
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



        /// <summary>
        /// Sincronizar una tabla local con una del servidor.
        /// </summary> 
        #endregion

        //public bool NuevoProveedor()
        //{
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(_connString))
        //        {
        //            string cadena = "INSERT INTO Clientes(Nombres,Apellidos,CedulaCliente,Email,Telefono,Puntos) VALUES (@name,@lastname,@cedula,@correo,@telefono, 100 )";
        //            SqlCommand cmd = new SqlCommand(cadena, conn);
        //            cmd.Parameters.AddWithValue("@name", Statics.PrimeraAMayuscula(Cliente.FirstName));
        //            cmd.Parameters.AddWithValue("@lastname", Statics.PrimeraAMayuscula(Cliente.LastName));
        //            cmd.Parameters.AddWithValue("@cedula", Cliente.Cedula);
        //            cmd.Parameters.AddWithValue("@correo", string.IsNullOrEmpty(Cliente.Correo) ? (object)DBNull.Value : Cliente.Correo);
        //            cmd.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(Cliente.Telefono) ? (object)DBNull.Value : Cliente.Telefono);
        //            conn.Open();
        //            cmd.ExecuteNonQuery();
        //            conn.Close();
        //            return true;

        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        if (e.Message.Substring(0, 50) == $"Violation of PRIMARY KEY constraint 'PK_Clientes'.")
        //        {
        //            MessageBox.Show($"La cedula {Cliente.Cedula} ya esta registrada.");
        //            return false;
        //        }
        //        else
        //        {
        //            MessageBox.Show(e.Message);
        //            return false;
        //        }
        //    }
        //}
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

    
