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
    public class ComprasPorLocalResultadoBusquedaViewModel : Screen
    {
        public MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public PedidoModel pedido;
        public EnvioModel envio = new EnvioModel();

        public ComprasPorLocalResultadoBusquedaViewModel(MainWindowViewModel argVentana, EnvioModel envio)
        {
            this.envio = envio;            
            VentanaPrincipal = argVentana;
            DisplayName = "Pedido de " + envio.puntoVenta.nombre;
        }

        public BindableCollection<ProductoModel> Productos
        {
            get => envio.productos;
            set
            {
                envio.productos = value;
                NotifyOfPropertyChange(() => Productos);
            }

        }


        public string Fecha
        {
            get { return envio.fechaEnvio.ToShortDateString(); }
        }
        public string Codigo
        {
            get { return envio.codigo; }
        }


        public string Placas
        {
            get { return envio.placasCarro; }

        }

        public string Conductor
        {
            get { return envio.nombreConductor; }

        }

        public void Editar()
        {
            VentanaPrincipal.ActivateItem(new ComprasPorLocalViewModel(VentanaPrincipal, envio));
        }


    }
}
