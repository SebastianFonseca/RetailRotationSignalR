using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main.Views
{
    public class ExistenciasNuevoViewModel: Screen
    {
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();

        public ExistenciasModel existencias = new ExistenciasModel();
        public ExistenciasNuevoViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
            getprod();
        }

        public async void getprod()
        {
            try
            {
                Task<object> re = conexion.CallServerMethod("ServidorgetIdProductos", Arguments: new object[] { });
                await re;
                Productos = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ProductoModel>>(re.Result.ToString());

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        public BindableCollection<ProductoModel> Productos
        {
            get
            {
                return existencias.productos;
            }
            set
            {
                existencias.productos = value;
                NotifyOfPropertyChange(() => Productos);

            }

        }

        public string Codigo
        {
            get { return DateTime.Now.ToString("dd''MM''yyyy''HH''mm''ss") + VentanaPrincipal.usuario.puntoDeVenta.codigo;  }
            set
            {
                existencias.codigo = value;
                NotifyOfPropertyChange(() => Codigo);
            }
        }


        public DateTime Fecha
        {
            get { return DateTime.Today.Date; }
            set 
            { 
                existencias.fecha = value;
                NotifyOfPropertyChange(() => Fecha);

            }
        }

        public EmpleadoModel Responsable
        {
            get { return VentanaPrincipal.usuario; }
            set
            {
                existencias.responsable = value;
                NotifyOfPropertyChange(() => Responsable);

            }
        }

        public void BackButton()
        {
            Productos.Clear();
            VentanaPrincipal.ActivateItem(new AdministracionInventarioViewModel(VentanaPrincipal));
        }
        public void Guardar()
        {
           
        }

    }
}
