using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Main.Models
{
    public class ComprasModel: DocumentoModel
    {
        /// <summary>
        /// Lista de productos con la suma de la cantidad pedida.
        /// </summary>
        public BindableCollection<ProductoModel> sumaProductosPedidosTransformadoUnidadCompra = new BindableCollection<ProductoModel>();
        BindableCollection<string> codPedidos = new BindableCollection<string>();

        public ComprasModel(BindableCollection<PedidoModel> pedidos)
        {
            this.codigo = DateTime.Today.ToString("ddMMyyyyHHmm");
            this.fecha = DateTime.Today.Date;

            ///Coleccion de strings que contendra todos los codigos de los productos para facilitar su manipulacion
            BindableCollection<string> codigos = new BindableCollection<string>();

            ///Se  guarda el codigo del pedido de la posicion 0 en la lista de codigos de pedido que componen la compra
            codPedidos.Add(pedidos[0].codigo);

            ///Se asume la cantidad de la compra en la posicion 0 como la cantidad inicial
            foreach (ProductoModel producto in pedidos[0].productos)
            {
                producto.sumaPedido = producto.pedido;
                codigos.Add(producto.codigoProducto);
                producto.unidadCompra = producto.unidadCompra.Substring(0, 3) + ".";
                producto.unidadVenta = producto.unidadVenta.Substring(0, 3) + ".";
                sumaProductosPedidosTransformadoUnidadCompra.Add(producto);

            }
            ///Si no hay mas pedidos que solamete 1 ya se proceso anteriormente el primero.
            if (pedidos.Count == 1)
            {
                for (int i = 0; i < sumaProductosPedidosTransformadoUnidadCompra.Count; i++)
                {
                    sumaProductosPedidosTransformadoUnidadCompra[i].sumaPedido = (int?)(sumaProductosPedidosTransformadoUnidadCompra[i].sumaPedido / sumaProductosPedidosTransformadoUnidadCompra[i].factorConversion);
                    if (sumaProductosPedidosTransformadoUnidadCompra[i].sumaPedido == 0)
                    {
                        sumaProductosPedidosTransformadoUnidadCompra.Remove(sumaProductosPedidosTransformadoUnidadCompra[i]);
                    }
                }

                return;
            }
            else
            { 
                ///Cada uno de los pedidos dados como parametro al constructor
                for (int i = 1; i < pedidos.Count; i++)
                {
                    ///Agregar el codigo del pedido a la lista de codigos de la compra
                    codPedidos.Add(pedidos[i].codigo);

                    ///Cada producto en cada uno de los pedidos.
                    foreach (ProductoModel productoPedido in pedidos[i].productos)
                    {

                        ///La posicon del producto pedido dado su codigo en la lista de codigos obtenida de los productos en el pedido de la posicion 0, para sumarle la cantidad de este nuevo pedido en la posicion i
                        int posicionProductPedido = codigos.IndexOf(productoPedido.codigoProducto);
                        ///si se encontro el produto en los codigos de los productos del pedido en la posicion 0. Si retorna posicion si esta, si retorna -1 no esta, debe agregarse
                        if(posicionProductPedido >= 0)
                        {
                            ///Se adiciona la nueva cantidad del nuevo pedido que se esta iterando
                            sumaProductosPedidosTransformadoUnidadCompra[posicionProductPedido].sumaPedido = sumaProductosPedidosTransformadoUnidadCompra[posicionProductPedido].sumaPedido + productoPedido.pedido;
                            productoPedido.unidadCompra = productoPedido.unidadCompra.Substring(0, 3) + ".";
                            productoPedido.unidadVenta = productoPedido.unidadVenta.Substring(0, 3) + ".";

                        }
                        ///El producto del pedido en iteracion no esta en la lista de los productos en la posicion 0 asi que se agrega.
                        else
                        {
                            productoPedido.sumaPedido = productoPedido.pedido;
                            productoPedido.unidadCompra = productoPedido.unidadCompra.Substring(0,3)+".";
                            productoPedido.unidadVenta = productoPedido.unidadVenta.Substring(0, 3) + ".";
                            sumaProductosPedidosTransformadoUnidadCompra.Add(productoPedido);
                        }
 
                    }
                }
            }

            for (int i = 0; i < sumaProductosPedidosTransformadoUnidadCompra.Count; i++)
            {
                sumaProductosPedidosTransformadoUnidadCompra[i].sumaPedido = (int?)(sumaProductosPedidosTransformadoUnidadCompra[i].sumaPedido / sumaProductosPedidosTransformadoUnidadCompra[i].factorConversion);
                if (sumaProductosPedidosTransformadoUnidadCompra[i].sumaPedido == 0)
                {
                    sumaProductosPedidosTransformadoUnidadCompra.Remove(sumaProductosPedidosTransformadoUnidadCompra[i]);
                }
            }



        }
    }
}
