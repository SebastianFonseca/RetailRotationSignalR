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
    class LocalEditarBusquedaViewModel : Screen, IDataErrorInfo
    {
        MainWindowViewModel VentanaPrincipal;
        LocalModel resultadoLocal = new LocalModel();
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        string CodigoAntiguo;

        //public static BindableCollection<EmpleadoModel> _administradores = new BindableCollection<EmpleadoModel>();
        //public BindableCollection<EmpleadoModel> Administradores
        //{
        //    get
        //    {
        //        _administradores = DbConnection.getAdministradores();
        //        return _administradores;
        //    }
        //    set { Administradores = value; }
        //}

        public LocalEditarBusquedaViewModel(MainWindowViewModel argVentana, LocalModel local)
        {
            resultadoLocal = local;
            CodigoAntiguo = local.codigo;
            VentanaPrincipal = argVentana;
        }



        public string Nombre
        {
            get { return resultadoLocal.nombre; }
            set
            {
                if (decimal.TryParse(value, out decimal a)) { MessageBox.Show("Los caracteres no pueden contener solo números"); }
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

        public DateTime FechaDeApertura
        {
            get { return resultadoLocal.fechaDeApertura; }
            set
            {
                if (resultadoLocal.fechaDeApertura != value)
                {
                    resultadoLocal.fechaDeApertura = value;
                }
                NotifyOfPropertyChange(() => FechaDeApertura);

            }
        }



        public async void Editar()
        {
                if (!string.IsNullOrWhiteSpace(resultadoLocal.nombre) &&
                    !string.IsNullOrWhiteSpace(resultadoLocal.direccion) &&
                    !string.IsNullOrWhiteSpace(resultadoLocal.telefono) &&
                    !string.IsNullOrWhiteSpace(resultadoLocal.ciudad) &&
                    !string.IsNullOrWhiteSpace(resultadoLocal.numeroDeCanastillas.ToString()) &&
                    !string.IsNullOrWhiteSpace(resultadoLocal.fechaDeApertura.ToString("DD-MM-YYYY")))
                {
                    try
                    {
                        if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                        {
                            MessageBoxResult result = MessageBox.Show($"Desea editar la informacion en la base de datos del local {resultadoLocal.codigo} {resultadoLocal.nombre}", "", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                Task<object> re = conexion.CallServerMethod("ServidorActualizarLocal", Arguments: new object[] { resultadoLocal, CodigoAntiguo });
                                await re;
                                if (re.Result.ToString() == "El local se ha actualizado.")
                                {
                                    MessageBox.Show($"Se ha editado la informacion del local {resultadoLocal.codigo} {resultadoLocal.nombre}");
                                    VentanaPrincipal.ActivateItem(new LocalResultadoBusquedaViewModel(VentanaPrincipal, resultadoLocal));
                                }
                                else
                                {
                                    MessageBox.Show(re.Result.ToString());
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Para editar un local debe estar conectado al servidor.");
                        }




                        //if (DbConnection.ActualizarLocal(Local: resultadoLocal, IdAnterior: CodigoAntiguo))
                        //{
                        //    _administradores.Clear();
                        //    VentanaPrincipal.ActivateItem(new LocalBuscarViewModel(VentanaPrincipal));

                        //}


                    }
                    catch (Exception e)
                    {

                        MessageBox.Show(e.Message);
                    }





                    //if (DbConnection.ActualizarLocal(Local: resultadoLocal, IdAnterior: CodigoAntiguo))
                    //{
                    //    _administradores.Clear();
                    //    VentanaPrincipal.ActivateItem(new LocalBuscarViewModel(VentanaPrincipal));

                    //}
                }
                else
                {
                    MessageBox.Show("Primero debe rellenar los datos.");

                }



        }


        public void BackButton()
        {
            //_administradores.Clear();
            VentanaPrincipal.ActivateItem(new LocalResultadoBusquedaViewModel(VentanaPrincipal, resultadoLocal));
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


                }

                else { flag += 1; }


                return result;
            }
        }
    }
}
