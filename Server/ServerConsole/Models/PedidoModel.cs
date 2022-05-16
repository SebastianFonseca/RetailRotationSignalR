using System;
using System.Collections.Generic;
using System.Text;

namespace ServerConsole.Models
{
    public class PedidoModel : DocumentoModel
    {
        public static implicit operator PedidoModel(ExistenciasModel v)
        {
            PedidoModel pedido = new PedidoModel()
            {
                codigo = v.codigo + ":" + DateTime.Now.ToString("ddMMyyyyHHmm"),
                fecha = DateTime.Today,
                responsable = v.responsable,
                puntoVenta = v.puntoVenta,
                productos = v.productos
            };
            return pedido;
        }
    }
}
