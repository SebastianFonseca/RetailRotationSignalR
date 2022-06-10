using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main.ViewModels
{
    public class PedidoNuevoViewModel : Screen
    {
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();

        public PedidoNuevoViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
            getExistencias();
        }

        public async void getExistencias() 
        {
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    Task<object> re = conexion.CallServerMethod("servidorGetTodasLasExistencias", Arguments: new object[] { });
                    await re;
                    Existencias = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ExistenciasModel>>(re.Result.ToString());
                }
                if (MainWindowViewModel.Status == "Trabajando localmente")
                {
                    Existencias = DbConnection.getTodasLasExistencias() ;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        private BindableCollection<ExistenciasModel> existenciasModels;

        public BindableCollection<ExistenciasModel> Existencias
        {
            get { return existenciasModels; }
            set
            { 
                existenciasModels = value;
                NotifyOfPropertyChange(() => Existencias);

            }
        }

        private ExistenciasModel _existencia;

        public ExistenciasModel Existencia
        {
            get { return _existencia; }
            set
            {
                _existencia = value;
                NotifyOfPropertyChange(() => Existencia);

            }
        }

        public void BackButton()
        {
            Existencias.Clear();
            VentanaPrincipal.ActivateItem(new AdministracionInventarioViewModel(VentanaPrincipal));
        }

        public async void ItemSeleccionado()
        {

            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    Task<object> re = conexion.CallServerMethod("servidorGetExistenciasConProductos", Arguments: new object[] { Existencia.codigo });
                    await re;
                    BindableCollection<ExistenciasModel> seleccionada = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ExistenciasModel>>(re.Result.ToString());
                    VentanaPrincipal.ActivateItem(new PedidoEditarViewModel(VentanaPrincipal, seleccionada[0]));
                }
                if (MainWindowViewModel.Status == "Trabajando localmente")
                {
                    VentanaPrincipal.ActivateItem(new PedidoEditarViewModel(VentanaPrincipal, DbConnection.getExistenciasConProductos(Existencia.codigo)[0]));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }





        }




    }
}
