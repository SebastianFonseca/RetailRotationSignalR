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
    public class CompraViewModel:Screen
    {
        public MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public BindableCollection<PedidoModel> pedidosSeleccionados = new BindableCollection<PedidoModel>();
        public ComprasModel compra;

        public CompraViewModel(MainWindowViewModel argVentana, BindableCollection<PedidoModel> pedidos)
        {
            VentanaPrincipal = argVentana;
            compra = new ComprasModel(pedidos);
        }

        public void BackButton()
        {
            if(Productos!= null)
                Productos.Clear();
            VentanaPrincipal.ActivateItem(new ComprasNuevoViewModel(VentanaPrincipal));
        }

        public BindableCollection<ProductoModel> Productos
        {
            get => compra.sumaProductosPedidos;
            set
            {
                compra.sumaProductosPedidos = value;
                NotifyOfPropertyChange(() => Productos);
            }

        }


    }
}
