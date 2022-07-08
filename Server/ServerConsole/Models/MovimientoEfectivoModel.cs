using System;
using System.Collections.Generic;
using System.Text;
using Caliburn;
using Caliburn.Micro;

namespace ServerConsole.Models
{
     public class MovimientoEfectivoModel : Screen
    {
        public int id { get; set; }
        public decimal aumentoDisminucion { get; set; }
        public decimal total { get; set; }
        public int? codIngreso { get; set; }
        public EgresoModel egreso { get; set; } = new EgresoModel();
        public LocalModel local { get; set; }

        /*public string tipo { get; set; }
        public DateTime fecha { get; set; }
        ProveedorModel proveedor { get; set; }
        public decimal valor { get; set; }*/

    }
    
}
