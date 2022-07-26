using Caliburn;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerConsole.Models
{
    public class MovimientoEfectivoModel : Screen
    {
        public int id { get; set; }
        public decimal? aumentoDisminucion { get; set; }
        public decimal total { get; set; }
        public IngresoModel ingreso { get; set; } = new IngresoModel();
        public EgresoModel egreso { get; set; } = new EgresoModel();
        public LocalModel local { get; set; } = new LocalModel();

        /*public string tipo { get; set; }
        public DateTime fecha { get; set; }
        ProveedorModel proveedor { get; set; }
        public decimal valor { get; set; }*/

    }

}
