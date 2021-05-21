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



        private string _firstName;

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (value == null)
                    MessageBox.Show("Pasando a nulo");

                _firstName = value; 
            }
        }



        public string LastName { 
            get; 
            set; }

        public string Cedula { 
            get; 
            set; }


        public string Telefono { get; set; } = null;

        public string  Direccion { get; set; } = null;

        public string Correo { get;
            set; } = null;


    }
 }
        

