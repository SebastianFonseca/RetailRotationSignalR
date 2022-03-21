using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using Client.Main.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Client.Main.ViewModels
{
    class ProveedorNuevoViewModel: Screen, IDataErrorInfo
    {
        MainWindowViewModel VentanaPrincipal;
        ProveedorModel NuevoProveedor = new ProveedorModel();
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();

        public ProveedorNuevoViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
            getProductosServidor();

        }

        public async void getProductosServidor()
        {

            if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
            {
                try
                {
                    Task<object> re = conexion.CallServerMethod("ServidorgetIdProductos", Arguments: new object[] { });
                    await re;

                    ProductoModel[] mn = System.Text.Json.JsonSerializer.Deserialize<ProductoModel[]>(re.Result.ToString());
                    foreach (ProductoModel item in mn)
                    {
                        Productos.Add(item);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        public string Nombre
        {
            get { return NuevoProveedor.firstName; }
            set
            {
                if (NuevoProveedor.firstName != value)
                {
                    NuevoProveedor.firstName = value;
                }
                NotifyOfPropertyChange(() => Nombre);
            }
        }

        public string Apellidos
        {
            get { return NuevoProveedor.lastName; }
            set
            {
                if (NuevoProveedor.lastName != value)
                {
                    NuevoProveedor.lastName = value;
                }
                NotifyOfPropertyChange(() => Apellidos);
            }
        }

        public string CC
        {
            get { return NuevoProveedor.cedula; }
            set
            {
                if (NuevoProveedor.cedula != value)
                {
                    NuevoProveedor.cedula = value;
                }
                NotifyOfPropertyChange(() => CC);
            }
        }

        public string Telefono
        {
            get { return NuevoProveedor.telefono; }
            set
            {
                if (NuevoProveedor.telefono != value)
                {
                    NuevoProveedor.telefono = value;
                }
                NotifyOfPropertyChange(() => Telefono);
            }
        }

        public string Direccion
        {
            get { return NuevoProveedor.direccion; }
            set
            {
                if (NuevoProveedor.direccion != value)
                {
                    NuevoProveedor.direccion = value;
                }
                NotifyOfPropertyChange(() => Direccion);
            }
        }

        public string Ciudad
        {
            get { return NuevoProveedor.ciudad; }
            set
            {
                if (NuevoProveedor.ciudad    != value)
                {
                    NuevoProveedor.ciudad = value;
                }
                NotifyOfPropertyChange(() => Ciudad);
            }
        }



        BindableCollection<ProductoModel> _productos = new BindableCollection<ProductoModel>();
        public BindableCollection<ProductoModel> Productos
        {
            get
            {
                return _productos;
            }
            set 
            { 
                _productos = value; 
                NotifyOfPropertyChange(() => Productos); 
            }
        }

        private BindableCollection<ProductoModel> _selectedProductos = new BindableCollection<ProductoModel>();
        public BindableCollection<ProductoModel> SelectedProductos
        {
            get
            {
                _selectedProductos.Clear();
                _selectedProductos.AddRange(Productos.Where(mo => mo.isSelected));
                return _selectedProductos;
            }
        }
        public void BackButton()
        {
            Productos.Clear();
            VentanaPrincipal.ActivateItem(new GerenciaAdministrativoViewModel(VentanaPrincipal));
        }

        public async void Guardar()
        {
            NuevoProveedor.productos = SelectedProductos;
            if (!string.IsNullOrWhiteSpace(NuevoProveedor.firstName) &&
                !string.IsNullOrWhiteSpace(NuevoProveedor.lastName) &&
                !string.IsNullOrWhiteSpace(NuevoProveedor.cedula ) &&
                !string.IsNullOrWhiteSpace(NuevoProveedor.telefono) &&
                !string.IsNullOrWhiteSpace(NuevoProveedor.direccion) &&
                !string.IsNullOrWhiteSpace(NuevoProveedor.ciudad) &&
                SelectedProductos.Count != 0)
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    try
                    {
                        Task<object> re = conexion.CallServerMethod("ServidorNuevoProveedor", Arguments: new[] { NuevoProveedor });
                        await re;
                        if (re.Result.ToString() == "Proveedor ya registrado.")
                        {
                            MessageBox.Show(re.Result.ToString());
                            CC = "";
                            return;
                        }
                        if (re.Result.ToString() == "Se ha registrado al nuevo proveedor.")
                        {
                            MessageBox.Show(re.Result.ToString());
                            VentanaPrincipal.ActivateItem(new ProveedorNuevoViewModel(VentanaPrincipal));
                            return;
                        }
                        MessageBox.Show(re.Result.ToString());

                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }

                }
                else
                {
                    MessageBox.Show("Para agregar un nuevo local debe estar conectado al servidor.");

                }

            }
            else
            {
                MessageBox.Show("Primero debe rellenar los datos.");
            }
        }          



        public string Error { get { return null; } }
        int flag = 0;
        public string this[string name]
        {
            get
            {
                string result = null;
                //long number = 0;
                if (flag == 8)
                {
                    if (name == "Nombre")
                    {
                        if (String.IsNullOrEmpty(Nombre))
                        {
                            result = "Este campo no puede estar vacío.";
                        }
                    }
                    else if (name == "Apellidos")
                    {
                        if (String.IsNullOrEmpty(Apellidos))
                        {
                            result = "Este campo no puede estar vacío.";
                        }
                    }
                    else if (name == "CC")
                    {
                        if (String.IsNullOrEmpty(CC))
                        {
                            result = "Este campo no puede estar vacío.";
                        }
                    }
                    else if (name == "Telefono")
                    {
                        if (String.IsNullOrEmpty(Telefono))
                        {
                            result = "Este campo no puede estar vacío.";
                        }
                        if (Telefono.Length != 10)
                        {
                            result = result + " Debe ser un número de 10 dígitos.";

                        }
                    }
                    else if (name == "Direccion")
                    {
                        if (String.IsNullOrEmpty(Direccion))
                        {
                            result = "Este campo no puede estar vacío.";
                        }
                    }
                    else if (name == "Ciudad")
                    {
                        if (String.IsNullOrEmpty(Ciudad))
                        {
                            result = "Este campo no puede estar vacío.";
                        }
                    }
                    else if (name == "Productos")
                    {
                        if (SelectedProductos.Count == 0)
                        {
                            result = "Debe elegir algun producto.";
                        }
                    }


                }

                else { flag += 1; }


                return result;
            }
        }



    }


}
