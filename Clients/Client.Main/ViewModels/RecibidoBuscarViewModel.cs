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
    public class RecibidoBuscarViewModel : Screen
    {
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public RecibidoBuscarViewModel(MainWindowViewModel argVentana)
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



        private BindableCollection<RecibidoModel> _busquedas = new BindableCollection<RecibidoModel>();
        public BindableCollection<RecibidoModel> Busquedas
        {
            get => _busquedas;
            set
            {
                _busquedas = value;
                NotifyOfPropertyChange(() => Busquedas);
            }
        }


        RecibidoModel Seleccionado;

        private RecibidoModel _recibidoSeleccionado;
        public RecibidoModel RecibidoSeleccionado
        {
            get => _recibidoSeleccionado;
            set
            {
                if (value != null)
                {
                    Seleccionado = value;
                    BuscarTbx = value.codigo + " " + value.fecha.ToString("yyyy-MM-dd");
                    BusquedasVisibilidad = "Hidden";
                }
                _recibidoSeleccionado = value;

                NotifyOfPropertyChange(() => RecibidoSeleccionado);
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
                if (Seleccionado == null)
                {
                    MessageBox.Show("Seleccione alguna de las busquedas");
                    return;
                }
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    try
                    {
                        Task<object> re = conexion.CallServerMethod("ServidorgetProductosRecibido", Arguments: new[] { Seleccionado.codigo });
                        await re;

                        Seleccionado.productos = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ProductoModel>>(re.Result.ToString());


                        VentanaPrincipal.ActivateItem(new RecibidoResultadoBusquedaViewModel(VentanaPrincipal, Seleccionado));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
                else if (MainWindowViewModel.Status == "Trabajando localmente")
                {
                    Seleccionado.productos = DbConnection.getProductosRecibido(Seleccionado.codigo);
                    VentanaPrincipal.ActivateItem(new RecibidoResultadoBusquedaViewModel(VentanaPrincipal, Seleccionado));

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
                        Task<object> re = conexion.CallServerMethod("ServidorgetRecibidos", Arguments: new[] { BuscarTbx, VentanaPrincipal.usuario.puntoDeVenta.codigo });
                        await re;

                        Busquedas = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<RecibidoModel>>(re.Result.ToString());

                    }
                    else if (MainWindowViewModel.Status == "Trabajando localmente")
                    {
                        Busquedas = DbConnection.getRecibidos(BuscarTbx, VentanaPrincipal.usuario.puntoDeVenta.codigo);
                    }

                    if (Busquedas != null & Busquedas.Count == 0)
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
                            Task<object> re = conexion.CallServerMethod("ServidorgetRecibidos", Arguments: new[] { BuscarTbxFecha, VentanaPrincipal.usuario.puntoDeVenta.codigo });
                            await re;

                            Busquedas = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<RecibidoModel>>(re.Result.ToString());

                        }
                        else if (MainWindowViewModel.Status == "Trabajando localmente")
                        {
                            Busquedas = DbConnection.getRecibidos(BuscarTbxFecha, VentanaPrincipal.usuario.puntoDeVenta.codigo);
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
                if (Busquedas != null & Busquedas.Count == 0)
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
