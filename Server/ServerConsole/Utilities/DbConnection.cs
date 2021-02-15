using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace ServerConsole.Utilities
{
    class DbConnection
    {

        private static string _connString = ConnectionString("RetailRotationServerDataBase");

        public static bool Login(string User, string Password)
        {



            try
            {

                using (IDbConnection conn = new SqlConnection(_connString))
                {

                    var parameters = new DynamicParameters();
                    parameters.Add("@cedula", User);
                    parameters.Add("@password", Password);
                    parameters.Add(name: "@RetVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);


                    var returnCode = conn.Execute(sql: "Login", param: parameters, commandType: CommandType.StoredProcedure);

                    if (parameters.Get<Int32>("@RetVal") == 0)
                        return false;
                    else
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
