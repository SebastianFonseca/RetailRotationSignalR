using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using Caliburn.Micro;
using Client.Main.Models;
using System.Windows;
using System.Threading.Tasks;

namespace Client.Main.ViewModels
{
    class InformesBuscarLocalesViewModel: Screen
    {

        public MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public InformesBuscarLocalesViewModel(MainWindowViewModel ventanaPrincipal)
        {
            this.VentanaPrincipal = ventanaPrincipal;
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

        private LocalModel _localSeleccionado;

        public LocalModel UsuarioSeleccionado
        {
            get { return _localSeleccionado; }
            set
            {

                if (value != null)
                {
                    seleccion = value;
                    BuscarTbx = value.codigo + " - " + value.nombre;
                }
                _localSeleccionado = value;

                NotifyOfPropertyChange(() => UsuarioSeleccionado);
            }
        }
        LocalModel seleccion = new LocalModel();

        private BindableCollection<LocalModel> _busquedas = new BindableCollection<LocalModel>();

        public BindableCollection<LocalModel> Busquedas
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




        public async void Buscar()
        {
            if (String.IsNullOrEmpty(BuscarTbx))
            {
                MessageBox.Show("Escriba un nombre o un código de local.");
            }
            else
            {
                try
                {
                    if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                    {

                        Task<object> re = conexion.CallServerMethod("ServidorGetLocales", Arguments: new[] { BuscarTbx.Split('-')[0].Trim() });
                        await re;
                        LocalModel[] mn = System.Text.Json.JsonSerializer.Deserialize<LocalModel[]>(re.Result.ToString());
                        BindableCollection<LocalModel> resultado = new BindableCollection<LocalModel>();
                        resultado.Clear();
                        foreach (LocalModel item in mn)
                        {
                            resultado.Add(item);
                        }

                        if (resultado.Count == 0)
                        {
                            MessageBox.Show("Nombre o código no registrados");
                        }
                        else
                        {
                            IEnumerator<LocalModel> e = resultado.GetEnumerator();
                            e.Reset();
                            while (e.MoveNext())
                            {
                                if (e.Current.codigo == BuscarTbx.Split('-')[0].Trim())
                                {
                                    VentanaPrincipal.ActivateItem(new LocalResultadoBusquedaViewModel(VentanaPrincipal, e.Current));
                                }


                            }

                            BusquedasVisibilidad = "Visible";
                            ComboboxDesplegado = "True";
                        }


                    }
                    else
                    {
                        MessageBox.Show("Para realizar una busqueda de locales debe estar conectado al servidor.");
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }

            }


        }

        public void BackButton()
        {

            VentanaPrincipal.ActivateItem(new InformesViewModel(VentanaPrincipal));
            Busquedas.Clear();
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
                            result = "Rellene este campo.";
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


        private string _comboboxDesplegado = "false";

        public string ComboboxDesplegado
        {
            get { return _comboboxDesplegado; }
            set
            {
                _comboboxDesplegado = value;
                NotifyOfPropertyChange(() => ComboboxDesplegado);
            }
        }



        public async void EscribiendoBusqueda()
        {
            try
            {

                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {

                    Task<object> re = conexion.CallServerMethod("ServidorGetLocales", Arguments: new[] { BuscarTbx });
                    await re;
                    LocalModel[] mn = System.Text.Json.JsonSerializer.Deserialize<LocalModel[]>(re.Result.ToString());
                    Busquedas.Clear();
                    foreach (LocalModel item in mn)
                    {
                        Busquedas.Add(item);
                    }
                    if (Busquedas == null || Busquedas.Count == 0)
                    {
                        ComboboxDesplegado = "false";
                        BusquedasVisibilidad = "Hidden";
                    }
                    else
                    {
                        BusquedasVisibilidad = "Visible";
                        ComboboxDesplegado = "true";

                    }
                }
                else
                {
                    MessageBox.Show("No es posible realizar la busqueda si no esta conectado al servidor.");
                }
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message);
            }





        }

    }
}
