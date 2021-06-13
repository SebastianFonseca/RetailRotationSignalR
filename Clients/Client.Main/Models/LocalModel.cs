using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Main.Models
{
    public class LocalModel
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Direccion;
        public string Telefono;
        public string Ciudad;
        public int NumeroDeCanastillas;
        public DateTime FechaDeApertura { get; set; } = DateTime.Now;


    }
}
