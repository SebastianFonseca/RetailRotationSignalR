using Autofac;
using Client.Main.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Caliburn.Micro;
using System.Windows;
using System.Threading.Tasks;

namespace Client.Main.ViewModels
{
    class ProveedorResultadoBusquedaViewModel : Screen
    {

        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        ProveedorModel proveedorEncontrado = new ProveedorModel();
        public ProveedorResultadoBusquedaViewModel(MainWindowViewModel argVentana, ProveedorModel prov)
        {
            VentanaPrincipal = argVentana;
            proveedorEncontrado = prov;
        }


        public string Nombre
        {
            get { return proveedorEncontrado.firstName; }
            set
            {
                if (proveedorEncontrado.firstName != value)
                {
                    proveedorEncontrado.firstName = value;
                }
                NotifyOfPropertyChange(() => Nombre);
            }
        }

        public string Apellidos
        {
            get { return proveedorEncontrado.lastName; }
            set
            {
                if (proveedorEncontrado.lastName != value)
                {
                    proveedorEncontrado.lastName = value;
                }
                NotifyOfPropertyChange(() => Apellidos);
            }
        }

        public string CC
        {
            get { return proveedorEncontrado.cedula; }
            set
            {
                if (proveedorEncontrado.cedula != value)
                {
                    proveedorEncontrado.cedula = value;
                }
                NotifyOfPropertyChange(() => CC);
            }
        }

        public string Telefono
        {
            get { return proveedorEncontrado.telefono; }
            set
            {
                if (proveedorEncontrado.telefono != value)
                {
                    proveedorEncontrado.telefono = value;
                }
                NotifyOfPropertyChange(() => Telefono);
            }
        }

        public string Direccion
        {
            get { return proveedorEncontrado.direccion; }
            set
            {
                if (proveedorEncontrado.direccion != value)
                {
                    proveedorEncontrado.direccion = value;
                }
                NotifyOfPropertyChange(() => Direccion);
            }
        }

        public string Ciudad
        {
            get { return proveedorEncontrado.ciudad; }
            set
            {
                if (proveedorEncontrado.ciudad != value)
                {
                    proveedorEncontrado.ciudad = value;
                }
                NotifyOfPropertyChange(() => Ciudad);
            }
        }

        public BindableCollection<ProductoModel> Productos
        {
            get
            {
                return proveedorEncontrado.productos;
            }
            set
            {
                proveedorEncontrado.productos = value;
                NotifyOfPropertyChange(() => Productos);
            }
        }

        public void Editar()
        {
            VentanaPrincipal.ActivateItem(new ProveedorEditarBusquedaViewModel(VentanaPrincipal, proveedorEncontrado));
        }

        public async void Eliminar()
        {

            try
            {
            if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
            {
                MessageBoxResult result = MessageBox.Show($"Desea eliminar permanentemente de la base de datos a {proveedorEncontrado.cedula} - {proveedorEncontrado.firstName} {proveedorEncontrado.lastName}", "", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    Task<object> re = conexion.CallServerMethod("ServidorDeleteProveedor", Arguments: new[] { proveedorEncontrado.cedula });
                    await re;

                    if (re.Result.ToString() == "Se ha eliminado al proveedor.")
                    {
                        MessageBox.Show($"Se ha eliminado al usuario {proveedorEncontrado.cedula} - {proveedorEncontrado.firstName} {proveedorEncontrado.lastName}");
                        VentanaPrincipal.ActivateItem(new ProveedorBuscarViewModel(VentanaPrincipal));
                    }
                    else
                    {
                        MessageBox.Show(re.Result.ToString());
                    }

                }
            }
            else
            {
                MessageBox.Show("Para eliminar un usuario debe estar conectado al servidor.");

            }
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message);
            }
            
        }

        public void BackButton()
        {

            VentanaPrincipal.ActivateItem(new ProveedorBuscarViewModel(VentanaPrincipal));
        }



    }
}
