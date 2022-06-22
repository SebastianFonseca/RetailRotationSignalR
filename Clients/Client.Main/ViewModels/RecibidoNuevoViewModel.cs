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
    class RecibidoNuevoViewModel:Screen
    {
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();

        public RecibidoNuevoViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
            getEnvios();
        }

        public async void getEnvios()
        {
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    Task<object> re = conexion.CallServerMethod("ServidorgetTodosLosEnviosPorLocal", Arguments: new object[] { VentanaPrincipal.usuario.cedula });
                    await re;
                    Envios = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<EnvioModel>>(re.Result.ToString());
                }
                if (MainWindowViewModel.Status == "Trabajando localmente")
                {
                    Envios = DbConnection.getTodosLosEnviosPorLocal(VentanaPrincipal.usuario.cedula);
                }
                if (Envios.Count == 0)
                {
                    Texto = "Ya se ha creado un recibido para todos los envios";
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private string _texto;

        public string Texto
        {
            get { return _texto; }
            set 
            { 
                _texto = value;
                NotifyOfPropertyChange(() => Texto);
            }
        }


        private BindableCollection<EnvioModel> _envios;

        public BindableCollection<EnvioModel> Envios
        {
            get { return _envios; }
            set
            {
                _envios = value;
                NotifyOfPropertyChange(() => Envios);

            }
        }

        private EnvioModel _envio;

        public EnvioModel Envio
        {
            get { return _envio; }
            set
            {
                _envio = value;
                NotifyOfPropertyChange(() => Envio);

            }
        }

        public void BackButton()
        {
            Envios.Clear();
            VentanaPrincipal.ActivateItem(new AdministracionInventarioViewModel(VentanaPrincipal));
        }

        public async void ItemSeleccionado()
        {

            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    Task<object> re = conexion.CallServerMethod("ServidorgetEnvioConProductos", Arguments: new object[] { Envio.codigo });
                    await re;
                    BindableCollection<EnvioModel> seleccionada = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<EnvioModel>>(re.Result.ToString());
                    VentanaPrincipal.ActivateItem(new RecibidoEditarViewModel(VentanaPrincipal, seleccionada[0]));
                }
                if (MainWindowViewModel.Status == "Trabajando localmente")
                {
                    VentanaPrincipal.ActivateItem(new RecibidoEditarViewModel(VentanaPrincipal, DbConnection.getEnvioConProductos(Envio.codigo)[0]));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

            }

        }
    }
}
