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
        public ExistenciasNuevoViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
            getprod();
        }

        public BindableCollection<ProductoModel> _productos;
        public BindableCollection<ProductoModel> Productos
        {
            get
            {
                return _productos;
            }
            set
            {
                _productos = value;
                NotifyOfPropertyChange(() => Productos);

            }

        }


        public string Codigo
        {
            get return 
        }
        

        public string Fecha
        {
            get { return DateTime.Today.ToShortDateString(); }
        }

        public string Responsable
        {
            get { return VentanaPrincipal.Usuario; }
        }


        public async void getprod()
        {
            try
            {
                Task<object> re = conexion.CallServerMethod("ServidorgetIdProductos", Arguments: new object[] { });
                await re;
                Productos =  System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ProductoModel>>(re.Result.ToString());

            }
            catch (Exception e)
            {
            MessageBox.Show(e.Message);
            }

        }
             

    }
}
