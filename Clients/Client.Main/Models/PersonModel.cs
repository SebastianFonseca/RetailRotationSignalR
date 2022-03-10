using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;

namespace Client.Main.Models
{
    public class PersonModel 
    {
        public string firstName { get; set; }

        public string lastName { get; set; }

        public string cedula { get; set; }

        public string telefono { get; set; } = null;

        public string  direccion { get; set; } = null;

        public string correo { get; set; } = null;

    }
 }


//private string _firstName;
//public string firstName
//{
//    get { return _firstName; }
//    set
//    {
//        _firstName = value; 
//    }
//}