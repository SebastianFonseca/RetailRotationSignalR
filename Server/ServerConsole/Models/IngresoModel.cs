using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerConsole.Models
{
    public class IngresoModel
    {
        public string id { get; set; }

        public string puntoPago { get; set; }

        public LocalModel puntoVenta { get; set; } = new LocalModel();

        public DateTime fecha { get; set; }

        public decimal? valor { get; set; }

        public decimal? efectivo { get; set; }

        public decimal? diferencia { get; set; }

        public EmpleadoModel supervisor { get; set; } = new EmpleadoModel();
        public EmpleadoModel cajero { get; set; } = new EmpleadoModel();

        public BindableCollection<FacturaModel> facturas { get; set; } = new BindableCollection<FacturaModel>();


    }
}
