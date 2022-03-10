using System;
using System.Collections.Generic;
using System.Text;

namespace ServerConsole.Models
{
    public class PersonModel
    {
        private string _firstName;
        public string firstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
            }
        }

        public string lastName
        {
            get;
            set;
        }

        public string cedula
        {
            get;
            set;
        }


        public string telefono { get; set; } = null;

        public string direccion { get; set; } = null;

        public string correo
        {
            get;
            set;
        } = null;

    }
}
