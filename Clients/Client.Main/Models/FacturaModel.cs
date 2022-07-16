using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;


namespace Client.Main.Models
{
    [Serializable]
    public  class FacturaModel : DocumentoModel
    {

        public FacturaModel()
        {
            this.codigo = ConfigurationManager.AppSettings["Caja"].Trim() + ":" + DateTime.Now.ToString("ddMMyyyyHHmmssffffff"  ) ;
           /* this.fecha = DateTime.Now;*/
            this.puntoDePago = ConfigurationManager.AppSettings["Caja"];
            this.puntoVenta.codigo = this.puntoDePago.Split(':')[0];
        }
        public ClientesModel cliente { get; set; } = new ClientesModel();

        public string puntoDePago { get; set; }

        public decimal? valorTotal { get; set; } = 0;

        public decimal? ivaTotal { get; set; } = 0;


        public decimal? descuentoTotal { get; set; } = 0;

        public EmpleadoModel superAuto { get; set; } = new EmpleadoModel();


        public string observaciones { get; set; }

    }
}
