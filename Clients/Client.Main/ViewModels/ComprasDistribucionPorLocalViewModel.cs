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
    public class ComprasDistribucionPorLocalViewModel :Conductor<IScreen>.Collection.OneActive
    {
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public BindableCollection<PedidoModel> pedidos = new BindableCollection<PedidoModel>();
        public  MainWindowViewModel VentanaPrincipal;
        public ComprasDistribucionPorLocalViewModel(BindableCollection<PedidoModel> pedidos, MainWindowViewModel VentanaPrincipal)
        {
            this.VentanaPrincipal = VentanaPrincipal;
            Items.Add(new CompraViewModel(VentanaPrincipal, pedidos));
            foreach (PedidoModel pedido in pedidos)
            {
                Items.Add(new ComprasPorLocalViewModel(VentanaPrincipal, pedido));
            }            
        }
    }
}
