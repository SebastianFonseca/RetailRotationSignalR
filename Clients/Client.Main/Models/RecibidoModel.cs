using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Main.Models
{
    public class RecibidoModel : DocumentoModel
    {

        public DateTime fechaRecibido { get; set; } = DateTime.Now.Date;
        public string nombreConductor { get; set; }
        public int? numeroCanastillas { get; set; } = null;
        public int? peso { get; set; } = null;

        public static implicit operator RecibidoModel(EnvioModel v)
        {
            return new RecibidoModel() 
            {
                codigo = v.codigo,
                productos = v.productos,
                puntoVenta = v.puntoVenta,
                responsable = v.responsable
            };
        }
    }
}
