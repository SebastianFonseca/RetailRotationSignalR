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
        public EnvioModel envio = new EnvioModel();

        public ComprasPorLocalViewModel(MainWindowViewModel argVentana, PedidoModel pedido)
        {
            this.pedido = pedido;
            envio = pedido;
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
            get => envio.productos;
            set
            {
                envio.productos = value;
                NotifyOfPropertyChange(() => Productos);
            }

        }


        public string Fecha
        {
            get { return envio.fecha.ToShortDateString(); }
        }
        public string Codigo  
        {
            get { return pedido.codigo; }
        }

        private string _placasN;

        public string PlacasN
        {
            get { return _placasN; }
            set
            { 
                _placasN = value;
                NotifyOfPropertyChange(() => PlacasN);
            }
        }

        private string _placasL;

        public string PlacasL
        {
            get { return _placasL; }
            set 
            {
                _placasL = value.ToUpper();
                NotifyOfPropertyChange(() => PlacasL);
            }
        }
        private string _conductor;

        public string Conductor
        {
            get { return _conductor; }
            set
            {
                Statics.PrimeraAMayuscula(value);
                _conductor = value;
                envio.nombreConductor = value;
                NotifyOfPropertyChange(() => Conductor);
            }
        }





        public void Guardar()
        {
            envio.placasCarro = PlacasL + "-" + PlacasN;
            DbConnection.NuevoEnvioBool(envio);
        }


    }
}
