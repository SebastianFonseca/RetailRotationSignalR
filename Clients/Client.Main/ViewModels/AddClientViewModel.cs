 using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Client.Main.Utilities;

namespace Client.Main.ViewModels
{
    class AddClientViewModel : Screen
    {
        private string _nombre;

        public string Nombre
        {
            get { return _nombre; }
            set {

                _nombre = value;
                NotifyOfPropertyChange(() => Nombre);
                NotifyOfPropertyChange(() => CanGuardar);
            }
        }

        private string _apellidos;

        public string Apellidos
        {
            get { return _apellidos; }
            set {
                _apellidos = value;
                NotifyOfPropertyChange(() => Apellidos);
                NotifyOfPropertyChange(() => CanGuardar);
            }
        }

        private string _cc;

        public string CC
        {
            get { return _cc; }
            set {
                _cc = value;
                NotifyOfPropertyChange(() => CC);
                NotifyOfPropertyChange(() => CanGuardar);
            }
        }

        public string Correo { get; set; }

        public string Telefono { get; set; }
        public bool CanGuardar => !string.IsNullOrWhiteSpace(this.Nombre) &&
                                  !string.IsNullOrWhiteSpace(this.Apellidos) &&
                                  !string.IsNullOrWhiteSpace(this.CC);

        public void Guardar()
        {
            if (string.IsNullOrEmpty(Correo))
            {
                Correo = null;
            }
            if (string.IsNullOrEmpty(Telefono))
            {
                Telefono = null;
            }

            DbConnection.AddClient(pNombre:Nombre,pApellidos:Apellidos,pCedula:CC,pCorreo:Correo,pTelefono:Telefono);
        }
    }


}
