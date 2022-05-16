using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main.ViewModels
{
    public class PedidoBuscarViewModel : Screen
    {

        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();


        public PedidoBuscarViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
        }



        private bool _buscartbxb = true;

        public bool Buscartbxb
        {
            get => _buscartbxb;
            set
            {
                _buscartbxb = value;
                NotifyOfPropertyChange(() => Buscartbxb);
            }
        }



        private string _buscarTbx;
        public string BuscarTbx
        {
            get => _buscarTbx;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Buscartbxfechab = false;
                    if (!string.IsNullOrEmpty(BuscarTbxFecha))
                    {
                        Buscartbxb = true;
                    }
                }
                else
                {
                    Buscartbxfechab = true;

                }
                _buscarTbx = value;
                NotifyOfPropertyChange(() => BuscarTbx);
            }
        }


        private bool _buscartbxfechab = true;

        public bool Buscartbxfechab
        {
            get => _buscartbxfechab;
            set
            {
                _buscartbxfechab = value;
                NotifyOfPropertyChange(() => Buscartbxfechab);
            }
        }

        private string _buscarTbxFecha;

        public string BuscarTbxFecha
        {
            get => _buscarTbxFecha;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Buscartbxb = false;
                }
                else

                {
                    Buscartbxb = true;

                }

                _buscarTbxFecha = value;
                NotifyOfPropertyChange(() => BuscarTbxFecha);

            }
        }



        private BindableCollection<PedidoModel> _busquedas = new BindableCollection<PedidoModel>();
        public BindableCollection<PedidoModel> Busquedas
        {
            get => _busquedas;
            set
            {
                _busquedas = value;
                NotifyOfPropertyChange(() => Busquedas);
            }
        }


        PedidoModel Seleccionada;

        private PedidoModel _existenciaSeleccionada;
        public PedidoModel ExistenciaSeleccionada
        {
            get => _existenciaSeleccionada;
            set
            {
                if (value != null)
                {
                    Seleccionada = value;
                    BuscarTbx = value.codigo + " " + value.fecha.ToString("yyyy-MM-dd");
                    BusquedasVisibilidad = "Hidden";
                }
                _existenciaSeleccionada = value;

                NotifyOfPropertyChange(() => ExistenciaSeleccionada);
            }
        }

        public async void Buscar()
        {
            if (String.IsNullOrEmpty(BuscarTbx) && string.IsNullOrEmpty(BuscarTbxFecha))
            {
                MessageBox.Show("Rellene alguno de los campos.");
                return;
            }
            else
            {
                if (Seleccionada == null)
                {
                    MessageBox.Show("Seleccione alguna de las busquedas");
                    return;
                }
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    try
                    {
                        Task<object> re = conexion.CallServerMethod("ServidorgetProductoPedido", Arguments: new[] { Seleccionada.codigo });
                        await re;

                        Seleccionada.productos = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ProductoModel>>(re.Result.ToString());


                        VentanaPrincipal.ActivateItem(new PedidoResultadoBusquedaViewModel(VentanaPrincipal, Seleccionada));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
                else if (MainWindowViewModel.Status == "Trabajando localmente")
                {
                    Seleccionada.productos = DbConnection.getProductoPedido(Seleccionada.codigo);
                    VentanaPrincipal.ActivateItem(new PedidoResultadoBusquedaViewModel(VentanaPrincipal, Seleccionada));

                }

            }
        }

        public void BackButton()
        {
            if (Busquedas.Count != 0)
            {
                Busquedas.Clear();
            }

            VentanaPrincipal.ActivateItem(new AdministracionInventarioViewModel(VentanaPrincipal));
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
                if (String.IsNullOrEmpty(BuscarTbx) && string.IsNullOrEmpty(BuscarTbxFecha)) { return "Hidden"; }
                return _busquedasVisibiliad;
            }
            set
            {
                _busquedasVisibiliad = value;
                NotifyOfPropertyChange(() => BusquedasVisibilidad);
            }
        }
        public async void EscribiendoBusqueda()
        {
            if (string.IsNullOrEmpty(BuscarTbx) & string.IsNullOrEmpty(BuscarTbxFecha))
            {
                BusquedasVisibilidad = "Hidden";
            }
            Busquedas.Clear();
            if (!string.IsNullOrEmpty(BuscarTbx))
            {
                try
                {
                    if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                    {
                        //BindableCollection<PedidoModel> existencias = new BindableCollection<PedidoModel>();
                        Task<object> re = conexion.CallServerMethod("ServidorGetPedidos", Arguments: new[] { BuscarTbx });
                        await re;

                        Busquedas = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<PedidoModel>>(re.Result.ToString());

                    }
                    else if (MainWindowViewModel.Status == "Trabajando localmente")
                    {
                        Busquedas = DbConnection.getPedidos(BuscarTbx);
                    }

                    if (Busquedas.Count == 0)
                    {
                        BusquedasVisibilidad = "Hidden";
                    }
                    else
                    {
                        BusquedasVisibilidad = "Visible";
                    }

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            else
            {
                if (BuscarTbxFecha != null && BuscarTbxFecha.Length == 10)
                {
                    try
                    {
                        if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                        {
                            //BindableCollection<PedidoModel> existencias = new BindableCollection<PedidoModel>();
                            Task<object> re = conexion.CallServerMethod("ServidorGetPedidos", Arguments: new[] { BuscarTbxFecha });
                            await re;

                            Busquedas = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<PedidoModel>>(re.Result.ToString());

                        }
                        else if (MainWindowViewModel.Status == "Trabajando localmente")
                        {
                            Busquedas = DbConnection.getPedidos(BuscarTbxFecha);
                        }

                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
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
    }
}
