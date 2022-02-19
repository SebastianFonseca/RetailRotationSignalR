using System;
using System.Collections.Generic;
using System.Text;

namespace ServerConsole.Models
{
    public class ProductoModel
    {
        public string CodigoProducto { get; set; }
        public string Nombre { get; set; }
        public string UnidadVenta { get; set; }
        public string UnidadCompra { get; set; }
        /// <summary>
        /// warning si el precio de venta es cero en una venta
        /// </summary>
        public decimal PrecioVenta { get; set; } = 0;
        public string Seccion { get; set; }
        public DateTime FechaVencimiento { get; set; } = DateTime.Now;
        public decimal IVA { get; set; }
        public string CodigoBarras { get; set; }

    }
}
