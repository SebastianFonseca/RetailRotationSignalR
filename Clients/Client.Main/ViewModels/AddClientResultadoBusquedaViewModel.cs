using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main.ViewModels
{
    class AddClientResultadoBusquedaViewModel : PropertyChangedBase
    {
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();

        ClientesModel resultadoCliente = new ClientesModel();       
        public AddClientResultadoBusquedaViewModel(MainWindowViewModel argVentana, ClientesModel resultadoBusqueda)
        {
            VentanaPrincipal = argVentana;
            resultadoCliente = resultadoBusqueda;
        }

        public string Name
        {
            get { return resultadoCliente.firstName; }
            set
            {
                if (resultadoCliente.firstName != value)
                    resultadoCliente.firstName = value;
                NotifyOfPropertyChange(() => Name);
            }
        }


        public string Apellidos
        {
            get { return resultadoCliente.lastName; }
            set
            {
                if (resultadoCliente.lastName != value)
                    resultadoCliente.lastName = value;
                NotifyOfPropertyChange(() => Apellidos);
            }
        }

        public string CC
        {
            get { return resultadoCliente.cedula; }
            set
            {
                if (resultadoCliente.cedula != value)
                    resultadoCliente.cedula = value;
                NotifyOfPropertyChange(() => CC);
            }
        }

        public string Correo
        {
            get { return resultadoCliente.correo; }
            set
            {
                if (resultadoCliente.correo != value)
                    resultadoCliente.correo = value;
                NotifyOfPropertyChange(() => Correo);
            }
        }

        public string Telefono
        {
            get { return resultadoCliente.telefono; }
            set
            {
                if (resultadoCliente.telefono != value)
                    resultadoCliente.telefono = value;
                NotifyOfPropertyChange(() => Telefono);

            }
        }
        public int Puntos
        {
            get { return resultadoCliente.puntos; }
            set
            {
                if (resultadoCliente.puntos != value)
                    resultadoCliente.puntos = value;
                NotifyOfPropertyChange(() => Telefono);

            }
        }



        public void BackButton()
        {

            VentanaPrincipal.ActivateItem(new AddClientBuscarViewModel(VentanaPrincipal));
        }


        public void Editar()
        {
            VentanaPrincipal.ActivateItem(new AddClientEditarBusquedaViewModel(VentanaPrincipal, resultadoCliente));
        }

        public async void Eliminar()
        {
            MessageBoxResult result = MessageBox.Show($"Desea eliminar permanentemente de la base de datos al cliente {resultadoCliente.firstName} {resultadoCliente.lastName}", "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
               
                try
                {
                    if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                    {
                        Task<object> re = conexion.CallServerMethod("ServidordeleteCliente", Arguments: new object[] { resultadoCliente.cedula });
                        await re;

                        if (re.Result.ToString() == "true")
                        {
                            MessageBox.Show($"Se ha eliminado al cliente {resultadoCliente.firstName} {resultadoCliente.lastName}");
                            VentanaPrincipal.ActivateItem(new AddClientBuscarViewModel(VentanaPrincipal));
                        }
                    }
                    if (MainWindowViewModel.Status == "Trabajando localmente")
                    {
                        if (DbConnection.deleteCliente(resultadoCliente.cedula))
                        {
                            MessageBox.Show($"Se ha eliminado al cliente {resultadoCliente.firstName} {resultadoCliente.lastName}");
                            VentanaPrincipal.ActivateItem(new AddClientBuscarViewModel(VentanaPrincipal));

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
