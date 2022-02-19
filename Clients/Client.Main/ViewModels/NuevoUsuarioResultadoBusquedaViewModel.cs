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

        public string Local
        {
            get
            {
  

                foreach (LocalModel slocal in locales)
                {
                    if (slocal.codigo == resultadoEmpleado.puntoDeVenta.codigo)
                    {
                        resultadoEmpleado.puntoDeVenta.nombre = slocal.nombre;
                        return slocal.nombre;
                    }
                }

                //IEnumerator<LocalModel> e = l.GetEnumerator();
                //e.Reset();
                //while (e.MoveNext())
                //{
                //    if (e.Current.codigo == resultadoEmpleado.puntoDeVenta.codigo)
                //    {
                //        resultadoEmpleado.puntoDeVenta.nombre = e.Current.nombre;
                        
                //        return e.Current.nombre;
                //    }
                //}
                return "No coincidencias.";
                
                
            }
            set
            {
                if (value == null)
                    MessageBox.Show("Local pasando a null");
                resultadoEmpleado.puntoDeVenta.codigo = value;                
                NotifyOfPropertyChange(() => Local);
            }
        }

        BindableCollection<LocalModel> locales = new BindableCollection<LocalModel>();
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
                        locales.Add(item);
                    }
                    //locales = lcl;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
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

        public void BackButton()
        {
            
            VentanaPrincipal.ActivateItem(new BuscarUsuarioViewModel(VentanaPrincipal));
        }


        public void Editar()
        {
            VentanaPrincipal.ActivateItem(new NuevoUsuarioEditarBusquedaViewModel(VentanaPrincipal, resultadoEmpleado));
        }

        public void Eliminar()
        {
            MessageBoxResult result = MessageBox.Show($"Desea eliminar permanentemente de la base de datos al usuario {resultadoEmpleado.firstName} {resultadoEmpleado.lastName}", "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                if (DbConnection.deleteEmpleado(resultadoEmpleado.cedula))
                {
                    MessageBox.Show($"Se ha eliminado al usuario {resultadoEmpleado.firstName} {resultadoEmpleado.lastName}");
                    VentanaPrincipal.ActivateItem(new BuscarUsuarioViewModel(VentanaPrincipal));

                }
            }
            
        }
    }
}
