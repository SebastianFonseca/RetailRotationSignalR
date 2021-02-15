using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Text;
using System.Windows;

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
            try {

                using (IDbConnection conn = new SqlConnection(_connString))
                {

                    var parameters = new DynamicParameters();
                    parameters.Add("@cedula", User);
                    parameters.Add("@password", Password);
                    parameters.Add(name: "@RetVal", dbType: DbType.Int32,direction: ParameterDirection.ReturnValue);


                    var returnCode = conn.Execute(sql:"Login", param:parameters, commandType: CommandType.StoredProcedure);                    
                    if (parameters.Get<Int32>("@RetVal") == 0)
                        return false;
                    else 
                        return true;
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
           
        }


        public static void AddClient(string pNombre, string pApellidos,string pCedula ,string pCorreo, string pTelefono )
        {
            try
            {

                using (IDbConnection conn = new SqlConnection(_connString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Nombre", pNombre);
                    parameters.Add("@Apellidos", pApellidos);
                    parameters.Add("@Cedula", pCedula);
                    parameters.Add("@Correo", pCorreo);
                    parameters.Add("@Telefono", pTelefono);
                    parameters.Add(name: "@RetVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                    var returnCode = conn.Execute(sql: "AddClient", param: parameters, commandType: CommandType.StoredProcedure);
                    if (parameters.Get<Int32>("@RetVal") == 0)
                    {
                        MessageBox.Show($"El cliente {pNombre} {pApellidos} ha recibido 100 puntos por registrarce.");
                    }
                    else
                        if (parameters.Get<Int32>("@RetVal") == 1)
                    {
                        MessageBox.Show($"Ya se registro un cliente con la cedula: {pCedula}.");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
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
