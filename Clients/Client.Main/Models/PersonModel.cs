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
    public class PersonModel : PropertyChangedBase
    {

        private String _cedula;

        public String Cedula
        {
            get { return _cedula; }
            set
            {
                NotifyOfPropertyChange(() => Cedula);
                _cedula = value; 
            }
        }


        public string FirstName { get; set; }


        public string LastName { get; set; }

        public string Telefono { get; set; }

        public string  Direccion { get; set; }

        public string Correo { get;
            set; }


    }
 }
        

