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

    public class AddClientBuscarViewModel : PropertyChangedBase, IDataErrorInfo
    {
        public MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();

        public AddClientBuscarViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
        }

        private string _buscarTbx;
        public string BuscarTbx
        {
            get { return _buscarTbx; }
            set
            {
                if (value == null)
                {
                    MessageBox.Show("Buscar null");
                }
                _buscarTbx = value;
                NotifyOfPropertyChange(() => BuscarTbx);
            }
        }



        private BindableCollection<ClientesModel> _busquedas = new BindableCollection<ClientesModel>();
        public BindableCollection<ClientesModel> Busquedas
        {
            get
            {
                return _busquedas;
            }
            set
            {
                _busquedas = value;
                NotifyOfPropertyChange(() => Busquedas);
            }
        }


        private ClientesModel _clienteSeleccionado;
        public ClientesModel ClienteSeleccionado
        {
            get { return _clienteSeleccionado; }
            set
            {
                if (value != null)
                {
                 
                    BuscarTbx = value.cedula + " - " + value.firstName + " " + value.lastName;
                    _clienteSeleccionado = value;
                }


                NotifyOfPropertyChange(() => ClienteSeleccionado);
            }
        }

     
        public void Buscar()
        {
            if (String.IsNullOrEmpty(BuscarTbx))
            {
                MessageBox.Show("Escriba un nombre o número de cédula");
            }
            else
            {
                VentanaPrincipal.ActivateItem(new AddClientResultadoBusquedaViewModel(VentanaPrincipal, ClienteSeleccionado));      
            }
        }

        public string Error { get { return null; } }
        int flag = 0;
        public string this[string name]
        {
            get
            {
                string result = null;
                if (flag == 2)
                {
                    if (name == "BuscarTbx")
                    {
                        if (String.IsNullOrEmpty(BuscarTbx))
                        {
                            result = "Escriba algun valor.";
                        }
                    }
                }
                else { flag += 1; }
                return result;
            }
        }

        private string _busquedasVisibiliad = "Hidden";
        public string BusquedasVisibilidad
        {
            get
            {
                if (String.IsNullOrEmpty(BuscarTbx)) { return "Hidden"; }
                return _busquedasVisibiliad;
            }
            set { _busquedasVisibiliad = value; NotifyOfPropertyChange(() => BusquedasVisibilidad); }
        }
        public async void EscribiendoBusqueda()
        {
            Busquedas.Clear();
  
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    Task<object> re = conexion.CallServerMethod("ServidorgetClientes", Arguments: new[] { BuscarTbx });
                    await re;
                    Busquedas = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ClientesModel>>(re.Result.ToString());

                    if (Busquedas.Count == 0 )
                    {
                        BusquedasVisibilidad = "Hidden";
                    }
                    else
                    {
                        BusquedasVisibilidad = "Visible";
                    }
                }
                else
                {

                    if (MainWindowViewModel.Status == "Trabajando localmente")
                    {

                        Busquedas = DbConnection.getClientes(BuscarTbx);
                        if (Busquedas.Count == 0 )
                        {
                            BusquedasVisibilidad = "Hidden";
                        }
                        else
                        {
                            BusquedasVisibilidad = "Visible";
                        }
                    }


                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


    }
}
