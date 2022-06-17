using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main.ViewModels
{
    class ComprasPagosEditarViewModel : Screen
    {
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public BindableCollection<PedidoModel> pedidosSeleccionados = new BindableCollection<PedidoModel>();

        public ComprasPagosEditarViewModel(MainWindowViewModel argVentana, string codigoCedula)
        {
            Info = codigoCedula;
            VentanaPrincipal = argVentana;
            getRegistros(codigoCedula);
        }


        public async void getRegistros(string caracteres) 
        {


            Productos = DbConnection.getRegistroCompraCodigoCedula(caracteres);
            foreach (ProductoModel producto in Productos)
            {
                producto.precioCompra = producto.precioCompra;
                if(producto.precioCompra != null & producto.compra != null)
                Total = (decimal)(Total + (producto.precioCompra * producto.compra));
            }

        }

        private decimal _total;
        public decimal Total
        {
            get { return _total; }
            set 
            { 
                _total = value;
                NotifyOfPropertyChange(() => Total);
            }
        }



        private BindableCollection<ProductoModel> _productos;

        public BindableCollection<ProductoModel> Productos
        {
            get { return _productos; }
            set
            {
                _productos = value;
                NotifyOfPropertyChange(() => Productos);
            }
        }

        private string _info;

        public string Info
        {
            get { return _info; }
            set 
            { 
                _info = value;
                NotifyOfPropertyChange(() => Info);
            }
        }


        public void BackButton()
        {
            if (Productos != null)
            {
                Productos.Clear();
            }

            VentanaPrincipal.ActivateItem(new ComprasPagosViewModel(VentanaPrincipal));
        }



    }
}
