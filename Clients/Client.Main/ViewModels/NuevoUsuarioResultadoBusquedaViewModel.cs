using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Autofac;
using System.Threading.Tasks;

namespace Client.Main.ViewModels
{
    class NuevoUsuarioResultadoBusquedaViewModel : PropertyChangedBase
    {

        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();

        EmpleadoModel resultadoEmpleado = new EmpleadoModel();

        public NuevoUsuarioResultadoBusquedaViewModel(MainWindowViewModel argVentana, EmpleadoModel resultadoBusqueda)
        {

            VentanaPrincipal = argVentana;
            resultadoEmpleado = resultadoBusqueda;
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

                    foreach (LocalModel item in System.Text.Json.JsonSerializer.Deserialize<LocalModel[]>(re.Result.ToString()))
                    {
                        if (item.codigo == resultadoEmpleado.puntoDeVenta.codigo)
                        {
                            resultadoEmpleado.puntoDeVenta.nombre = item.nombre;
                            Local = item.nombre;
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        public string Local
        {
            get
            { 
                return resultadoEmpleado.puntoDeVenta.nombre;                               
            }
            set
            {
                if (value == null)
                    MessageBox.Show("Local pasando a null");
                resultadoEmpleado.puntoDeVenta.codigo = value;                
                NotifyOfPropertyChange(() => Local);
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

        public string Cargo
        {
            get
            {
                //Cuando se edita un usaurio se muestra la nueva info, pero el cargo no funciona.
                if (resultadoEmpleado.cargo.StartsWith("S"))
                {
                    return resultadoEmpleado.cargo.Substring(37);
                }

                return resultadoEmpleado.cargo;
            }
            set
            {
                if (resultadoEmpleado.cargo != value)
                {
                    resultadoEmpleado.cargo = value;
                }
                NotifyOfPropertyChange(() => Cargo);

            }
        }

        public void BackButton()
        {
            
            VentanaPrincipal.ActivateItem(new BuscarUsuarioViewModel(VentanaPrincipal));
        }


        public void Editar()
        {
            VentanaPrincipal.ActivateItem(new NuevoUsuarioEditarBusquedaViewModel(VentanaPrincipal, resultadoEmpleado));
        }

        public async void Eliminar()
        {
            if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
            {
                MessageBoxResult result = MessageBox.Show($"Desea eliminar permanentemente de la base de datos el usuario {resultadoEmpleado.cedula} - {resultadoEmpleado.firstName} {resultadoEmpleado.lastName}", "", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    Task<object> re = conexion.CallServerMethod("ServidorDeleteUsuario", Arguments: new[] { resultadoEmpleado.cedula });
                    await re;

                    if (re.Result.ToString() == "Se ha eliminado al usuario.")
                    {
                        MessageBox.Show($"Se ha eliminado al usuario {resultadoEmpleado.cedula} - {resultadoEmpleado.firstName} {resultadoEmpleado.lastName}");
                        VentanaPrincipal.ActivateItem(new LocalBuscarViewModel(VentanaPrincipal));
                    }
                    else
                    {
                        MessageBox.Show(re.Result.ToString());
                    }


                    //if (DbConnection.deleteLocal(resultadoLocal.codigo))
                    //{
                    //    MessageBox.Show($"Se ha eliminado al usuario {resultadoLocal.codigo} {resultadoLocal.nombre}");
                    //    VentanaPrincipal.ActivateItem(new LocalBuscarViewModel(VentanaPrincipal));

                    //}
                }
            }
            else
            {
                MessageBox.Show("Para eliminar un usuario debe estar conectado al servidor.");

            }


            //MessageBoxResult result = MessageBox.Show($"Desea eliminar permanentemente de la base de datos al usuario {resultadoEmpleado.firstName} {resultadoEmpleado.lastName}", "", MessageBoxButton.YesNo);
            //if (result == MessageBoxResult.Yes)
            //{
            //    if (DbConnection.deleteEmpleado(resultadoEmpleado.cedula))
            //    {
            //        MessageBox.Show($"Se ha eliminado al usuario {resultadoEmpleado.firstName} {resultadoEmpleado.lastName}");
            //        VentanaPrincipal.ActivateItem(new BuscarUsuarioViewModel(VentanaPrincipal));

            //    }
            //}
            
        }
    }
}
