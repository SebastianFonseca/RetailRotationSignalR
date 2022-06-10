using System;
using System.Collections.Generic;
using System.Text;

namespace ServerConsole.Models
{
    public class EmpleadoModel : PersonModel
    {
        public DateTime fechaDeContratacion { get; set; } = DateTime.Now;
        public decimal? salario { get; set; }
        public string password { get; set; } = null;
        public string cargo { get; set; }
        public LocalModel puntoDeVenta { get; set; } = new LocalModel();
    }
}
