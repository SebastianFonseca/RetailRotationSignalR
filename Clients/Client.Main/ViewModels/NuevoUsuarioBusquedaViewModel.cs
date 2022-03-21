using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Client.Main.ViewModels
{
    class NuevoUsuarioBusquedaViewModel : PropertyChangedBase, IDataErrorInfo
    {


        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();

        public NuevoUsuarioBusquedaViewModel(MainWindowViewModel argVentana)
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

        private EmpleadoModel _usuarioSeleccionado;

        public EmpleadoModel UsuarioSeleccionado
        {
            get { return _usuarioSeleccionado; }
            set
            {

                //BusquedasVisibilidad = "Hidden";
                if (value != null)
                {
                    seleccion = value;
                    BuscarTbx = value.cedula + "-" + value.firstName + " " + value.lastName;
                }
                _usuarioSeleccionado = value;

                NotifyOfPropertyChange(() => UsuarioSeleccionado);
            }
        }
        EmpleadoModel seleccion = new EmpleadoModel();

        private BindableCollection<EmpleadoModel> _busquedas = new BindableCollection<EmpleadoModel>();

        public BindableCollection<EmpleadoModel> Busquedas
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
                MessageBox.Show("Escriba un nombre o número de cédula");
            }
            else
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    try
                    {
                        Task<object> re = conexion.CallServerMethod("ServidorGetUsuarios", Arguments: new[] { BuscarTbx.Split('-')[0].Trim() });
                        await re;
                        EmpleadoModel[] mn = System.Text.Json.JsonSerializer.Deserialize<EmpleadoModel[]>(re.Result.ToString());
                        BindableCollection<EmpleadoModel> resultado = new BindableCollection<EmpleadoModel>();
                        resultado.Clear();
                        foreach (EmpleadoModel item in mn)
                        {
                            resultado.Add(item);
                        }



                        // BindableCollection<EmpleadoModel> resultado = DbConnection.getEmpleados(BuscarTbx.Split('-')[0]);

                        if (resultado.Count == 0)
                        {
                            MessageBox.Show("Número de cédula, nombre o apellido no resgistrados");
                        }
                        else
                        {
                            IEnumerator<EmpleadoModel> e = resultado.GetEnumerator();
                            e.Reset();
                            while (e.MoveNext())
                            {
                                if (e.Current.cedula == BuscarTbx.Split('-')[0])
                                {
                                    VentanaPrincipal.ActivateItem(new NuevoUsuarioResultadoBusquedaViewModel(VentanaPrincipal, e.Current));
                                }


                            }

                            BusquedasVisibilidad = "Visible";
                            ComboboxDesplegado = "True";
                        }
                    }
                    catch (Exception e)
                    {

                        MessageBox.Show(e.Message);

                    }
                }
                else
                {
                    MessageBox.Show("No es posible realizar la busqueda si no esta conectado al servidor.");

                }




            }





        }

        public void BackButton()
        {

            VentanaPrincipal.ActivateItem(new GerenciaAdministrativoViewModel(VentanaPrincipal));
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
                    Task<object> re = conexion.CallServerMethod("ServidorGetUsuarios", Arguments: new[] { BuscarTbx });
                    await re;
                    EmpleadoModel[] mn = System.Text.Json.JsonSerializer.Deserialize<EmpleadoModel[]>(re.Result.ToString());
                    Busquedas.Clear();
                    foreach (EmpleadoModel item in mn)
                    {
                        Busquedas.Add(item);
                    }

                    //Busquedas = DbConnection.getEmpleados(BuscarTbx);
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
