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
    public class ComprasPagosViewModel : Screen
    {
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
   
        public ComprasPagosViewModel(MainWindowViewModel argVentana)
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



        private BindableCollection<ProveedorModel> _busquedasProveedor = new BindableCollection<ProveedorModel>();
        public BindableCollection<ProveedorModel> BusquedasProveedor
        {
            get
            {
                return _busquedasProveedor;
            }
            set
            {
                _busquedasProveedor = value;
                NotifyOfPropertyChange(() => BusquedasProveedor);
            }
        }

        public string codigoCedula = "";

        private ProveedorModel _proveedorSeleccionado; 
        public ProveedorModel ProveedorSeleccionado
        {
            get { return _proveedorSeleccionado; }
            set
            {
                if (value != null)
                {
                    codigoCedula = value.cedula;
                    BuscarTbx = value.cedula + " - " + value.firstName + " " + value.lastName;
                    
                }
                _proveedorSeleccionado = value;

                NotifyOfPropertyChange(() => ProveedorSeleccionado);
            }
        }




        private BindableCollection<ProductoModel> _busquedasProducto = new BindableCollection<ProductoModel>();
        public BindableCollection<ProductoModel> BusquedasProducto
        {
            get
            {
                return _busquedasProducto;
            }
            set
            {
                _busquedasProducto = value;
                NotifyOfPropertyChange(() => BusquedasProducto);
            }
        }


        private ProductoModel _poductoSeleccionado;
        public ProductoModel ProductoSeleccionado
        {
            get { return _poductoSeleccionado; }
            set
            {
                if (value != null)
                {
                    codigoCedula = value.codigoProducto;
                    BuscarTbx = value.codigoProducto + " - " + value.nombre;
                    
                }
                _poductoSeleccionado = value;

                NotifyOfPropertyChange(() => ProveedorSeleccionado);
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
                        //Task<object> re = conexion.CallServerMethod("ServidorGetProveedor", Arguments: new[] { BuscarTbx.Split('-')[0].Trim() });
                        //await re;
                        //ComprasModel[] proveedor = System.Text.Json.JsonSerializer.Deserialize<ComprasModel[]>(re.Result.ToString());
                        //if (proveedor[0].cedula == null)
                        //{
                        //    MessageBox.Show("Número de cédula, nombre o apellido no resgistrados");
                        //}
                        //else
                        //{
                        //    VentanaPrincipal.ActivateItem(new ProveedorResultadoBusquedaViewModel(VentanaPrincipal, proveedor[0]));
                        //    BusquedasVisibilidad = "Visible";
                        //    //                            ComboboxDesplegado = "True";
                        //}
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
                else
                {
                    VentanaPrincipal.ActivateItem(new ComprasPagosEditarViewModel(VentanaPrincipal, codigoCedula));
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
            BusquedasProveedor.Clear();
            BusquedasProducto.Clear();
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    BindableCollection<ComprasModel> proveedores = new BindableCollection<ComprasModel>();
                    Task<object> re = conexion.CallServerMethod("ServidorGetProveedores", Arguments: new[] { BuscarTbx });
                    await re;
                    //ComprasModel[] mn = System.Text.Json.JsonSerializer.Deserialize<ComprasModel[]>(re.Result.ToString());

                    proveedores = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ComprasModel>>(re.Result.ToString());

                    if (proveedores.Count != 0)
                    {
                        
                    }
                    if (BusquedasProducto.Count == 0 & BusquedasProducto.Count == 0)
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
                        BusquedasProveedor = DbConnection.getNombresProveedores(BuscarTbx);
                        BusquedasProducto = DbConnection.getProductos(caracteres: BuscarTbx);
                        if (BusquedasProveedor.Count == 0 & BusquedasProducto.Count == 0)
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
