using System;
using System.Collections.Generic;
using System.Text;

namespace ServerConsole.Models
{
    public class LocalModel
    {
        public string codigo { get; set; }
        public string nombre { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
        public string ciudad { get; set; }
        public int numeroDeCanastillas { get; set; } 
        public DateTime fechaDeApertura { get; set; } = DateTime.Now;
    }
}
