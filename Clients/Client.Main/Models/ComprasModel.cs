using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Main.Models
{
    public class ComprasModel: DocumentoModel
    {
        public BindableCollection<ProductoModel> sumaProductosPedidos = new BindableCollection<ProductoModel>();
        BindableCollection<string> codPedidos = new BindableCollection<string>();

        public ComprasModel(BindableCollection<PedidoModel> pedidos)
        {
            BindableCollection<string> codigos = new BindableCollection<string>();
            codPedidos.Add(pedidos[0].codigo);
            foreach (ProductoModel producto in pedidos[0].productos)
            {
                producto.sumaPedido = producto.pedido;
                codigos.Add(producto.codigoProducto);
                sumaProductosPedidos.Add(producto);
                
            }
            if (pedidos.Count == 1)
            {
                return;
            }
            else { 

                for (int i = 1; i < pedidos.Count; i++)
                {
                    foreach (ProductoModel productoPedido in pedidos[i].productos)
                    {
                        codPedidos.Add(pedidos[i].codigo);
                        int posicionProductPedido = codigos.IndexOf(productoPedido.codigoProducto);
                        if(posicionProductPedido >= 0)
                        {
                            sumaProductosPedidos[posicionProductPedido].sumaPedido = sumaProductosPedidos[posicionProductPedido].sumaPedido + productoPedido.pedido;

                        }
                        else
                        {
                            productoPedido.sumaPedido = productoPedido.pedido;
                            sumaProductosPedidos.Add(productoPedido);
                        }


                        //for (int j = 0; j <=  sumaProductosPedidos.Count; j ++)
                        //{
                        //    if (productoPedido.codigoProducto == sumaProductosPedidos[j].codigoProducto)
                        //    {
                        //        sumaProductosPedidos[j].sumaPedido = sumaProductosPedidos[j].sumaPedido + productoPedido.pedido;
                        //        continue;
                        //    }
                        //    else if(j == sumaProductosPedidos.Count)
                        //    {
                        //        productoPedido.sumaPedido = productoPedido.pedido;
                        //        sumaProductosPedidos.Add(productoPedido);
                        //    }
                        //}
                    }
                }
            }
        }
    }
}
