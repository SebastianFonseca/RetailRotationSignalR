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
    public abstract class PersonModel 
    {
        public string cedula { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public string telefono { get; set; } = null;

        public string  direccion { get; set; } = null;

        public string correo { get; set; } = null;

    }
 }

