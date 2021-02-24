 using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Client.Main.Utilities;
using Client.Main.Models;

namespace Client.Main.ViewModels
{
    public class AddClientViewModel : Screen
    {
        ClientesModel NuevoCliente;

        public AddClientViewModel()
        {
            NuevoCliente = new ClientesModel();
        }
        
        public string Nombre
        {
            get { return NuevoCliente.FirstName; }
            set {

                if (NuevoCliente.FirstName != value)                        
                    NuevoCliente.FirstName = value;                       
                NotifyOfPropertyChange(() => Nombre);
                NotifyOfPropertyChange(() => CanGuardar);
            }
        }

        public string Apellidos
        {
            get { return NuevoCliente.LastName; }
            set {
                if(NuevoCliente.LastName != value)
                    NuevoCliente.LastName = value;
                NotifyOfPropertyChange(() => Apellidos);
                NotifyOfPropertyChange(() => CanGuardar);
            }
        }

        public string CC
        {
            get { return NuevoCliente.Cedula; }
            set {
                if(NuevoCliente.Cedula != value)
                    NuevoCliente.Cedula = value;
                NotifyOfPropertyChange(() => CC);
                NotifyOfPropertyChange(() => CanGuardar);
            }
        }


        public string  Correo
        {
            get { return NuevoCliente.Correo; }
            set {
                if(NuevoCliente.Correo != value)
                    NuevoCliente.Correo = value;
                NotifyOfPropertyChange(() => Correo);
            }
        }

        public string Telefono
        {
            get { return NuevoCliente.Telefono; }
            set { 
                if(NuevoCliente.Telefono != value)
                    NuevoCliente.Telefono = value;
                NotifyOfPropertyChange(() => Telefono);

            }
        }

        public bool CanGuardar => !string.IsNullOrWhiteSpace(this.Nombre) &&
                                  !string.IsNullOrWhiteSpace(this.Apellidos) &&
                                  !string.IsNullOrWhiteSpace(this.CC);

        public void Guardar()
        {
            if (string.IsNullOrEmpty(NuevoCliente.Correo))
            {  
                NuevoCliente.Correo = null;
            }
            if (string.IsNullOrEmpty(NuevoCliente.Telefono))
            {
                NuevoCliente.Telefono = null;
            }

            DbConnection.AddClient(Cliente: NuevoCliente);

            Nombre = "";
            Apellidos = "";
            CC = "";
            Correo = "";
            Telefono = "";
        }
    }  
}


