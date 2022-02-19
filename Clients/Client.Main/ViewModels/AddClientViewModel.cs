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
    public class AddClientViewModel : PropertyChangedBase, IDataErrorInfo
    {
        ClientesModel NuevoCliente;
        MainWindowViewModel VentanaPrincipal;

        public AddClientViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
            NuevoCliente = new ClientesModel();
            conexion.Connection.On("ClienteExiste", handler: (string a) =>
            {

                MessageBox.Show(a);
                CC = "";

            });
        }

       
        public string Name
        {
            get { return NuevoCliente.firstName; }
            set{
                if (NuevoCliente.firstName != value)
                    NuevoCliente.firstName = value;
                NotifyOfPropertyChange(() => Name);
            }
        }


        public string Apellidos
        {
            get { return NuevoCliente.lastName; }
            set
            {               
                if (NuevoCliente.lastName != value)
                    NuevoCliente.lastName = value;
                NotifyOfPropertyChange(() => Apellidos);
            }
        }

        public string CC
        {
            get { return NuevoCliente.cedula; }
            set
            {     
                if (NuevoCliente.cedula != value)
                    NuevoCliente.cedula = value;
                NotifyOfPropertyChange(() => CC);
            }
        }

        public string Correo
        {
            get { return NuevoCliente.correo; }
            set
            {
                if (NuevoCliente.correo != value)
                    NuevoCliente.correo = value;
                NotifyOfPropertyChange(() => Correo);
            }
        }

        public string Telefono
        {
            get { return NuevoCliente.telefono; }
            set
            {
                if (NuevoCliente.telefono != value)
                    NuevoCliente.telefono = value;
                NotifyOfPropertyChange(() => Telefono);

            }
        }


        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();

        public async void Guardar()
        {

            //DbConnection.SincronizarReplicacionMerge();
            if (!string.IsNullOrWhiteSpace(NuevoCliente.firstName) && !string.IsNullOrWhiteSpace(NuevoCliente.lastName) && !string.IsNullOrWhiteSpace(NuevoCliente.cedula))
            {
                if (string.IsNullOrEmpty(NuevoCliente.correo))
                {
                    NuevoCliente.correo = null;
                }
                if (string.IsNullOrEmpty(NuevoCliente.telefono))
                {
                    NuevoCliente.telefono = null;
                }

                try
                {
                    if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                    {
                        Task<object> re = conexion.CallServerMethod("ServidorAddClient", Arguments: new[] { NuevoCliente });
                        await re;
                        if (Convert.ToInt32(re.Result.ToString()) == 1)
                        {
                            MessageBox.Show($"El cliente {NuevoCliente.firstName} {NuevoCliente.lastName} se ha registrado en el servidor con 100 puntos.");
                            VentanaPrincipal.ActivateItem(new AddClientViewModel(VentanaPrincipal));
                            return;
                        }
                        if (Convert.ToInt32(re.Result.ToString()) == 0)
                        {
                            MessageBox.Show($"El cliente {NuevoCliente.firstName} {NuevoCliente.lastName} ya esta registrado.");
                            CC = "";
                            return;
                        }
                    }

                    if (DbConnection.AddClient(Cliente: NuevoCliente))
                    {
                        MessageBox.Show($"El cliente {NuevoCliente.firstName} {NuevoCliente.lastName} se ha registrado localmente con 100 puntos.");
                        VentanaPrincipal.ActivateItem(new AddClientViewModel(VentanaPrincipal));
                    }
                    else
                    {
                        CC = "";
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
                    else if(name == "Name")
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


