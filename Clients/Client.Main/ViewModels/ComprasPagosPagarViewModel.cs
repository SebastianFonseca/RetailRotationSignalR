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
    public class ComprasPagosPagarViewModel : Screen
    {
        
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public BindableCollection<PedidoModel> pedidosSeleccionados = new BindableCollection<PedidoModel>();
        public ProductoModel producto;
        public ComprasPagosPagarViewModel(ProductoModel producto)
        {
            this.producto = producto;
            if(producto.compra != null && producto.precioCompra!= null)
                 Valor = (decimal)(producto.compra * producto.precioCompra);
        }

        private string _codigoSoporte;

        public string CodigoSoporte
        {
            get { return producto.soportePago; }
            set 
            {
                producto.soportePago = value;
                _codigoSoporte = value;
                NotifyOfPropertyChange(() => CodigoSoporte);
            }
        }

        private decimal _valor;

        public decimal Valor
        {
            get { return _valor; }
            set
            { 
                _valor = value;
                NotifyOfPropertyChange(() => Valor);
            }
        }

        public void Guardar()
        {
            this.TryClose();
        }


    }
}
