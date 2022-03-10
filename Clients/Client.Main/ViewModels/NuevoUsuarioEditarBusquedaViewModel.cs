using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace Client.Main.ViewModels
{
    class NuevoUsuarioEditarBusquedaViewModel : PropertyChangedBase, IDataErrorInfo
    {
        MainWindowViewModel VentanaPrincipal;

        EmpleadoModel resultadoEmpleado = new EmpleadoModel();
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        string CedulaAntigua;

        public NuevoUsuarioEditarBusquedaViewModel(MainWindowViewModel argVentana, EmpleadoModel resultadoBusqueda)
        {
            VentanaPrincipal = argVentana;
            resultadoEmpleado = resultadoBusqueda;
            CedulaAntigua = resultadoEmpleado.cedula;
            getLocalesServidor();
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

        private BindableCollection<LocalModel> _locales = new BindableCollection<LocalModel>();
        public BindableCollection<LocalModel> Locales
        {
            get
            {

                return _locales;
                //return DbConnection.getLocales();
            }
            set
            {

                _locales = value;
                NotifyOfPropertyChange(() => Locales);
            }

        }
        public LocalModel Local
        {
            get
            {
                return resultadoEmpleado.puntoDeVenta;
            }
            set                
            {
                if (value != null)
                {
                    resultadoEmpleado.puntoDeVenta = value;
                    NotifyOfPropertyChange(() => Local);
                }                
            }
        }
        public string Nombre
        {
            get { return resultadoEmpleado.firstName; }
            set
            {
                if (resultadoEmpleado.firstName != value)
                {
                    resultadoEmpleado.firstName = value;
                }
                NotifyOfPropertyChange(() => Nombre);

            }
        }
        public string Apellidos
        {
            get { return resultadoEmpleado.lastName; }
            set
            {
                if (resultadoEmpleado.lastName != value)
                {
                    resultadoEmpleado.lastName = value;
                }
                NotifyOfPropertyChange(() => Apellidos);

            }
        }
        public string CC
        {
            get { return resultadoEmpleado.cedula; }
            set
            {
                if (resultadoEmpleado.cedula != value)
                {
                    resultadoEmpleado.cedula = value;
                }
                NotifyOfPropertyChange(() => CC);

            }
        }
        public string Telefono
        {
            get { return resultadoEmpleado.telefono; }
            set
            {
                if (resultadoEmpleado.telefono != value)
                {
                    resultadoEmpleado.telefono = value;
                }
                NotifyOfPropertyChange(() => Telefono);

            }
        }
        public DateTime FechaContratacion
        {
            get { return resultadoEmpleado.fechaDeContratacion; }
            set
            {
                if (resultadoEmpleado.fechaDeContratacion != value)
                {
                    resultadoEmpleado.fechaDeContratacion = value;
                }
                NotifyOfPropertyChange(() => FechaContratacion);

            }
        }
        public string Cargo
        {
            get { return resultadoEmpleado.cargo; }
            set
            {
                //MessageBox.Show( value.Length.ToString());
                if (resultadoEmpleado.cargo != value)
                {
                    resultadoEmpleado.cargo = value; 
                }
                NotifyOfPropertyChange(() => Cargo);

            }
        }
        public string Password
        {
            get { return resultadoEmpleado.password; }
            set
            {
                if (resultadoEmpleado.password != value)
                {
                    resultadoEmpleado.password = value;
                }
                NotifyOfPropertyChange(() => Password);

            }
        }
        public decimal Salario
        {
            get { return resultadoEmpleado.salario; }
            set
           {
                if (resultadoEmpleado.salario != value)
                {
                    resultadoEmpleado.salario = value;
                }
                NotifyOfPropertyChange(() => Salario);

            }
        }
        public string Direccion
        {
            get { return resultadoEmpleado.direccion; }
            set
            {
                if (resultadoEmpleado.direccion != value)
                {
                    resultadoEmpleado.direccion = value;
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
        public async void Actualizar()
        {

                if (!string.IsNullOrWhiteSpace(resultadoEmpleado.firstName) &&
                    !string.IsNullOrWhiteSpace(resultadoEmpleado.lastName) &&
                    !string.IsNullOrWhiteSpace(resultadoEmpleado.cedula) &&
                    !string.IsNullOrWhiteSpace(resultadoEmpleado.direccion) &&
                    !string.IsNullOrWhiteSpace(resultadoEmpleado.telefono) &&
                    !string.IsNullOrWhiteSpace(resultadoEmpleado.puntoDeVenta.nombre) &&
                    !string.IsNullOrWhiteSpace(resultadoEmpleado.fechaDeContratacion.ToString()) &&
                    !string.IsNullOrWhiteSpace(resultadoEmpleado.cargo) &&
                    !string.IsNullOrWhiteSpace(resultadoEmpleado.salario.ToString()) &&
                    !string.IsNullOrWhiteSpace(resultadoEmpleado.password) &&
                    !string.IsNullOrWhiteSpace(PasswordAgain))
                {
                try
                {
                    if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                    {
                        MessageBoxResult result = MessageBox.Show($"Desea editar la informacion en la base de datos del usuario {resultadoEmpleado.cedula} - {resultadoEmpleado.firstName} {resultadoEmpleado.lastName}.", "", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            Task<object> re = conexion.CallServerMethod("ServidorUpdateUsuario", Arguments: new object[] { resultadoEmpleado, CedulaAntigua });
                            await re;
                            if (re.Result.ToString() == "Usuario actualizado")
                            {
                                resultadoEmpleado.password = "";
                                resultadoEmpleado.cargo = Cargo;
                                VentanaPrincipal.ActivateItem(new NuevoUsuarioResultadoBusquedaViewModel(VentanaPrincipal, resultadoEmpleado));
                            }
                            else
                            {
                                MessageBox.Show(re.Result.ToString());
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Para editar un usuario debe estar conectado al servidor.");

                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }






                //if (DbConnection.ActualizarUsuario(Empleado: resultadoEmpleado, CC: CedulaAntigua))
                //{
                //    resultadoEmpleado.password = "";
                //    VentanaPrincipal.ActivateItem(new NuevoUsuarioResultadoBusquedaViewModel(VentanaPrincipal, resultadoEmpleado));
                //}
                //else
                //{
                //    CC = "";
                //}
            }
                else
                {
                    MessageBox.Show("Primero debe rellenar los datos.");

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
