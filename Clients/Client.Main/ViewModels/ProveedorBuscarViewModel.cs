using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main.ViewModels
{
    class ProveedorBuscarViewModel : PropertyChangedBase, IDataErrorInfo
    {
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public ProveedorBuscarViewModel(MainWindowViewModel argVentana)
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

        private ProveedorModel _usuarioSeleccionado;

        public ProveedorModel UsuarioSeleccionado
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
        ProveedorModel seleccion = new ProveedorModel();

        private BindableCollection<ProveedorModel> _busquedas = new BindableCollection<ProveedorModel>();

        public BindableCollection<ProveedorModel> Busquedas
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

        private BindableCollection<ProveedorModel> _busquedasProducto = new BindableCollection<ProveedorModel>();

        public BindableCollection<ProveedorModel> BusquedasProducto
        {
            get { return _busquedasProducto; }
            set
            {
                _busquedasProducto = value;
                NotifyOfPropertyChange(() => BusquedasProducto);
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
                        Task<object> re = conexion.CallServerMethod("ServidorGetProveedores", Arguments: new[] { BuscarTbx.Split('-')[0].Trim() });
                        await re;
                        ProveedorModel[] mn = System.Text.Json.JsonSerializer.Deserialize<ProveedorModel[]>(re.Result.ToString());
                        BindableCollection<ProveedorModel> resultado = new BindableCollection<ProveedorModel>();
                        resultado.Clear();
                        foreach (ProveedorModel item in mn)
                        {
                            resultado.Add(item);
                        }



                        // BindableCollection<ProveedorModel> resultado = DbConnection.getEmpleados(BuscarTbx.Split('-')[0]);

                        if (resultado.Count == 0)
                        {
                            MessageBox.Show("Número de cédula, nombre o apellido no resgistrados");
                        }
                        else
                        {
                            IEnumerator<ProveedorModel> e = resultado.GetEnumerator();
                            e.Reset();
                            while (e.MoveNext())
                            {
                                if (e.Current.cedula == BuscarTbx.Split('-')[0])
                                {
                                    VentanaPrincipal.ActivateItem(new ProveedorResultadoBusquedaViewModel(VentanaPrincipal, e.Current));
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
            if(Busquedas.Count != 0) Busquedas.Clear();
            if(BusquedasProducto.Count != 0) BusquedasProducto.Clear();
            VentanaPrincipal.ActivateItem(new DC_AdministrativoViewModel(VentanaPrincipal));

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
                    BindableCollection<ProveedorModel> proveedores = new BindableCollection<ProveedorModel>();
                    Task<object> re = conexion.CallServerMethod("ServidorGetProveedores", Arguments: new[] { BuscarTbx });
                    await re;
                    //ProveedorModel[] mn = System.Text.Json.JsonSerializer.Deserialize<ProveedorModel[]>(re.Result.ToString());
                    
                     proveedores = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ProveedorModel>>(re.Result.ToString());

                    if (proveedores.Count != 0)
                    {
                        Busquedas.Clear();
                        BusquedasProducto.Clear();
                        if (proveedores[proveedores.Count-1].ciudad == "separador")
                        {
                            proveedores.RemoveAt(proveedores.Count - 1);
                            Busquedas = proveedores;

                        }
                        else if (proveedores[0].ciudad == "separador")
                        {
                            proveedores.RemoveAt(0);
                            BusquedasProducto = proveedores;
                        }
                        else
                        {
                            for (int i = 0; i < proveedores.Count; i++)
                            {
                                if (proveedores[i+1].ciudad == "separador")
                                {
                                    proveedores.RemoveAt(i+1);
                                    break;
                                }
                                Busquedas.Add(proveedores[i]);
                                proveedores.Remove(proveedores[i]);

                            }
                            BusquedasProducto = proveedores;
                        }
                    }




                    
                    //foreach (ProveedorModel item in mn)
                    //{
                    //    Busquedas.Add(item);
                    //}

                    //Busquedas = DbConnection.getEmpleados(BuscarTbx);
                    if ( Busquedas.Count == 0 & BusquedasProducto.Count == 0)
                    {
                       // ComboboxDesplegado = "false";
                        BusquedasVisibilidad = "Hidden";
                    }
                    else
                    {
                        BusquedasVisibilidad = "Visible";
                      //  ComboboxDesplegado = "true";

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
