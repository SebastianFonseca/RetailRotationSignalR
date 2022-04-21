using System;
using System.Collections.Generic;
using System.Text;

namespace ServerConsole.Models
{
    public class ProductoModel
    {
        public string codigoProducto { get; set; }
        public string nombre { get; set; }
        public string unidadVenta { get; set; }
        public string unidadCompra { get; set; }
        /// <summary>
        /// warning si el precio de venta es cero en una venta
        /// </summary>
        public decimal precioVenta { get; set; } = 0;
        public string seccion { get; set; }
        public DateTime fechaVencimiento { get; set; } = DateTime.Today;
        public decimal iva { get; set; }
        public string codigoBarras { get; set; }
        public int? existencia { get; set; } = null;
        public string estado { get; set; }

    }
}
