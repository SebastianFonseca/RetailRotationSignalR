using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main.ViewModels
{
    class LocalResultadoBusquedaViewModel : Screen
    {
        MainWindowViewModel VentanaPrincipal;

        LocalModel resultadoLocal = new LocalModel();
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();

        public LocalResultadoBusquedaViewModel(MainWindowViewModel argVentana, LocalModel resultadoBusqueda)
        {
            VentanaPrincipal = argVentana;
            resultadoLocal = resultadoBusqueda;
        }

        public string Nombre
        {
            get { return resultadoLocal.nombre; }
            set
            {
                if (resultadoLocal.nombre != value)
                {
                    resultadoLocal.nombre = value;
                }
                NotifyOfPropertyChange(() => Nombre);

            }
        }

        public string Direccion
        {
            get { return resultadoLocal.direccion; }
            set
            {
                if (resultadoLocal.direccion != value)
                {
                    resultadoLocal.direccion = value;
                }
                NotifyOfPropertyChange(() => Direccion);

            }
        }

        public string Telefono
        {
            get { return resultadoLocal.telefono; }
            set
            {
                if (resultadoLocal.telefono != value)
                {
                    resultadoLocal.telefono = value;
                }
                NotifyOfPropertyChange(() => Telefono);

            }
        }

        public string Ciudad
        {
            get { return resultadoLocal.ciudad; }
            set
            {
                if (resultadoLocal.ciudad != value)
                {
                    resultadoLocal.ciudad = value;
                }
                NotifyOfPropertyChange(() => Ciudad);

            }
        }

        public int NumeroDeCanastillas
        {
            get { return resultadoLocal.numeroDeCanastillas; }
            set
            {
                if (resultadoLocal.numeroDeCanastillas != value)
                {
                    resultadoLocal.numeroDeCanastillas = value;
                }
                NotifyOfPropertyChange(() => NumeroDeCanastillas);

            }
        }

        public string FechaDeApertura
        {
            get { return resultadoLocal.fechaDeApertura.ToShortDateString(); }

        }

        private EmpleadoModel _admin;

        public EmpleadoModel Admin
        {
            get { return _admin; }
            set
            {
                //resultadoLocal.Administrador = value;
                _admin = value;
                NotifyOfPropertyChange(() => Admin);
            }
        }

        public void Editar()
        {
            VentanaPrincipal.ActivateItem(new LocalEditarBusquedaViewModel(VentanaPrincipal, resultadoLocal));

        }
        public async void Eliminar()
        {
            if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
            {
                try
                {
                     MessageBoxResult result = MessageBox.Show($"Desea eliminar permanentemente de la base de datos el local {resultadoLocal.codigo} {resultadoLocal.nombre}", "", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                         Task<object> re = conexion.CallServerMethod("ServidorEliminarLocal", Arguments: new[] { resultadoLocal.codigo, resultadoLocal.nombre });
                         await re;
                         object[] mn = System.Text.Json.JsonSerializer.Deserialize<object[]>(re.Result.ToString());

                        if (mn[0].ToString() == "Local con empleados.")
                        {
                            BindableCollection<EmpleadoModel> empleados_local = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<EmpleadoModel>>(mn[1].ToString());
                            string empleados = "Los siguientes empleados estan asignados a este local, debe eliminarlos o actualizar el local al que pertenecen antes de continuar con esta acción:\n";
                            foreach (EmpleadoModel empleado in empleados_local)
                            {
                                empleados = empleados +" " + empleado.cedula + " - " + empleado.firstName + " " + empleado.lastName + ".\n";
                            }
                            MessageBox.Show(empleados);
                            return;
                        }

                        else if (mn[0].ToString() == "Local eliminado")
                        {
                            MessageBox.Show($"Se ha eliminado al local {resultadoLocal.codigo} {resultadoLocal.nombre}");
                            VentanaPrincipal.ActivateItem(new LocalBuscarViewModel(VentanaPrincipal));
                            return;
                        }
                        else
                        {
                            MessageBox.Show(re.Result.ToString());
                            return;
                        }


                        //if (DbConnection.deleteLocal(resultadoLocal.codigo))
                        //{
                        //    MessageBox.Show($"Se ha eliminado al usuario {resultadoLocal.codigo} {resultadoLocal.nombre}");
                        //    VentanaPrincipal.ActivateItem(new LocalBuscarViewModel(VentanaPrincipal));

                        //}
                    }
                }
                catch (Exception e)
                {

                    MessageBox.Show(e.Message);
                }
            }
            else
            {
                MessageBox.Show("Para eliminar un local debe estar conectado al servidor.");
            }

        }

        public void BackButton()
        {

            VentanaPrincipal.ActivateItem(new LocalBuscarViewModel(VentanaPrincipal));

        }
    }
}