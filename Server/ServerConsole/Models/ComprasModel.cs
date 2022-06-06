using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerConsole.Models
{
    public class ComprasModel : DocumentoModel
    {

        public int? numeroCanastillas { get; set; } = null;
        public int? peso { get; set; } = null;

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


    }
}
