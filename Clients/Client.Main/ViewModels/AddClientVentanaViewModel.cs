using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Client.Main.Utilities;
using Client.Main.Models;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Autofac;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
namespace Client.Main.ViewModels
{
    public class AddClientVentanaViewModel: Screen
    {
        public ClientesModel cliente = ContainerConfig.scope.Resolve<ClientesModel>();
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();

        public AddClientVentanaViewModel(MainWindowViewModel argVentana)
        {

            VentanaPrincipal = argVentana;
            cliente.puntos = 100;
            conexion.Connection.On("ClienteExiste", handler: (string a) =>
            {

                MessageBox.Show(a);
                CC = "";

            });
        }

        public string Name
        {
            get { return cliente.firstName; }
            set
            {
                if (cliente.firstName != value)
                    cliente.firstName = value;
                NotifyOfPropertyChange(() => Name);
            }
        }


        public string Apellidos
        {
            get { return cliente.lastName; }
            set
            {
                if (cliente.lastName != value)
                    cliente.lastName = value;
                NotifyOfPropertyChange(() => Apellidos);
            }
        }

        public string CC
        {
            get { return cliente.cedula; }
            set
            {
                if (Int32.TryParse(value, out int resul))
                {
                    if (cliente.cedula != value)
                        cliente.cedula = value;
                    NotifyOfPropertyChange(() => CC);
                }

            }
        }

        public string Correo
        {
            get { return cliente.correo; }
            set
            {
                if (cliente.correo != value)
                    cliente.correo = value;
                NotifyOfPropertyChange(() => Correo);
            }
        }

        public string Telefono
        {
            get { return cliente.telefono; }
            set
            {
                if (Int32.TryParse(value, out int resul))
                { 
                    if (cliente.telefono != value)
                        cliente.telefono = value;
                    NotifyOfPropertyChange(() => Telefono);
                }
            }
        }

        public async void Guardar()
        {

            //DbConnection.SincronizarReplicacionMerge();
            if (!string.IsNullOrWhiteSpace(cliente.firstName) && !string.IsNullOrWhiteSpace(cliente.lastName) && !string.IsNullOrWhiteSpace(cliente.cedula))
            {
                if (string.IsNullOrEmpty(cliente.correo))
                {
                    cliente.correo = null;
                }
                if (string.IsNullOrEmpty(cliente.telefono))
                {
                    cliente.telefono = null;
                }

                try
                {
                    if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                    {
                        Task<object> re = conexion.CallServerMethod("ServidorAddClient", Arguments: new[] { cliente });
                        await re;
                        if (re.Result.ToString() == "true")
                        {
                            MessageBox.Show($"El cliente {cliente.firstName} {cliente.lastName} se ha registrado en el servidor con 100 puntos.");
                            this.TryClose();
                            return;
                        }
                        if (re.Result.ToString() == "Cliente ya existe")
                        {
                            MessageBox.Show($"El cliente {cliente.firstName} {cliente.lastName} ya esta registrado.");
                            CC = "";
                            return;
                        }
                    }
                    else if (MainWindowViewModel.Status == "Trabajando localmente")
                    {
                        if (DbConnection.AddClient(Cliente: cliente))
                        {
                            MessageBox.Show($"El cliente {cliente.firstName} {cliente.lastName} se ha registrado localmente con 100 puntos.");
                            this.TryClose();
                        }
                        else
                        {
                            CC = "";
                        }
                    }

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

        public string Error { get { return null; } }
        int flag = 0;
        public string this[string name]
        {
            get
            {
                string result = null;
                if (flag == 5)
                {
                    if (name == "CC")
                    {
                        if (String.IsNullOrEmpty(CC))
                        {
                            result = "Rellene este campo.";
                        }
                    }
                    else if (name == "Name")
                    {
                        if (String.IsNullOrEmpty(Name))
                        {
                            result = "Rellene este campo.";
                        }
                    }
                    else if (name == "Apellidos")
                    {
                        if (String.IsNullOrEmpty(Apellidos))
                        {
                            result = "Rellene este campo.";
                        }

                    }
                    else if (name == "Telefono")
                    {
                        if (Telefono.Length != 10)
                        {
                            result = "El número telefónico debe tener 10 digitos.";
                        }
                        if (String.IsNullOrEmpty(Telefono))
                        {
                            return null;
                        }

                    }
                }

                else { flag += 1; }


                return result;

            }
        }
    }
}
