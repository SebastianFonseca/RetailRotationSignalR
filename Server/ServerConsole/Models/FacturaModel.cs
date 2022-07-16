using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace ServerConsole.Models
{
    public class FacturaModel : DocumentoModel
    {
        public FacturaModel()
        {
            this.codigo = ConfigurationManager.AppSettings["Caja"].Trim() + ":" + DateTime.Now.ToString("ddMMyyyyHHMMssffffff");
            this.fecha = DateTime.Today;
            this.puntoDePago = ConfigurationManager.AppSettings["Caja"];
            this.puntoVenta.codigo = this.puntoDePago.Split(':')[0];
        }
        public ClientesModel cliente { get; set; } = new ClientesModel();

        public string puntoDePago { get; set; }

        public decimal? valorTotal { get; set; }

        public decimal? ivaTotal { get; set; } = 0;


        public decimal? descuentoTotal { get; set; } = 0;

        public EmpleadoModel superAuto { get; set; } = new EmpleadoModel();
        public string observaciones { get; set; }


    }
}
