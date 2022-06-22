using System;
using System.Collections.Generic;
using System.Text;

namespace ServerConsole.Models
{
    public class ProductoModel
    {
        public string codigoProducto { get; set; }
        public string nombre { get; set; }
        public string unidadVenta { get; set; } = "";
        public string unidadCompra { get; set; } = "";
        public decimal factorConversion { get; set; } = 1;
        public string seccion { get; set; }
        public DateTime fechaVencimiento { get; set; } = DateTime.Today;

        public decimal iva { get; set; }
        public string codigoBarras { get; set; }
        public int? existencia { get; set; }

        public int? pedido { get; set; }

        public int? sumaPedido { get; set; }

        public int? compra { get; set; }

        public int? compraPorLocal { get; set; }
        public int? recibido { get; set; } = null;
        public ProveedorModel proveedor { get; set; } = new ProveedorModel();
        public string estado { get; set; }
        public DateTime? fechaDePago { get; set; }
        public DateTime? fechaDeCompra { get; set; }

        public string soportePago { get; set; }

        public string codigoCompra { get; set; }

        /// <summary>
        /// warning si el precio de venta es cero en una venta
        /// </summary>
        public decimal? precioVenta { get; set; }
        public decimal? precioCompra { get; set; }

    }
}
