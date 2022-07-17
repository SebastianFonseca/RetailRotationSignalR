using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main.ViewModels
{
    public class InformesBuscarClientesViewModel : Screen
    {

        public MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public InformesBuscarClientesViewModel(MainWindowViewModel ventanaPrincipal)
        {
            this.VentanaPrincipal = ventanaPrincipal;
            getPromedio();
        }

        public async void getPromedio()
        {
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {

                    Task<object> re = conexion.CallServerMethod("ServidorpromediofacturasRegistradas", Arguments: new object[] { null });
                    await re;

                    decimal?[] respuesta = System.Text.Json.JsonSerializer.Deserialize<decimal?[]>(re.Result.ToString());
                    Promedio = 100 - respuesta[0];
                    Facturas = respuesta[2];
                    Clientes = respuesta[1];
                }
                if (MainWindowViewModel.Status == "Trabajando localmente")
                {
                    MessageBox.Show("No esta conectado al servidor");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public decimal? Promedio { get; set; }
        public decimal? Facturas { get; set; }
        public decimal? Clientes { get; set; }


        private string _buscarTbx;
        public string BuscarTbx
        {
            get => _buscarTbx;
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
            get => _busquedas;
            set
            {
                _busquedas = value;
                NotifyOfPropertyChange(() => Busquedas);
            }
        }


        private ClientesModel _clienteSeleccionado;
        public ClientesModel ClienteSeleccionado
        {
            get => _clienteSeleccionado;
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

        public void BackButton()
        {
            if (Busquedas.Count != 0)
            {
                Busquedas.Clear();
            }

            VentanaPrincipal.ActivateItem(new InformesViewModel(VentanaPrincipal));
        }


        public void Buscar()
        {
            if (String.IsNullOrEmpty(BuscarTbx))
            {
                MessageBox.Show("Escriba un nombre o número de cédula");
            }
            else
            {
                VentanaPrincipal.ActivateItem(new InformesBuscarClientesResultadoViewModel(VentanaPrincipal, ClienteSeleccionado));
            }
        }

        public string Error => null;
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

                    if (Busquedas.Count == 0)
                    {
                        BusquedasVisibilidad = "Hidden";
                    }
                    else
                    {
                        BusquedasVisibilidad = "Visible";
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
