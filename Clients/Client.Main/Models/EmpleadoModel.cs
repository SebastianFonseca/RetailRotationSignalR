using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Main.Models
{
    public class EmpleadoModel : PersonModel
    {
        public DateTime FechaDeContratacion { get; set; } = DateTime.Now;
        public string Password { get; set; } = null;
        public string Cargo { get; set; }
    }
}
