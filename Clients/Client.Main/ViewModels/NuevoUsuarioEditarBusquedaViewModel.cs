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
    class NuevoUsuarioEditarBusquedaViewModel : PropertyChangedBase, IDataErrorInfo
    {
        MainWindowViewModel VentanaPrincipal;

        EmpleadoModel resultadoEmpleado = new EmpleadoModel();
        string CedulaAntigua;

        public NuevoUsuarioEditarBusquedaViewModel(MainWindowViewModel argVentana, EmpleadoModel resultadoBusqueda)
        {
            VentanaPrincipal = argVentana;
            resultadoEmpleado = resultadoBusqueda;
            CedulaAntigua = resultadoEmpleado.Cedula;
        }


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
                return resultadoEmpleado.PuntoDeVenta;
            }
            set
                
            {
                if (value != null)
                {
                    resultadoEmpleado.PuntoDeVenta = value;
                    NotifyOfPropertyChange(() => Local);
                }

                
            }
        }



        public string Nombre
        {
            get { return resultadoEmpleado.FirstName; }
            set
            {
                if (resultadoEmpleado.FirstName != value)
                {
                    resultadoEmpleado.FirstName = value;
                }
                NotifyOfPropertyChange(() => Nombre);

            }
        }
        public string Apellidos
        {
            get { return resultadoEmpleado.LastName; }
            set
            {
                if (resultadoEmpleado.LastName != value)
                {
                    resultadoEmpleado.LastName = value;
                }
                NotifyOfPropertyChange(() => Apellidos);

            }
        }

        public string CC
        {
            get { return resultadoEmpleado.Cedula; }
            set
            {
                if (resultadoEmpleado.Cedula != value)
                {
                    resultadoEmpleado.Cedula = value;
                }
                NotifyOfPropertyChange(() => CC);

            }
        }

        public string Telefono
        {
            get { return resultadoEmpleado.Telefono; }
            set
            {
                if (resultadoEmpleado.Telefono != value)
                {
                    resultadoEmpleado.Telefono = value;
                }
                NotifyOfPropertyChange(() => Telefono);

            }
        }


        public DateTime FechaContratacion
        {
            get { return resultadoEmpleado.FechaDeContratacion; }
            set
            {
                if (resultadoEmpleado.FechaDeContratacion != value)
                {
                    resultadoEmpleado.FechaDeContratacion = value;
                }
                NotifyOfPropertyChange(() => FechaContratacion);

            }
        }

        public string Cargo
        {
            get { return resultadoEmpleado.Cargo; }
            set
            {
                //MessageBox.Show( value.Length.ToString());
                if (resultadoEmpleado.Cargo != value)
                {
                    resultadoEmpleado.Cargo = value;
                }
                NotifyOfPropertyChange(() => Cargo);

            }
        }

        public string Password
        {
            get { return resultadoEmpleado.Password; }
            set
            {
                if (resultadoEmpleado.Password != value)
                {
                    resultadoEmpleado.Password = value;
                }
                NotifyOfPropertyChange(() => Password);

            }
        }

        public decimal Salario
        {
            get { return resultadoEmpleado.Salario; }
            set
           {
                if (resultadoEmpleado.Salario != value)
                {
                    resultadoEmpleado.Salario = value;
                }
                NotifyOfPropertyChange(() => Salario);

            }
        }

        public string Direccion
        {
            get { return resultadoEmpleado.Direccion; }
            set
            {
                if (resultadoEmpleado.Direccion != value)
                {
                    resultadoEmpleado.Direccion = value;
                }
                NotifyOfPropertyChange(() => Direccion);

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

        public void Actualizar()
        {
            if (Statics.ClientStatus == "Trabajando localmente")
            {
                if (!string.IsNullOrWhiteSpace(resultadoEmpleado.FirstName) &&
                    !string.IsNullOrWhiteSpace(resultadoEmpleado.LastName) &&
                    !string.IsNullOrWhiteSpace(resultadoEmpleado.Cedula) &&
                    !string.IsNullOrWhiteSpace(resultadoEmpleado.Direccion) &&
                    !string.IsNullOrWhiteSpace(resultadoEmpleado.Telefono) &&
                    !string.IsNullOrWhiteSpace(resultadoEmpleado.PuntoDeVenta.Nombre) &&
                    !string.IsNullOrWhiteSpace(resultadoEmpleado.FechaDeContratacion.ToString()) &&
                    !string.IsNullOrWhiteSpace(resultadoEmpleado.Cargo) &&
                    !string.IsNullOrWhiteSpace(resultadoEmpleado.Salario.ToString()) &&
                    !string.IsNullOrWhiteSpace(resultadoEmpleado.Password) &&
                    !string.IsNullOrWhiteSpace(PasswordAgain))
                {
                    if (DbConnection.ActualizarUsuario(Empleado: resultadoEmpleado, CC: CedulaAntigua))
                    {
                        resultadoEmpleado.Password = "";
                        VentanaPrincipal.ActivateItem(new NuevoUsuarioResultadoBusquedaViewModel(VentanaPrincipal, resultadoEmpleado));
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
            VentanaPrincipal.ActivateItem(new BuscarUsuarioViewModel(VentanaPrincipal));
        }




        public string Error { get { return null; } }
        int flag = 0;
        public string this[string name]
        {
            get
            {
                string result = null;
                long number = 0;
                if (flag == 9)
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
                    else if (name == "PasswordAgain")
                    {
                        if (Password != null)
                        {
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
