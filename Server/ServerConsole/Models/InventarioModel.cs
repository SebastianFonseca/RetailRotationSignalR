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
        public int idRegistro { get; set; }
        public string idlocalOServidor { get; set; }

    }
}
