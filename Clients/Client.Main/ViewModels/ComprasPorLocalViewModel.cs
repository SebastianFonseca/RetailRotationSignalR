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
    public class ComprasPorLocalViewModel : Screen
    {
        public MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public PedidoModel pedido;

        public ComprasPorLocalViewModel(MainWindowViewModel argVentana, PedidoModel pedido)
        {
            this.pedido = pedido;
            VentanaPrincipal = argVentana;
            DisplayName = "Pedido de "+ pedido.puntoVenta.nombre;
        }

        public void BackButton()
        {
            if (Productos != null)
                Productos.Clear();
            VentanaPrincipal.ActivateItem(new ComprasNuevoViewModel(VentanaPrincipal));
        }

        public BindableCollection<ProductoModel> Productos
        {
            get => pedido.productos;
            set
            {
                pedido.productos = value;
                NotifyOfPropertyChange(() => Productos);
            }

        }


        public string Fecha
        {
            get { return pedido.fecha.ToShortDateString(); }
        }
        public string Codigo  
        {
            get { return pedido.codigo; }
        }

        public string ProductoVisibilidad { get; set; } = "Visible";

        public void Guardar()
        {
            ProductoVisibilidad = "Hidden";
            MessageBox.Show("Holaa");
        }


    }
}
