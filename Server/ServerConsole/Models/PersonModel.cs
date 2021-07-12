using System;
using System.Collections.Generic;
using System.Text;

namespace ServerConsole.Models
{
    public class PersonModel
    {
        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
            }
        }

        public string LastName
        {
            get;
            set;
        }

        public string Cedula
        {
            get;
            set;
        }


        public string Telefono { get; set; } = null;

        public string Direccion { get; set; } = null;

        public string Correo
        {
            get;
            set;
        } = null;

    }
}
