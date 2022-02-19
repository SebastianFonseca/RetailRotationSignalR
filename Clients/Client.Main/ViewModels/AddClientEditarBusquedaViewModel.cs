using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace Client.Main.ViewModels
{
    class AddClientEditarBusquedaViewModel : PropertyChangedBase, IDataErrorInfo
    {

        MainWindowViewModel VentanaPrincipal;

        ClientesModel resultadoCliente = new ClientesModel();
        string CedulaAntigua;

        public AddClientEditarBusquedaViewModel(MainWindowViewModel argVentana, ClientesModel resultadoBusqueda)
        {
            VentanaPrincipal = argVentana;
            resultadoCliente = resultadoBusqueda;
            CedulaAntigua = resultadoCliente.cedula; 
        }

        public string Name
        {
            get { return resultadoCliente.firstName; }
            set
            {
                if (resultadoCliente.firstName != value)
                    resultadoCliente.firstName = value;
                NotifyOfPropertyChange(() => Name);
            }
        }


        public string Apellidos
        {
            get { return resultadoCliente.lastName; }
            set
            {
                if (resultadoCliente.lastName != value)
                    resultadoCliente.lastName = value;
                NotifyOfPropertyChange(() => Apellidos);
            }
        }

        public string CC
        {
            get { return resultadoCliente.cedula; }
            set
            {
                if (resultadoCliente.cedula != value)
                    resultadoCliente.cedula = value;
                NotifyOfPropertyChange(() => CC);
            }
        }

        public string Correo
        {
            get { return resultadoCliente.correo; }
            set
            {
                if (resultadoCliente.correo != value)
                    resultadoCliente.correo = value;
                NotifyOfPropertyChange(() => Correo);
            }
        }

        public string Telefono
        {
            get { return resultadoCliente.telefono; }
            set
            {
                if (resultadoCliente.telefono != value)
                    resultadoCliente.telefono = value;
                NotifyOfPropertyChange(() => Telefono);

            }
        }


        public int Puntos
        {
            get { return resultadoCliente.Puntos; }
            set
            {
                if (resultadoCliente.Puntos != value)
                    resultadoCliente.Puntos = value;
                NotifyOfPropertyChange(() => Telefono);

            }
        }

        public void BackButton()
        {
            VentanaPrincipal.ActivateItem(new AddClientBuscarViewModel(VentanaPrincipal));
        }

        public void Editar()
        {
            if (!string.IsNullOrWhiteSpace(resultadoCliente.firstName) && !string.IsNullOrWhiteSpace(resultadoCliente.lastName) && !string.IsNullOrWhiteSpace(resultadoCliente.cedula))
            {
                if (string.IsNullOrEmpty(resultadoCliente.correo))
                {
                    resultadoCliente.correo = null;
                }
                if (string.IsNullOrEmpty(resultadoCliente.telefono))
                {
                    resultadoCliente.telefono = null;
                }

                try
                {
                    //if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                    //{
                    //    Task<object> re = conexion.CallServerMethod("ServidorAddClient", Arguments: new[] { resultadoCliente });
                    //    await re;
                    //    if (Convert.ToInt32(re.Result.ToString()) == 1)
                    //    {
                    //        MessageBox.Show($"El cliente {resultadoCliente.FirstName} {resultadoCliente.LastName} se ha registrado en el servidor con 100 puntos.");
                    //        VentanaPrincipal.ActivateItem(new AddClientViewModel(VentanaPrincipal));
                    //        return;
                    //    }
                    //    if (Convert.ToInt32(re.Result.ToString()) == 0)
                    //    {
                    //        MessageBox.Show($"El cliente {resultadoCliente.FirstName} {resultadoCliente.LastName} ya esta registrado.");
                    //        CC = "";
                    //        return;
                    //    }
                    //}

                    if (DbConnection.ActualizarCliente(Cliente:resultadoCliente, CC:CedulaAntigua))
                    { 
                        MessageBox.Show($"Se ha editado la informacion del cliente {resultadoCliente.firstName} {resultadoCliente.lastName}. ");
                        VentanaPrincipal.ActivateItem(new AddClientBuscarViewModel(VentanaPrincipal));
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
