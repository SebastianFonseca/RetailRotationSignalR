using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Caliburn;
using Caliburn.Micro;

namespace Client.Main.Models
{
    public class IngresoModel
    {
        public IngresoModel()
        {
            this.id = ConfigurationManager.AppSettings["Caja"].Trim() + ":" + DateTime.Now.ToString("ddMMyyyyHHmmssffffff");
            this.fecha = DateTime.Now;
            this.puntoPago = ConfigurationManager.AppSettings["Caja"];
            this.puntoVenta.codigo = this.puntoPago.Split(':')[0];
        }

        public string id { get; set; }

        public string puntoPago { get; set; }

        public LocalModel puntoVenta { get; set; } = new LocalModel();

        public DateTime fecha { get; set; }

        public decimal? valor { get; set; }

        public decimal? efectivo { get; set; }

        public decimal? diferencia { get; set; }

        public EmpleadoModel supervisor { get; set; } = new EmpleadoModel();
        public EmpleadoModel Cajero { get; set; } = new EmpleadoModel();

        public BindableCollection<FacturaModel> facturas { get; set; } = new BindableCollection<FacturaModel>();

    }
}
