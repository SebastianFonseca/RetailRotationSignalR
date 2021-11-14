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
            CedulaAntigua = resultadoCliente.Cedula; 
        }

        public string Name
        {
            get { return resultadoCliente.FirstName; }
            set
            {
                if (resultadoCliente.FirstName != value)
                    resultadoCliente.FirstName = value;
                NotifyOfPropertyChange(() => Name);
            }
        }


        public string Apellidos
        {
            get { return resultadoCliente.LastName; }
            set
            {
                if (resultadoCliente.LastName != value)
                    resultadoCliente.LastName = value;
                NotifyOfPropertyChange(() => Apellidos);
            }
        }

        public string CC
        {
            get { return resultadoCliente.Cedula; }
            set
            {
                if (resultadoCliente.Cedula != value)
                    resultadoCliente.Cedula = value;
                NotifyOfPropertyChange(() => CC);
            }
        }

        public string Correo
        {
            get { return resultadoCliente.Correo; }
            set
            {
                if (resultadoCliente.Correo != value)
                    resultadoCliente.Correo = value;
                NotifyOfPropertyChange(() => Correo);
            }
        }

        public string Telefono
        {
            get { return resultadoCliente.Telefono; }
            set
            {
                if (resultadoCliente.Telefono != value)
                    resultadoCliente.Telefono = value;
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
            if (!string.IsNullOrWhiteSpace(resultadoCliente.FirstName) && !string.IsNullOrWhiteSpace(resultadoCliente.LastName) && !string.IsNullOrWhiteSpace(resultadoCliente.Cedula))
            {
                if (string.IsNullOrEmpty(resultadoCliente.Correo))
                {
                    resultadoCliente.Correo = null;
                }
                if (string.IsNullOrEmpty(resultadoCliente.Telefono))
                {
                    resultadoCliente.Telefono = null;
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
                        MessageBox.Show($"Se ha editado la informacion del cliente {resultadoCliente.FirstName} {resultadoCliente.LastName}. ");
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
