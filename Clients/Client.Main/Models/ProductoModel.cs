﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Main.Models
{
    public class ProductoModel : Screen
    {
        public string codigoProducto { get; set; }
        public string nombre { get; set; }
        public string unidadVenta { get; set; } = "";
        public string unidadCompra { get; set; } = "";
        public string seccion { get; set; }
        public DateTime fechaVencimiento { get; set; } = DateTime.Now;
        public decimal iva { get; set; }
        public string codigoBarras { get; set; }
        public int? existencia { get; set; }
        public int? pedido { get; set; }
        public int? sumaPedido { get; set; }
        public int? compra { get; set; }
        public string estado { get; set; }

        /// <summary>
        /// warning si el precio de venta es cero en una venta
        /// </summary>
        public decimal precioVenta { get; set; } = 0;

        /// <summary>
        ///Propiedad necesaria para el binding de datos en las listbox con el productoseleccionado.
        /// </summary>
        bool _isSelected;
        public bool isSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange();
            }
        }

    }
}
