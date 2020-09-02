using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Client.Main.Utilities
{

    public static class DbConnection
    {
        /// <summary>
        /// Return the connection string from App.config file of the giving name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;            
        }



    }
}
