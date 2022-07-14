using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Main.ViewModels
{
    public class ExistenciaResultadoBusquedaViewModel:Screen
    {
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public ExistenciasModel Existencia;
        public ExistenciaResultadoBusquedaViewModel(MainWindowViewModel argVentana, ExistenciasModel pExistencia)
        {
            VentanaPrincipal = argVentana;
            Existencia = pExistencia;

        }

        public BindableCollection<ProductoModel> Productos
        {
            get => Existencia.productos;
            set
            {
                Existencia.productos = value;
                NotifyOfPropertyChange(() => Productos);
            }

        }

        public string Codigo
        {
            get => Existencia.codigo;
            set
            {
                Existencia.codigo = value;
                NotifyOfPropertyChange(() => Codigo);
            }
        }

        private string _dia = DateTime.Today.Day.ToString();

        public string Dia
        {

            get { return Existencia.fecha.Day.ToString(); }
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
            get { return Existencia.fecha.Month.ToString(); }
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
            get { return Existencia.fecha.Year.ToString(); }
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
            get => Existencia.responsable.cedula.ToString();

        }

        public void BackButton()
        {
            Productos.Clear();
            VentanaPrincipal.ActivateItem(new ExistenciasBuscarViewModel(VentanaPrincipal));
        }






    }
}
