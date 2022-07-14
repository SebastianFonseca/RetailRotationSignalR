using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Main.Models
{
    public class ComprasModel: DocumentoModel
    {

        public int? numeroCanastillas { get; set; } = null;
        public decimal? peso { get; set; } = null;

        /// <summary>
        /// Lista con los pedidos que componen el documento de compras.
        /// </summary>
        public BindableCollection<string> codPedidos { get; set; } = new BindableCollection<string>();

        /// <summary>
        /// Lista de productos con la suma de la cantidad pedida.
        /// </summary>
        public BindableCollection<ProductoModel> sumaPedidos { get; set; } = new BindableCollection<ProductoModel>();

        public ComprasModel()
        {

        }
        public ComprasModel(BindableCollection<PedidoModel> pedidos)
        {
            this.codigo = DateTime.Now.ToString("ddMMyyyyHHmm");
            this.fecha = DateTime.Today.Date;

            ///Coleccion de strings que contendra todos los codigos de los productos para facilitar su manipulacion
            BindableCollection<string> codigosProductos = new BindableCollection<string>();

            ///Se  guarda el codigo del pedido de la posicion 0 en la lista de codigos de pedido que componen la compra
            codPedidos.Add(pedidos[0].codigo);

            ///Se asume la cantidad de la compra en la posicion 0 como la cantidad inicial
            foreach (ProductoModel producto in pedidos[0].productos)
            {
                producto.sumaPedido = producto.pedido;
                codigosProductos.Add(producto.codigoProducto);
                producto.unidadCompra = producto.unidadCompra.Substring(0, 3) + ".";
                producto.unidadVenta = producto.unidadVenta.Substring(0, 3) + ".";
                sumaPedidos.Add(producto);

            }
            ///Si no hay mas pedidos que solamete 1 ya se proceso anteriormente el primero.
            if (pedidos.Count == 1)
            {
                for (int i = 0; i < sumaPedidos.Count; i++)
                {
                    sumaPedidos[i].sumaPedido = (int?)(sumaPedidos[i].sumaPedido / sumaPedidos[i].factorConversion);

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
                        int posicionProductPedido = codigosProductos.IndexOf(productoPedido.codigoProducto);
                        ///si se encontro el produto en los codigos de los productos del pedido en la posicion 0. Si retorna posicion si esta, si retorna -1 no esta, debe agregarse
                        if(posicionProductPedido >= 0)
                        {
                            ///Se adiciona la nueva cantidad del nuevo pedido que se esta iterando
                            sumaPedidos[posicionProductPedido].sumaPedido = sumaPedidos[posicionProductPedido].sumaPedido + productoPedido.pedido;
                            productoPedido.unidadCompra = productoPedido.unidadCompra.Substring(0, 3) + ".";
                            productoPedido.unidadVenta = productoPedido.unidadVenta.Substring(0, 3) + ".";

                        }
                        ///El producto del pedido en iteracion no esta en la lista de los productos en la posicion 0 asi que se agrega.
                        else
                        {
                            productoPedido.sumaPedido = productoPedido.pedido;
                            productoPedido.unidadCompra = productoPedido.unidadCompra.Substring(0,3)+".";
                            productoPedido.unidadVenta = productoPedido.unidadVenta.Substring(0, 3) + ".";
                            sumaPedidos.Add(productoPedido);
                        }
 
                    }
                }
            }

            for (int i = 0; i < sumaPedidos.Count; i++)
            {
                sumaPedidos[i].sumaPedido = (int?)(sumaPedidos[i].sumaPedido / sumaPedidos[i].factorConversion);
            }
        }
    }
}
