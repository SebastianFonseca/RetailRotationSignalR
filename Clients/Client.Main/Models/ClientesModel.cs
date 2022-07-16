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

    public class ClientesModel : PersonModel
    {
        public decimal puntos { get; set; }
        public string estado { get; set; }

    }

}
