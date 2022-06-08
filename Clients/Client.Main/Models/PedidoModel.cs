using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Main.Models
{
    public class PedidoModel : DocumentoModel
    {
        public static explicit operator PedidoModel(ExistenciasModel v)
        {
            PedidoModel pedido = new PedidoModel()
            {
                codigo = v.codigo + ":" + DateTime.Now.ToString("ddMMyyyyHHmm "),
                fecha = DateTime.Today,
                responsable = v.responsable,
                puntoVenta = v.puntoVenta,
                productos = v.productos
            };
            return pedido;
        }
        /// <summary>
        ///Propiedad necesaria para el binding de datos en las listbox.
        /// </summary>
        bool _isSelected;
        public bool isSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange();
            }
        }
    }
}
