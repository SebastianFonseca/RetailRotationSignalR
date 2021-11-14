using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Client.Main.ViewModels
{
    class AddClientResultadoBusquedaViewModel : PropertyChangedBase
    {
        MainWindowViewModel VentanaPrincipal;

        ClientesModel resultadoCliente = new ClientesModel();       
        public AddClientResultadoBusquedaViewModel(MainWindowViewModel argVentana, ClientesModel resultadoBusqueda)
        {
            VentanaPrincipal = argVentana;
            resultadoCliente = resultadoBusqueda;
        }

        public string Name
        {
            get { return resultadoCliente.FirstName; }
            set
            {
                if (resultadoCliente.FirstName != value)
                    resultadoCliente.FirstName = value;
                NotifyOfPropertyChange(() => Name);
            }
        }


        public string Apellidos
        {
            get { return resultadoCliente.LastName; }
            set
            {
                if (resultadoCliente.LastName != value)
                    resultadoCliente.LastName = value;
                NotifyOfPropertyChange(() => Apellidos);
            }
        }

        public string CC
        {
            get { return resultadoCliente.Cedula; }
            set
            {
                if (resultadoCliente.Cedula != value)
                    resultadoCliente.Cedula = value;
                NotifyOfPropertyChange(() => CC);
            }
        }

        public string Correo
        {
            get { return resultadoCliente.Correo; }
            set
            {
                if (resultadoCliente.Correo != value)
                    resultadoCliente.Correo = value;
                NotifyOfPropertyChange(() => Correo);
            }
        }

        public string Telefono
        {
            get { return resultadoCliente.Telefono; }
            set
            {
                if (resultadoCliente.Telefono != value)
                    resultadoCliente.Telefono = value;
                NotifyOfPropertyChange(() => Telefono);

            }
        }
        public int Puntos
        {
            get { return resultadoCliente.Puntos; }
            set
            {
                if (resultadoCliente.Puntos != value)
                    resultadoCliente.Puntos = value;
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

        public void Eliminar()
        {
            MessageBoxResult result = MessageBox.Show($"Desea eliminar permanentemente de la base de datos al cliente {resultadoCliente.FirstName} {resultadoCliente.LastName}", "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                if (DbConnection.deleteCliente(resultadoCliente.Cedula))
                {
                    MessageBox.Show($"Se ha eliminado al cliente {resultadoCliente.FirstName} {resultadoCliente.LastName}");
                    VentanaPrincipal.ActivateItem(new AddClientBuscarViewModel(VentanaPrincipal));

                }
            }

        }
    }
}
