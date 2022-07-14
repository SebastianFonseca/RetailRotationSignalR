using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Main.ViewModels
{
    public class PedidoResultadoBusquedaViewModel: Screen
    {
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public PedidoModel pedido;


        public PedidoResultadoBusquedaViewModel(MainWindowViewModel argVentana, PedidoModel pPedido)
        {
            VentanaPrincipal = argVentana;
            pedido = pPedido;
            
        }



        public BindableCollection<ProductoModel> Productos
        {
            get
            {
                foreach (ProductoModel producto in pedido.productos)
                {
                    producto.unidadVenta =  producto.unidadVenta.Substring(0, 3) ;
                }

                return pedido.productos; 
            }
            set
            {

                pedido.productos = value;
                NotifyOfPropertyChange(() => Productos);
            }

        }

        public string Codigo
        {
            get => pedido.codigo;
            set
            {
                pedido.codigo = value;
                NotifyOfPropertyChange(() => Codigo);
            }
        }

        private string _dia = DateTime.Today.Day.ToString();

        public string Dia
        {

            get { return pedido.fecha.Day.ToString(); }
            set
            {
                short a;
                if (Int16.TryParse(value, out a))
                {
                    if (Int32.Parse(value) < DateTime.Now.Day)
                    {
                        _dia = value;
                        NotifyOfPropertyChange(() => Dia);
                    }


                }
            }
        }

        private string _mes = DateTime.Today.Month.ToString();

        public string Mes
        {
            get { return pedido.fecha.Month.ToString(); }
            set
            {
                short a;
                if (Int16.TryParse(value, out a))
                {
                    if (Int32.Parse(value) <= DateTime.Now.Month)
                    {
                        _mes = value;
                        NotifyOfPropertyChange(() => Mes);
                    }
                }
            }
        }
        private string _año = DateTime.Today.Year.ToString();



        public string Año
        {
            get { return pedido.fecha.Year.ToString(); }
            set
            {
                short a;
                if (Int16.TryParse(value, out a))
                {
                    if (Int32.Parse(value) < DateTime.Now.Year)
                    {
                        _año = value;
                        NotifyOfPropertyChange(() => Año);

                    }
                }

            }
        }

        public string Responsable
        {
            get => pedido.responsable.cedula.ToString();

        }

        public void BackButton()
        {
            Productos.Clear();
            VentanaPrincipal.ActivateItem(new PedidoBuscarViewModel(VentanaPrincipal));
        }



    }
}
