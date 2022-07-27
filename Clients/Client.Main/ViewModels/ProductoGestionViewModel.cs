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
    class ProductoGestionViewModel : PropertyChangedBase, IDataErrorInfo
    {
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();

        public ProductoGestionViewModel(MainWindowViewModel argVentana)
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

        private BindableCollection<ProductoModel> _busquedas = new BindableCollection<ProductoModel>();
        public BindableCollection<ProductoModel> Busquedas
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



        ProductoModel seleccion = new ProductoModel();

        private ProductoModel _selectedBusquedas = new ProductoModel();
        public ProductoModel SelectedBusquedas
        {
            get { return _selectedBusquedas; }
            set
            {

                if (value != null)
                {
                    seleccion = value;
                    BuscarTbx = value.codigoProducto + "-" + value.nombre;
                }
                _selectedBusquedas = value;

                NotifyOfPropertyChange(() => SelectedBusquedas);
            }
        }


        public void AgregarProducto()
        {
            VentanaPrincipal.ActivateItem(new ProductoNuevoViewModel(VentanaPrincipal));
        }

        public  void Buscar()
        {
            if (String.IsNullOrEmpty( BuscarTbx))
            {
                MessageBox.Show("Escriba algun codigo o nombre de producto.");
            }
            else if (seleccion.codigoProducto == null)
            {
                MessageBox.Show("No se ha encontrado ningun producto.");
            }
            else
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {                    
                    VentanaPrincipal.ActivateItem(new ProductoResultadoBusquedaViewModel(VentanaPrincipal, seleccion));
                }
                else
                {
                    MessageBox.Show("No es posible realizar la busqueda si no esta conectado al servidor.");
                }
            }
        }

        public void BackButton()
        {
            if (Busquedas.Count != 0) Busquedas.Clear();
            VentanaPrincipal.ActivateItem(new GerenciaAdministrativoViewModel(VentanaPrincipal));
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


        public void SelectionChanged()
        {
            BusquedasVisibilidad = "Hidden";
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
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    Task<object> re = conexion.CallServerMethod("ServidorGetProductos", Arguments: new[] { BuscarTbx });
                    await re;
                    Busquedas = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ProductoModel>>(re.Result.ToString());                    
                    if (Busquedas.Count == 0 )
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
