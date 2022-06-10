using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Client.Main.ViewModels
{
    class NuevoUsuarioViewModel : PropertyChangedBase, IDataErrorInfo
    {
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();


        EmpleadoModel NuevoEmpleado = new EmpleadoModel();

        public NuevoUsuarioViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
            getLocalesServidor();
        }

        private BindableCollection<LocalModel> _locales = new BindableCollection<LocalModel>();
        public  BindableCollection<LocalModel> Locales
        {
            get
            {

                return _locales;
                //return DbConnection.getLocales();
            }
            set {
                
                _locales = value;
                NotifyOfPropertyChange(() => Locales);
            }

        }

        public async void getLocalesServidor()
        {

            if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
            {
                try
                {
                    Task<object> re = conexion.CallServerMethod("ServidorGetIdLocales", Arguments: new object[] { });
                    await re;

                    LocalModel[] mn = System.Text.Json.JsonSerializer.Deserialize<LocalModel[]>(re.Result.ToString());
                    BindableCollection<LocalModel> lcl = new BindableCollection<LocalModel>();
                    foreach (LocalModel item in mn)
                    {
                        lcl.Add(item);
                    }
                    Locales = lcl;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        
        public LocalModel Local 
        {
            get
            {
                return NuevoEmpleado.puntoDeVenta; 
            }
            set
            {
                NuevoEmpleado.puntoDeVenta = value;
                NotifyOfPropertyChange(() => Local);
            }
        }



        public string Nombre
        {
            get { return NuevoEmpleado.firstName; }
            set { 
            if(NuevoEmpleado.firstName != value)
                {
                    NuevoEmpleado.firstName = value;
                }
                NotifyOfPropertyChange(() => Nombre);
            
            }
        }
        public string Apellidos
        {
            get { return NuevoEmpleado.lastName; }
            set
            {
                if (NuevoEmpleado.lastName != value)
                {
                    NuevoEmpleado.lastName = value;
                }
                NotifyOfPropertyChange(() => Apellidos);

            }
        }

        public string CC
        {
            get { return NuevoEmpleado.cedula; }
            set
            {
                if (NuevoEmpleado.cedula != value)
                {
                    NuevoEmpleado.cedula = value;
                }
                NotifyOfPropertyChange(() => CC);

            }
        }

        public string Telefono
        {
            get { return NuevoEmpleado.telefono; }
            set
            {
                if (NuevoEmpleado.telefono != value)
                {
                    NuevoEmpleado.telefono = value;
                }
                NotifyOfPropertyChange(() => Telefono);

            }
        }


        public DateTime FechaContratacion
        {
            get { return NuevoEmpleado.fechaDeContratacion; }
            set
            {
                if (NuevoEmpleado.fechaDeContratacion != value)
                {
                    NuevoEmpleado.fechaDeContratacion = value;
                }
                NotifyOfPropertyChange(() => FechaContratacion);

            }
        }

        public string Cargo
        {
            get { return NuevoEmpleado.cargo; }
            set
            {
                if (NuevoEmpleado.cargo != value)
                {
                    NuevoEmpleado.cargo = value;
                }
                NotifyOfPropertyChange(() => Cargo);

            }
        }

        private BindableCollection<string> _cargos = new BindableCollection<string>() {
            "Administrador de sede", 
            "Bodeguero",
            "Cajero",
            "Encargado de despacho",
            "Gerente general", 
            "Jefe de compras"
        };


        public BindableCollection<string> Cargos
        {
            get { return _cargos; }
            set { _cargos = value; }
        }


        public string Password
        {
            get { return NuevoEmpleado.password; }
            set
            {
                if (NuevoEmpleado.password != value)
                {
                    NuevoEmpleado.password = value;
                }
                NotifyOfPropertyChange(() => Password);

            }
        }

        public decimal? Salario
        {
            get { return NuevoEmpleado.salario; }
            set
            {
                if (NuevoEmpleado.salario != value)
                {
                    NuevoEmpleado.salario = value;
                }
                NotifyOfPropertyChange(() => Salario);

            }
        }

        public string Direccion
        {
            get { return NuevoEmpleado.direccion; }
            set
            {
                if (NuevoEmpleado.direccion != value)
                {
                    NuevoEmpleado.direccion = value;
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

        public async void Guardar()
        {
            if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
            {
                if (Statics.ClientStatus == "Trabajando localmente")
                {
                     if (!string.IsNullOrWhiteSpace(NuevoEmpleado.firstName) && 
                         !string.IsNullOrWhiteSpace(NuevoEmpleado.lastName) && 
                         !string.IsNullOrWhiteSpace(NuevoEmpleado.cedula) &&
                         !string.IsNullOrWhiteSpace(NuevoEmpleado.direccion) &&
                         !string.IsNullOrWhiteSpace(NuevoEmpleado.telefono) &&
                         !string.IsNullOrWhiteSpace(NuevoEmpleado.puntoDeVenta.nombre) &&
                         !string.IsNullOrWhiteSpace(NuevoEmpleado.fechaDeContratacion.ToString()) &&
                         !string.IsNullOrWhiteSpace(NuevoEmpleado.cargo) &&
                         !string.IsNullOrWhiteSpace(NuevoEmpleado.salario.ToString()) &&
                         !string.IsNullOrWhiteSpace(NuevoEmpleado.password) &&
                         !string.IsNullOrWhiteSpace(PasswordAgain))
                        {
                        try
                        {
                            Task<object> re = conexion.CallServerMethod("ServidorCreateNuevoUsuario", Arguments: new[] { NuevoEmpleado });
                            await re;
                            if (re.Result.ToString() == "Empleado ya registrado.")
                            {
                                MessageBox.Show("El número de cédula ya ha sido registrado.");
                                CC = "";
                                return;
                            }
                            else if (re.Result.ToString() == "Se ha registrado al nuevo usuario.")
                            {
                                MessageBox.Show("Se ha registrado al nuevo usuario.");
                                Locales.Clear();
                                VentanaPrincipal.ActivateItem(new NuevoUsuarioViewModel(VentanaPrincipal));
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
                        MessageBox.Show("Primero debe rellenar los datos.");

                     }
                 }
            }
            else
            {
                MessageBox.Show("Para agregar un nuevo usuario debe estar conectado al servidor.");
            }




           
        }
       
        public void BackButton()
        {
            Locales.Clear();
            VentanaPrincipal.ActivateItem(new GerenciaAdministrativoViewModel(VentanaPrincipal));
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
