using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace Client.Main.ViewModels
{

    class AddClientBuscarViewModel : PropertyChangedBase, IDataErrorInfo
    {
        MainWindowViewModel VentanaPrincipal;
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

        private ClientesModel _usuarioSeleccionado;

        public ClientesModel UsuarioSeleccionado
        {
            get { return _usuarioSeleccionado; }
            set
            {

                if (value != null)
                {
                    seleccion = value;
                    BuscarTbx = value.Cedula + "-" + value.FirstName + " " + value.LastName;
                }
                _usuarioSeleccionado = value;

                NotifyOfPropertyChange(() => UsuarioSeleccionado);
            }
        }
        ClientesModel seleccion = new ClientesModel();

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




        public void Buscar()
        {
            if (String.IsNullOrEmpty(BuscarTbx))
            {
                MessageBox.Show("Escriba un nombre o número de cédula");
            }
            else
            {
                BindableCollection<ClientesModel> resultado = DbConnection.getClientes(BuscarTbx.Split('-')[0]);

                if (resultado.Count == 0)
                {
                    MessageBox.Show("Número de cédula, nombre o apellido no resgistrados");
                }
                else
                {
                    IEnumerator<ClientesModel> e = resultado.GetEnumerator();
                    e.Reset();
                    while (e.MoveNext())
                    {
                        if (e.Current.Cedula == BuscarTbx.Split('-')[0])
                        {
                            VentanaPrincipal.ActivateItem(new AddClientResultadoBusquedaViewModel(VentanaPrincipal, e.Current));
                        }


                    }

                    BusquedasVisibilidad = "Visible";
                    ComboboxDesplegado = "True";
                }

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



        public void EscribiendoBusqueda()
        {

            Busquedas = DbConnection.getClientes(BuscarTbx);
            if (Busquedas == null || Busquedas.Count == 0)
            {
                BusquedasVisibilidad = "Hidden";
            }
            else
            {
                BusquedasVisibilidad = "Visible";
                ComboboxDesplegado = "true";

            }

        }


    }
}
