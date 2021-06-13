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
    class NuevoUsuarioViewModel : PropertyChangedBase, IDataErrorInfo
    {
        MainWindowViewModel VentanaPrincipal;


        EmpleadoModel NuevoEmpleado = new EmpleadoModel();

        public BindableCollection<LocalModel> Locales
        {
            get
            {
                return DbConnection.getLocales();
            }

        }


        public LocalModel Local 
        {
            get
            {
                return NuevoEmpleado.PuntoDeVenta; 
            }
            set
            {
                NuevoEmpleado.PuntoDeVenta = value;
                NotifyOfPropertyChange(() => Local);
            }
        }

        public NuevoUsuarioViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
        }

        public string Nombre
        {
            get { return NuevoEmpleado.FirstName; }
            set { 
            if(NuevoEmpleado.FirstName != value)
                {
                    NuevoEmpleado.FirstName = value;
                }
                NotifyOfPropertyChange(() => Nombre);
            
            }
        }
        public string Apellidos
        {
            get { return NuevoEmpleado.LastName; }
            set
            {
                if (NuevoEmpleado.LastName != value)
                {
                    NuevoEmpleado.LastName = value;
                }
                NotifyOfPropertyChange(() => Apellidos);

            }
        }

        public string CC
        {
            get { return NuevoEmpleado.Cedula; }
            set
            {
                if (NuevoEmpleado.Cedula != value)
                {
                    NuevoEmpleado.Cedula = value;
                }
                NotifyOfPropertyChange(() => CC);

            }
        }

        public string Correo
        {
            get { return NuevoEmpleado.Correo; }
            set
            {
                if (NuevoEmpleado.Correo != value)
                {
                    NuevoEmpleado.Correo = value;
                }
                NotifyOfPropertyChange(() => Correo);

            }
        }

        public string Telefono
        {
            get { return NuevoEmpleado.Telefono; }
            set
            {
                if (NuevoEmpleado.Telefono != value)
                {
                    NuevoEmpleado.Telefono = value;
                }
                NotifyOfPropertyChange(() => Telefono);

            }
        }


        public DateTime FechaContratacion
        {
            get { return NuevoEmpleado.FechaDeContratacion; }
            set
            {
                if (NuevoEmpleado.FechaDeContratacion != value)
                {
                    NuevoEmpleado.FechaDeContratacion = value;
                }
                NotifyOfPropertyChange(() => FechaContratacion);

            }
        }

        public string Cargo
        {
            get { return NuevoEmpleado.Cargo; }
            set
            {
                if (NuevoEmpleado.Cargo != value)
                {
                    NuevoEmpleado.Cargo = value;
                }
                NotifyOfPropertyChange(() => Cargo);

            }
        }

        public string Password
        {
            get { return NuevoEmpleado.Password; }
            set
            {
                if (NuevoEmpleado.Password != value)
                {
                    NuevoEmpleado.Password = value;
                }
                NotifyOfPropertyChange(() => Password);

            }
        }

        public string Salario
        {
            get { return NuevoEmpleado.Salario; }
            set
            {
                if (NuevoEmpleado.Salario != value)
                {
                    NuevoEmpleado.Salario = value;
                }
                NotifyOfPropertyChange(() => Salario);

            }
        }

        private string _passwordAgain;

        public string PasswordAgain
        {
            get { return _passwordAgain; }
            set 
            {               
                _passwordAgain = value;
                NotifyOfPropertyChange(() => PasswordAgain);
            }
        }

        public void Guardar()
        {
            if (Statics.ClientStatus == "Trabajando localmente")
            {
                if (!string.IsNullOrWhiteSpace(NuevoEmpleado.FirstName) && 
                    !string.IsNullOrWhiteSpace(NuevoEmpleado.LastName) && 
                    !string.IsNullOrWhiteSpace(NuevoEmpleado.Cedula) &&
                    !string.IsNullOrWhiteSpace(NuevoEmpleado.Correo) &&
                    !string.IsNullOrWhiteSpace(NuevoEmpleado.Telefono) &&
                    !string.IsNullOrWhiteSpace(NuevoEmpleado.FechaDeContratacion.ToString()) &&
                    !string.IsNullOrWhiteSpace(NuevoEmpleado.Cargo) &&
                    !string.IsNullOrWhiteSpace(NuevoEmpleado.Password) &&
                    !string.IsNullOrWhiteSpace(PasswordAgain))
                {
                    if (DbConnection.NuevoUsuario(Empleado: NuevoEmpleado))
                    {
                        VentanaPrincipal.ActivateItem(new NuevoUsuarioViewModel(VentanaPrincipal));
                    }
                    else
                    {
                        CC = "";
                    }
                }
                else
                {
                    MessageBox.Show("Primero debe rellenar los datos.");

                }


            }
            else
            {
                //Llamar metodo del servidor.
            }
           
        }
       
        public void BackButton()
        {
            Locales.Clear();
            VentanaPrincipal.ActivateItem(new DC_AdministrativoViewModel(VentanaPrincipal));
        }




        public string Error { get { return null; } }
        int flag = 0;
        public string this[string name]
        {
            get
            {
                string result = null;
                long number = 0;
                if (flag == 10)
                {
                    if (name == "CC")
                    {
                        if (!long.TryParse(CC, out number))
                        {
                            result = "Este campo no puede estar vacío.";
                        }
                    }
                    else if (name == "Nombre")
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
                    else if (name == "Correo")
                    {
                        if (String.IsNullOrEmpty(Correo))
                        {
                            result = "Este campo no puede estar vacío.";
                        }

                    }
                    else if (name == "Telefono")
                    {
                        if (String.IsNullOrEmpty(Telefono) )
                        {
                            result = "Este campo no puede estar vacío.";
                        }
                        if(Telefono.Length != 10)
                        {
                            result = result + " Debe ser un número de 10 dígitos.";

                        }
                    }
                    else if (name == "FechaContratacion")
                    {
                        if (String.IsNullOrEmpty(FechaContratacion.ToString()) | (FechaContratacion > DateTime.Today | FechaContratacion < new DateTime(2010, 01, 01)))
                        {
                            result = "Elija o escriba una fecha válida.";
                        }


                    }
                    else if (name == "SelectedCargo")
                    {
                        if (String.IsNullOrEmpty(Cargo))
                        {
                            result = "Elija una opción.";
                        }

                    }
                    else if (name == "Local")
                    {
                        if (Local == null)
                        {
                            result = "Elija una opción.";
                        }

                    }
                    else if (name == "PasswordAgain" )
                    {
                        if (Password != null) { 
                        if (Password != PasswordAgain)
                        {
                            result = "Las contraseñas no coinciden.";
                        }
                        if (Password.Length < 10)
                        {
                            result = result + " Minimo 10 caracteres.";
                        }
}
                    }

                }

                else { flag += 1; }


                return result;
            }
        }

    }
}
