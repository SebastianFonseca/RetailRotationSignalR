using Autofac;
using Autofac.Core;
using Autofac.Core.Lifetime;
using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using Client.Main.Views;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Composition;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main.ViewModels
{
    public class ShellViewModel : Conductor<object>, IDataErrorInfo
    {
        ///Objeto responsable de la administracion de las ventanas.
        private readonly IWindowManager window = new WindowManager();
        ///Clase responsable de la conexion al servidor obteniendo la instanca unica (Autofac) 
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        ///Propiedades enlazadas con el textbox y el passwordbox  de la vista.
        private static string _user;
        public  string User
        { 
            get=> _user;
            set
            {
                _user = value;
                NotifyOfPropertyChange(() => User);
            }
        }



        private string _password;
        public string UserPassword
        {
            get => _password;
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => UserPassword);
            }
        }

        private string _stackVisibility = "Collapsed";
        public string StackVisibility
        {
            get { return _stackVisibility; }
            set {
                _stackVisibility = value;
                NotifyOfPropertyChange(() => StackVisibility);
            }
        }

        string Status_conexion = "Trabajando localmente";


        public ShellViewModel()
        {

        }

        bool flag2 = true;

        ///variable de clase neccesaria para abrir la ventana principal.

        /// <summary>
        /// Fired event when 'Entrar' button is pressed.
        /// </summary>

        //public MainWindowViewModel VentanaPricipal = ContainerConfig.scope.Resolve<MainWindowViewModel>();
       // public MainWindowViewModel VentanaPricipal = ContainerConfig.scope.Resolve<MainWindowViewModel>();


        public async void Entrar()
        {
            
            try
            {
                StackVisibility = "Visible";
                ///Vefificar que los campos no esten vacios.
                if (!string.IsNullOrWhiteSpace(User) && !string.IsNullOrWhiteSpace(UserPassword))
                {
                    ///Si ya hubo un intento de logueo y esta conectado al servidor.
                    if (Status_conexion == "Conectado al servidor")
                    {
                        if (conexion.Connection != null & conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected)
                        {
                            Task<object> re = conexion.CallServerMethod("ServidorValidarUsuario", Arguments: new[] { User, UserPassword });
                            await re;
                            if (Convert.ToBoolean(re.Result.ToString()) == true)
                            {
                                window.ShowWindow(new MainWindowViewModel(User, Status_conexion));
                                this.TryClose();
                                return;
                            }
                            else
                            {
                                MessageBox.Show("El nombre de usuario o la contraseña son incorrectos.");
                            }
                        }
                    }
                    ///Para intentar loguarse por pimera vez y conectarse al servidor.
                    //else
                   // {


                    await Connect.ConnectToServer(User, "Admin") ;
                    if (flag2)
                    {
                        conexion.Connection.On("SetStatus", handler: (string a) =>
                        {
                            StackVisibility = "Collapsed";
                            Status_conexion = "Conectado al servidor";
                            MessageBox.Show(a);

                        });
                        flag2 = false;
                    }

                    await conexion.CallServerMethod("TestMethod", Arguments: new[] { "Conectado al sevidor." });                              
                    if (Status_conexion == "Conectado al servidor" & conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected)
                    {
                        Task<object> re = conexion.CallServerMethod("ServidorValidarUsuario", Arguments: new[] { User, UserPassword });
                        await re;
                        if (Convert.ToBoolean(re.Result.ToString()) == true)
                        {
                            window.ShowWindow(new MainWindowViewModel(User, Status_conexion));
                            this.TryClose();
                            return;
                        }
                        else
                        {
                            MessageBox.Show("El nombre de usuario o la contraseña son incorrectos.");
                        }
                    }
                        //else
                        //{

                    StackVisibility = "Collapsed";
                    if (DbConnection.Login(User: User, Password: UserPassword))
                    {
                        window.ShowWindow(new MainWindowViewModel(User, Status_conexion));
                        this.TryClose();
                    }
                    else
                    {
                        MessageBox.Show("Usuario o contraseña incorrectas.");
                    }
                      //  }
                    //}
                }
                // Si alguno de los campos no se ha llenado.
                else
                {
                    MessageBox.Show("Debe llenar todos los campos.");
                }
            }
            catch (Exception e)
            { MessageBox.Show(e.Message); }

        }

        ///Codigo necesario para la validacion de los datos ingresados en las cajas de texto del formulario.
        public string Error { get { return null; } }
        int flag = 0;
        public string this[string name]
        {
            get
            {
                string result = null;
                if (flag == 3)
                {
                    if (name == "User")
                    {
                        if (String.IsNullOrEmpty(User))
                        {
                            result = "Este campo no puede estar vacío.";
                        }
                    }
                    else if (name == "UserPassword")
                    {
                        if (String.IsNullOrEmpty(UserPassword))
                        {
                            result = "Este campo no puede estar vacío.";
                        }
                    }
                 
                }
                else { flag += 1; }
                return result;
            }
        }

    }
}

