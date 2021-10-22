using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Client.Main.ViewModels
{
    class NuevoUsuarioResultadoBusquedaViewModel : PropertyChangedBase
    {

        MainWindowViewModel VentanaPrincipal;

        EmpleadoModel resultadoEmpleado = new EmpleadoModel();

        public NuevoUsuarioResultadoBusquedaViewModel(MainWindowViewModel argVentana, EmpleadoModel resultadoBusqueda)
        {
            VentanaPrincipal = argVentana;
            resultadoEmpleado = resultadoBusqueda;
        }

        public string Local
        {
            get
            {
                IEnumerator<LocalModel> e = DbConnection.getLocales().GetEnumerator();
                e.Reset();
                while (e.MoveNext())
                {
                    if (e.Current.Codigo == resultadoEmpleado.PuntoDeVenta.Codigo)
                    {
                        resultadoEmpleado.PuntoDeVenta.Nombre = e.Current.Nombre;
                        
                        return e.Current.Nombre;
                    }
                }
                return "";
                
                
            }
            set
            {
                if (value == null)
                    MessageBox.Show("Local pasando a null");
                resultadoEmpleado.PuntoDeVenta.Codigo = value;                
                NotifyOfPropertyChange(() => Local);
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

        public string Salario
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

        public void BackButton()
        {
            
            VentanaPrincipal.ActivateItem(new BuscarUsuarioViewModel(VentanaPrincipal));
        }


        public void Editar()
        {
            VentanaPrincipal.ActivateItem(new NuevoUsuarioEditarBusquedaViewModel(VentanaPrincipal, resultadoEmpleado));
        }


    }
}
