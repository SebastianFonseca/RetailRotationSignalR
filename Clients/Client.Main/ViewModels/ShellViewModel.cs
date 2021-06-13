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
    public class ShellViewModel : Conductor<object>
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
                NotifyOfPropertyChange(() => CanEntrar);

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
                NotifyOfPropertyChange(() => CanEntrar);
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

        bool havepassflag = false;
        private bool _canEntrar;
        public bool CanEntrar
        {
            get
            {
              if((!string.IsNullOrEmpty(User) && !string.IsNullOrEmpty(UserPassword))&& havepassflag == false)
                { 
                    _canEntrar = true;
                    return _canEntrar;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                _canEntrar = value;
                NotifyOfPropertyChange(() => CanEntrar);

            }

        }

        /// <summary>
        /// Fired event when 'Entrar' button is pressed.
        /// </summary>
        public async void Entrar()
        {
            havepassflag = true;
            CanEntrar = false;
            StackVisibility = "Visible"; 
            
            Connect.ConnectToServer(User, "Admin");


            try
            {

                conexion.Connection.On("SetStatus", handler: (string a ) =>
                {
                    StackVisibility = "Collapsed";
                    Statics.ClientStatus = "Conectado al servidor";                   
                    MessageBox.Show(a);
                
                });
                await conexion.CallServerMethod("TestMethod", Arguments: new[] { "Conectado al sevidor."});

                if (Statics.ClientStatus == "Conectado al servidor")
                {
                    conexion.Connection.On("ClienteValidacion", handler: (string Usr, bool response) =>
                    {
                        if(response)
                        { 
                        MainWindowViewModel model = new MainWindowViewModel(Usr, Statics.ClientStatus);
                        window.ShowWindow(model);
                        this.TryClose();
                        }
                        else
                        {
                            MessageBox.Show("El nombre de usuario o la contraseña son incorrectos.");
                            havepassflag = false;
                        }

                    });
                 var re = conexion.CallServerMethod("ServidorValidarUsuario", Arguments: new[] { User, Statics.Hash(UserPassword) });
                 return;  
                }

                else
                {
                    StackVisibility = "Collapsed";
                    if (DbConnection.Login(User:User, Password:UserPassword))
                    {
                    MainWindowViewModel model = new MainWindowViewModel(User, Statics.ClientStatus);
                    window.ShowWindow(model);
                    this.TryClose();
                    }
                    else
                    {
                        MessageBox.Show("El nombre de usuario o la contraseña son incorrectos.");
                        havepassflag = false;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

    }
}

