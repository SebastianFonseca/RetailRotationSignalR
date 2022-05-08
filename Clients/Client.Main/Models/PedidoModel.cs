using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Main.Models
{
    public class PedidoModel : DocumentoModel
    {
        public static implicit operator PedidoModel(ExistenciasModel v)
        {
            PedidoModel pedido = new PedidoModel()
            {
                codigo = v.codigo,
                fecha = v.fecha,
                responsable = v.responsable,
                puntoVenta = v.puntoVenta,
                productos = v.productos
            };
            return pedido;
        }
    }
}
