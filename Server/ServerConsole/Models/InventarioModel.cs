using System;
using System.Collections.Generic;
using System.Text;

namespace ServerConsole.Models
{
    public class InventarioModel: DocumentoModel
    {
        public string codigoProducto { get; set; }
        public string tipo { get; set; }
        public decimal? aumentoDisminucion { get; set; }
        public decimal? total { get; set; }
        public int? idRegistroLocal { get; set; }

        public int? idRegistroServidor { get; set; }

        public string codigoDelInventarioDelLocal { get; set; }


        public string recibido { get; set; }


        public string factura { get; set; }
    }
}
