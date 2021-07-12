using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace Client.Main.ViewModels
{
    class NuevoLocalViewModel : Screen, IDataErrorInfo
    {
        MainWindowViewModel VentanaPrincipal;
        LocalModel NuevoLocal = new LocalModel();
        EmpleadoModel Administrador = new EmpleadoModel();

        public static BindableCollection<EmpleadoModel> _administradores = new BindableCollection<EmpleadoModel>();
        public BindableCollection<EmpleadoModel> Administradores
        {
            get
            {
                _administradores = DbConnection.getAdministradores();
                return _administradores; 
            }
            set { Administradores = value; }
        }

        public NuevoLocalViewModel(MainWindowViewModel argVentana)
        {
            //NuevoLocal.Administrador = Administrador;
            VentanaPrincipal = argVentana;
        }



        public string Nombre
        {
            get { return NuevoLocal.Nombre; }
            set
                {
                if (NuevoLocal.Nombre != value)
                {
                    NuevoLocal.Nombre = value;
                }
                NotifyOfPropertyChange(() => Nombre);

            }
        }

        public string Direccion
        {
            get { return NuevoLocal.Direccion; }
            set
            {
                if (NuevoLocal.Direccion != value)
                {
                    NuevoLocal.Direccion = value;
                }
                NotifyOfPropertyChange(() => Direccion);

            }
        }

        public string Telefono
        {
            get { return NuevoLocal.Telefono; }
            set
            {
                if (NuevoLocal.Telefono != value)
                {
                    NuevoLocal.Telefono = value;
                }
                NotifyOfPropertyChange(() => Telefono);

            }
        }

        public string Ciudad
        {
            get { return NuevoLocal.Ciudad; }
            set
            {
                if (NuevoLocal.Ciudad != value)
                {
                    NuevoLocal.Ciudad = value;
                }
                NotifyOfPropertyChange(() => Ciudad);

            }
        }

        public int NumeroDeCanastillas
        {
            get { return NuevoLocal.NumeroDeCanastillas; }
            set
            {
                if (NuevoLocal.NumeroDeCanastillas != value)
                {
                    NuevoLocal.NumeroDeCanastillas = value;
                }
                NotifyOfPropertyChange(() => NumeroDeCanastillas);

            }
        }

        public DateTime FechaDeApertura
        {
            get { return NuevoLocal.FechaDeApertura; }
            set
            {
                if (NuevoLocal.FechaDeApertura != value)
                {
                    NuevoLocal.FechaDeApertura = value;
                }
                NotifyOfPropertyChange(() => FechaDeApertura);

            }
        }

        private EmpleadoModel _admin;

        public EmpleadoModel Admin
        {
            get { return _admin; }
            set {
                //NuevoLocal.Administrador = value;
                _admin = value;
                NotifyOfPropertyChange(() => Admin);
            }
        }


        public void Guardar()
        {
            DbConnection.NuevoIdLocal();

            if (MainWindowViewModel.Status == "Trabajando localmente")
            {
                if (!string.IsNullOrWhiteSpace(NuevoLocal.Nombre ) &&
                    !string.IsNullOrWhiteSpace(NuevoLocal.Direccion) &&
                    !string.IsNullOrWhiteSpace(NuevoLocal.Telefono) &&
                    !string.IsNullOrWhiteSpace(NuevoLocal.Ciudad) &&
                    !string.IsNullOrWhiteSpace(NuevoLocal.NumeroDeCanastillas.ToString()) &&
                    !string.IsNullOrWhiteSpace(NuevoLocal.FechaDeApertura.ToString("DD-MM-YYYY")))// &&
                    //!string.IsNullOrWhiteSpace(NuevoLocal.Administrador.Cedula))

                {
                    DbConnection.NuevoLocal(NuevoLocal: NuevoLocal);
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
            _administradores.Clear();
            VentanaPrincipal.ActivateItem(new DC_AdministrativoViewModel(VentanaPrincipal));
        }

        public string Error { get { return null; } }
        int flag = 0;
        public string this[string name]
        {
            get
            {
                string result = null;
                if (flag == 8)
                {
                     if (name == "Nombre")
                    {
                        if (String.IsNullOrEmpty(Nombre))
                        {
                            result = "Este campo no puede estar vacío.";
                        }
                    }


                    else if (name == "Direccion")
                    {
                        if (String.IsNullOrEmpty(Direccion))
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
                    else if (name == "Ciudad")
                    {
                        if (String.IsNullOrEmpty(Ciudad))
                        {
                            result = "Este campo no puede estar vacío.";
                        }

                    }
                    else if (name == "NumeroDeCanastillas")
                    {
                        if (String.IsNullOrEmpty(NumeroDeCanastillas.ToString()))
                        {
                            result = "Este campo no puede estar vacío.";
                        }
   
                    }
                    else if (name == "FechaDeApertura")
                    {
                        if (String.IsNullOrEmpty(FechaDeApertura.ToString()) | (FechaDeApertura > DateTime.Today | FechaDeApertura < new DateTime(2010, 01, 01)))
                        {
                            result = "Elija o escriba una fecha válida.";
                        }


                    }
                    else if (name == "Administrador")
                    {
                        if (String.IsNullOrEmpty(Admin.Cedula))
                        {
                            result = "Elija una opción.";
                        }

                    }

                }

                else { flag += 1; }


                return result;
            }
        }





    }

}
