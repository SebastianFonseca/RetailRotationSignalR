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

        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();

        ///Propiedades enlazadas con el textbox y el passwordbox  de la vista.
        private string _user;
        public string User { 
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
        public ShellViewModel()
        {
        }

        /// <summary>
        /// Fired event when 'Entrar' button is pressed.
        /// </summary>
        public async void Entrar()
        {
            StackVisibility = "Visible";
            if (!string.IsNullOrWhiteSpace(User) && !string.IsNullOrWhiteSpace(UserPassword))
            {
                Connect.ConnectToServer(User, "Admin");
                try
                {
                    conexion.Connection.On("SetStatus", handler: (string a) =>
                    {
                        StackVisibility = "Collapsed";
                        Statics.ClientStatus = "Conectado al servidor";
                        MessageBox.Show(a);

                    });

                    await conexion.CallServerMethod("TestMethod", Arguments: new[] { "Conectado al sevidor." });

                    if (Statics.ClientStatus == "Conectado al servidor")
                    {
                        conexion.Connection.On("ClienteValidacion", handler: (string Usr, bool response) =>
                        {
                            if (response)
                            {
                                MainWindowViewModel model = new MainWindowViewModel(Usr, Statics.ClientStatus);
                                window.ShowWindow(model);
                                this.TryClose();
                            }
                            else
                            {
                                MessageBox.Show("El nombre de usuario o la contraseña son incorrectos.");
                            }
                        });
                        var re = conexion.CallServerMethod("ServidorValidarUsuario", Arguments: new[] { User, Statics.Hash(UserPassword) });
                        return;
                    }

                    else
                    {
                        StackVisibility = "Collapsed";
                        if (DbConnection.Login(User: User, Password: UserPassword))
                        {
                            MainWindowViewModel model = new MainWindowViewModel(User, Statics.ClientStatus);
                            window.ShowWindow(model);
                            this.TryClose();
                        }
                        else
                        {
                            MessageBox.Show("Usuario o contraseña incorrectas.");
                        }

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Debe llenar todos los campos.");
            }
        }


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

