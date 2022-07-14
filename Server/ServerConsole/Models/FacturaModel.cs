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

    }
}
