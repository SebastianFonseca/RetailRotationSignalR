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

namespace Client.Main.Utilities
{
    public class DbConnection
    {

        private static string _connString = ConnectionString("RetailRotationClientDataBase");
        /// <summary>
        /// Metodo responsable de verificar en la base de datos localmente si el usuario que intenta ingresar esta o no registrado.
        /// </summary>
        /// <param name="User"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static bool Login(string User, string Password)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string cadena = "SELECT [Password]  FROM EMPLEADO where [CedulaEmpleado]=@user";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@user", User);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows) { return false; }
                        while (reader.Read())
                        {
                            if (Statics.Verify(Password,reader["Password"].ToString()))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                            
                        }
                        conn.Close();
                        return false;
                    }
                    
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
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
                    MessageBox.Show($"El cliente {Cliente.FirstName} {Cliente.LastName} ha recibido 100 puntos por registrarce.");
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

        public static bool NuevoUsuario(EmpleadoModel Empleado)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {

                    string cadena = "INSERT INTO Empleado(CedulaEmpleado,CodigoPuntoVenta,Nombres,Apellidos,FechaContratacion,Salario,Telefono,Cargo,Password) VALUES " +
                                                        "(@cedula,@puntodeventa,@nombre,@apellidos,@fecha,@salario,@telefono,@cargo,@contraseña);";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@cedula", Empleado.Cedula);
                    cmd.Parameters.AddWithValue("@puntodeventa", Empleado.PuntoDeVenta.Codigo);
                    cmd.Parameters.AddWithValue("@nombre", Statics.PrimeraAMayuscula(Empleado.FirstName));
                    cmd.Parameters.AddWithValue("@apellidos", Statics.PrimeraAMayuscula(Empleado.LastName));
                    cmd.Parameters.AddWithValue("@fecha", Empleado.FechaDeContratacion.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@salario", Empleado.Salario);
                    cmd.Parameters.AddWithValue("@telefono", Empleado.Telefono);
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


        public static BindableCollection<EmpleadoModel> empleados = new BindableCollection<EmpleadoModel>();
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

        public static bool NuevoLocal(LocalModel NuevoLocal)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {

                    string cadena = "INSERT INTO PuntoVenta(codigoPuntoVenta,Nombres,Direccion,Telefono,Ciudad,NumeroCanastillas,FechaDeApertura,Administrador) " +
                                                   "VALUES (@codigo,@nombre,@direccion,@telefono,@ciudad,@nrocanastillas,@fechaapertura,@administrador)";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    cmd.Parameters.AddWithValue("@codigo", NuevoIdLocal());
                    cmd.Parameters.AddWithValue("@nombre", Statics.PrimeraAMayuscula(NuevoLocal.Nombre));
                    cmd.Parameters.AddWithValue("@direccion", Statics.PrimeraAMayuscula(NuevoLocal.Direccion));
                    cmd.Parameters.AddWithValue("@telefono", Statics.PrimeraAMayuscula(NuevoLocal.Telefono));
                    cmd.Parameters.AddWithValue("@ciudad", Statics.PrimeraAMayuscula(NuevoLocal.Ciudad));
                    cmd.Parameters.AddWithValue("@nrocanastillas", Statics.PrimeraAMayuscula(NuevoLocal.NumeroDeCanastillas.ToString()));
                    cmd.Parameters.AddWithValue("@fechaapertura", Statics.PrimeraAMayuscula(NuevoLocal.FechaDeApertura.Date.ToString("yyyy-MM-dd")));
                    //cmd.Parameters.AddWithValue("@administrador", Statics.PrimeraAMayuscula(NuevoLocal.Administrador.Cedula));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show($"Se ha registrado al nuevo usuario {NuevoLocal.Nombre }");
                    conn.Close();
                    return true;

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                
                if (e.HResult.ToString() == "-2146232060")
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


        public static BindableCollection<LocalModel> locales = new BindableCollection<LocalModel>();
        public static BindableCollection<LocalModel> getLocales()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    LocalModel local = new LocalModel();
                    string cadena = "SELECT [CodigoPuntoVenta], [Nombres], [NumeroCanastillas]  FROM PuntoVenta";
                    SqlCommand cmd = new SqlCommand(cadena, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            local.Codigo = reader["CodigoPuntoVenta"].ToString();
                            local.Nombre = reader["Nombres"].ToString();
                            local.NumeroDeCanastillas = Convert.ToInt32(reader["NumeroCanastillas"]);
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
