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
        public decimal? factorConversion { get; set; } = 1;
        public string seccion { get; set; }
        public DateTime fechaVencimiento { get; set; } = DateTime.Today;

        public decimal? iva { get; set; }
        public string codigoBarras { get; set; }
        public decimal? existencia { get; set; }

        public decimal? pedido { get; set; }

        public decimal? sumaPedido { get; set; }

        public decimal? compra { get; set; }

        public decimal? compraPorLocal { get; set; }
        public decimal? recibido { get; set; } = null;
        public decimal? cambioInventario { get; set; }
        public ProveedorModel proveedor { get; set; } = new ProveedorModel();
        public string estado { get; set; }
        public DateTime? fechaDePago { get; set; }
        public DateTime? fechaDeCompra { get; set; }

        public string soportePago { get; set; }

        public string codigoCompra { get; set; }
        public decimal? precioVenta { get; set; }

        public decimal? precioVentaConDescuento { get; set; }
        public decimal? precioCompra { get; set; }
        public decimal? porcentajePromocion { get; set; }

        private decimal? _cantidadVenta = 0;
        public decimal? cantidadVenta
        {
            get { return _cantidadVenta; }
            set { _cantidadVenta = value; }
        }

        private decimal? _totalValorVenta;
        public decimal? totalValorVenta
        {
            get { return _totalValorVenta; }
            set { _totalValorVenta = value; }
        }

    }
}
