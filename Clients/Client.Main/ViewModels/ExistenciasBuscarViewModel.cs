using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main.ViewModels
{
    public class ExistenciasBuscarViewModel: Screen
    {

        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public ExistenciasBuscarViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;

        }

        private string _buscarTbx;
        public string BuscarTbx
        {
            get => _buscarTbx;
            set
            {
                if (!string.IsNullOrEmpty(BuscarTbxFecha))
                {
                    return;
                }
                _buscarTbx = value;
                NotifyOfPropertyChange(() => BuscarTbx);
            }
        }

        private string _buscarTbxFecha;

        public string BuscarTbxFecha
        {
            get { return _buscarTbxFecha; }
            set 
            {
                if (!string.IsNullOrEmpty(BuscarTbx))
                {
                    return;
                }
                _buscarTbxFecha = value;
                NotifyOfPropertyChange(() => BuscarTbxFecha);

            }
        }



        private BindableCollection<ExistenciasModel> _busquedas = new BindableCollection<ExistenciasModel>();
        public BindableCollection<ExistenciasModel> Busquedas
        {
            get => _busquedas;
            set
            {
                _busquedas = value;
                NotifyOfPropertyChange(() => Busquedas);
            }
        }


        ExistenciasModel Seleccionada;

        private ExistenciasModel _existenciaSeleccionada;
        public ExistenciasModel ExistenciaSeleccionada
        {
            get => _existenciaSeleccionada;
            set
            {
                if (value != null)
                {
                    Seleccionada = value;
                    BuscarTbx = value.codigo + "-" + value.fecha ;
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

                            VentanaPrincipal.ActivateItem(new ExistenciaResultadoBusquedaViewModel(VentanaPrincipal, Seleccionada));

                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
                else
                {
//                    MessageBox.Show("No es posible realizar la busqueda si no esta conectado al servidor.");
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
            Busquedas.Clear();
            if (!string.IsNullOrEmpty(BuscarTbx))
            {
                try
                {
                    if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                    {
                        BindableCollection<ExistenciasModel> existencias = new BindableCollection<ExistenciasModel>();
                        Task<object> re = conexion.CallServerMethod("servidorGetExistencias", Arguments: new[] { BuscarTbx });
                        await re;

                        Busquedas = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ExistenciasModel>>(re.Result.ToString());

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
            else { 
            if (BuscarTbxFecha.Length == 10)
            {
                try
                {
                    if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                    {
                        BindableCollection<ExistenciasModel> existencias = new BindableCollection<ExistenciasModel>();
                        Task<object> re = conexion.CallServerMethod("servidorGetExistencias", Arguments: new[] { BuscarTbxFecha });
                        await re;

                        Busquedas = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ExistenciasModel>>(re.Result.ToString());

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








    }
}
