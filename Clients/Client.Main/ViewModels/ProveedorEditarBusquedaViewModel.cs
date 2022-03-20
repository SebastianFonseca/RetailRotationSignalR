using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main.ViewModels
{
    class ProveedorEditarBusquedaViewModel: Screen
    {
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        ProveedorModel proveedorEditar = new ProveedorModel();
        public ProveedorEditarBusquedaViewModel(MainWindowViewModel argVentana, ProveedorModel prov)
        {
            VentanaPrincipal = argVentana;
            proveedorEditar = prov;
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
            get { return proveedorEditar.firstName; }
            set
            {
                if (proveedorEditar.firstName != value)
                {
                    proveedorEditar.firstName = value;
                }
                NotifyOfPropertyChange(() => Nombre);
            }
        }

        public string Apellidos
        {
            get { return proveedorEditar.lastName; }
            set
            {
                if (proveedorEditar.lastName != value)
                {
                    proveedorEditar.lastName = value;
                }
                NotifyOfPropertyChange(() => Apellidos);
            }
        }

        public string CC
        {
            get { return proveedorEditar.cedula; }
            set
            {
                if (proveedorEditar.cedula != value)
                {
                    proveedorEditar.cedula = value;
                }
                NotifyOfPropertyChange(() => CC);
            }
        }

        public string Telefono
        {
            get { return proveedorEditar.telefono; }
            set
            {
                if (proveedorEditar.telefono != value)
                {
                    proveedorEditar.telefono = value;
                }
                NotifyOfPropertyChange(() => Telefono);
            }
        }

        public string Direccion
        {
            get { return proveedorEditar.direccion; }
            set
            {
                if (proveedorEditar.direccion != value)
                {
                    proveedorEditar.direccion = value;
                }
                NotifyOfPropertyChange(() => Direccion);
            }
        }

        public string Ciudad
        {
            get { return proveedorEditar.ciudad; }
            set
            {
                if (proveedorEditar.ciudad != value)
                {
                    proveedorEditar.ciudad = value;
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
            VentanaPrincipal.ActivateItem(new ProveedorResultadoBusquedaViewModel(VentanaPrincipal, proveedorEditar));
        }

        public async void Guardar()
        {
            proveedorEditar.productos = SelectedProductos;
            if (!string.IsNullOrWhiteSpace(proveedorEditar.firstName) &&
                !string.IsNullOrWhiteSpace(proveedorEditar.lastName) &&
                !string.IsNullOrWhiteSpace(proveedorEditar.cedula) &&
                !string.IsNullOrWhiteSpace(proveedorEditar.telefono) &&
                !string.IsNullOrWhiteSpace(proveedorEditar.direccion) &&
                !string.IsNullOrWhiteSpace(proveedorEditar.ciudad) &&
                SelectedProductos.Count != 0)
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    try
                    {
                        Task<object> re = conexion.CallServerMethod("ServidorActualizarProveedor", Arguments: new[] { proveedorEditar });
                        await re;
                        if (re.Result.ToString() == "Proveedor ya registrado.")
                        {
                            MessageBox.Show(re.Result.ToString());
                            CC = "";
                            return;
                        }
                        if (re.Result.ToString() == "Se ha editado la informacion.")
                        {
                            MessageBox.Show(re.Result.ToString());
                            VentanaPrincipal.ActivateItem(new ProveedorResultadoBusquedaViewModel(VentanaPrincipal, proveedorEditar));
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
