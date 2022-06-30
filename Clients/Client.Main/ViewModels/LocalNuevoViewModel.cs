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
using Autofac;
using System.Threading.Tasks;

namespace Client.Main.ViewModels
{
    class LocalNuevoViewModel : Screen, IDataErrorInfo
    {
        MainWindowViewModel VentanaPrincipal;
        LocalModel NuevoLocal = new LocalModel();
        EmpleadoModel Administrador = new EmpleadoModel();
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();


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

        public LocalNuevoViewModel(MainWindowViewModel argVentana)
        {
            //NuevoLocal.Administrador = Administrador;
            VentanaPrincipal = argVentana;
        }



        public string Nombre
        {
            get { return NuevoLocal.nombre; }
            set
            {
                if (decimal.TryParse(value, out decimal a)) { MessageBox.Show("Los caracteres no pueden contener solo números"); }
                if (NuevoLocal.nombre != value)
                {
                    NuevoLocal.nombre = value;
                }
                NotifyOfPropertyChange(() => Nombre);

            }
        }

        public string Direccion
        {
            get { return NuevoLocal.direccion; }
            set
            {
                if (NuevoLocal.direccion != value)
                {
                    NuevoLocal.direccion = value;
                }
                NotifyOfPropertyChange(() => Direccion);

            }
        }

        public string Telefono
        {
            get { return NuevoLocal.telefono; }
            set
            {
                if (NuevoLocal.telefono != value)
                {
                    NuevoLocal.telefono = value;
                }
                NotifyOfPropertyChange(() => Telefono);

            }
        }

        public string Ciudad
        {
            get { return NuevoLocal.ciudad; }
            set
            {
                if (NuevoLocal.ciudad != value)
                {
                    NuevoLocal.ciudad = value;
                }
                NotifyOfPropertyChange(() => Ciudad);

            }
        }

        public int NumeroDeCanastillas
        {
            get { return NuevoLocal.numeroDeCanastillas; }
            set
            {
                if (NuevoLocal.numeroDeCanastillas != value)
                {
                    NuevoLocal.numeroDeCanastillas = value;
                }
                NotifyOfPropertyChange(() => NumeroDeCanastillas);

            }
        }

        public DateTime FechaDeApertura
        {
            get { return NuevoLocal.fechaDeApertura; }
            set
            {
                if (NuevoLocal.fechaDeApertura != value)
                {
                    NuevoLocal.fechaDeApertura = value;
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


        public async void Guardar()
        {           
            if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
            {
                
                if (!string.IsNullOrWhiteSpace(NuevoLocal.nombre ) &&
                    !string.IsNullOrWhiteSpace(NuevoLocal.direccion) &&
                    !string.IsNullOrWhiteSpace(NuevoLocal.telefono) &&
                    !string.IsNullOrWhiteSpace(NuevoLocal.ciudad) &&
                    !string.IsNullOrWhiteSpace(NuevoLocal.numeroDeCanastillas.ToString()) &&
                    !string.IsNullOrWhiteSpace(NuevoLocal.fechaDeApertura.ToString("DD-MM-YYYY")))

                {
                    try
                    {
                        Task<object> re = conexion.CallServerMethod("ServidorNuevoLocal", Arguments: new[] { NuevoLocal });
                        await re;
                        if ((re.Result.ToString()).Substring(0, 9) == "El nombre")
                        {
                            MessageBox.Show(re.Result.ToString());
                            Nombre = "";
                            return;
                        }
                        if ((re.Result.ToString()).Substring(0, 16) == "Se ha registrado")
                        {
                            MessageBox.Show(re.Result.ToString());
                            VentanaPrincipal.ActivateItem(new LocalNuevoViewModel(VentanaPrincipal));
                            return;
                        }
                        MessageBox.Show(re.Result.ToString());

                        //DbConnection.NuevoLocal(NuevoLocal: NuevoLocal);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }

                }
                else
                {
                    MessageBox.Show("Primero debe rellenar los datos.");

                }


            }
            else
            {
                MessageBox.Show("Para agregar un nuevo local debe estar conectado al servidor.");
            }
        }


        public void BackButton()
        {
            _administradores.Clear();
            VentanaPrincipal.ActivateItem(new GerenciaAdministrativoViewModel(VentanaPrincipal));
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
                        if (String.IsNullOrEmpty(Admin.cedula))
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
