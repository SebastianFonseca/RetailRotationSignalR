using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Client.Main.Models
{
    public class ClientesModel : PersonModel/*, IDataErrorInfo*/
    {
        public int Puntos { get;
            set; }
        //public string Error { get { return null; } }
        //bool flag = false;
        //public string this[string name]
        //{
        //    get
        //    {
        //        string result = null;
        //        long number = 0;
        //        if (flag)
        //        {
        //            if (name == "Cedula")
        //            {
        //                if (!long.TryParse(Cedula, out number))
        //                {
        //                    result = "La cedula no debe contener letras.";
        //                }
        //            }
        //        }
        //        else { flag = true; }


        //        return result;
        //    }
        //}
    }

}
