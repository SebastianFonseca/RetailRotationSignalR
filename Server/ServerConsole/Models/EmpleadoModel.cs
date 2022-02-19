using System;
using System.Collections.Generic;
using System.Text;

namespace ServerConsole.Models
{
    public class EmpleadoModel : PersonModel
    {
        public DateTime FechaDeContratacion { get; set; } = DateTime.Now;
        public decimal Salario { get; set; }
        public string Password { get; set; } = null;
        public string Cargo { get; set; }
        public LocalModel PuntoDeVenta { get; set; } = new LocalModel();
    }
}
