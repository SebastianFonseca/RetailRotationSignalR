using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Main.ViewModels
{
    public class CompraResultadoBusquedaViewModel:Screen
    {
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public ComprasModel compra;


        public CompraResultadoBusquedaViewModel(MainWindowViewModel argVentana, ComprasModel pCompra)
        {
            VentanaPrincipal = argVentana;
            compra = pCompra;

        }

        public BindableCollection<ProductoModel> Productos
        {
            get
            {
                if (compra.productos != null)
                {
                    foreach (ProductoModel producto in compra.productos)
                    {
                        producto.unidadCompra = producto.unidadCompra.Substring(0, 3);

                    }
                }


                return compra.productos;
            }
            set
            {

                compra.productos = value;
                NotifyOfPropertyChange(() => Productos);
            }

        }

        public string Codigo
        {
            get => compra.codigo;
            set
            {
                compra.codigo = value;
                NotifyOfPropertyChange(() => Codigo);
            }
        }

        private string _dia = DateTime.Today.Day.ToString();

        public string Dia
        {

            get { return compra.fecha.Day.ToString(); }
            set
            {
                short a;
                if (Int16.TryParse(value, out a))
                {
                    if (Int16.Parse(value) < DateTime.Now.Day)
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
            get { return compra.fecha.Month.ToString(); }
            set
            {
                short a;
                if (Int16.TryParse(value, out a))
                {
                    if (Int16.Parse(value) <= DateTime.Now.Month)
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
            get { return compra.fecha.Year.ToString(); }
            set
            {
                short a;
                if (Int16.TryParse(value, out a))
                {
                    if (Int16.Parse(value) < DateTime.Now.Year)
                    {
                        _año = value;
                        NotifyOfPropertyChange(() => Año);

                    }
                }

            }
        }

        public string Fecha
        {
            get { return compra.fecha.ToString("dd/MM/yyyy");  }
        }

        public string Responsable
        {
            get => compra.responsable.cedula.ToString();

        }

        public void BackButton()
        {
            Productos.Clear();
            VentanaPrincipal.ActivateItem(new ListadoCompraViewModel(VentanaPrincipal));
        }
         public void Editar()
        {
            VentanaPrincipal.ActivateItem(new CompraViewModel(VentanaPrincipal, compra));
        }


    }
}
