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
        public ShellViewModel()
        {
         
        }
        ///Propiedades enlazadas con el textbox y el passwordbox  de la vista.
        public string User { get; set; }
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
        ///Expresion lambda encargada de activar/desactivar el boton 'Entrar' cuando los textbox se han llenado.
        public bool CanEntrar => !string.IsNullOrEmpty(User) && !string.IsNullOrEmpty(UserPassword);

        ///Evento disparado al presionar el boton 'Entrar'
        public async void Entrar()
        {                                      
            Connect.ConnectToServer(User, "Admin");

            try
            {

                conexion.Connection.On("SetStatus", handler: (string a ) =>
                {
                    Statics.ClientStatus = "Conectado al servidor";
                    MessageBox.Show(a);
                
                });
                await conexion.CallServerMethod("TestMethod", Arguments: new[] { "Hola"});

                if (Statics.ClientStatus == "Conectado al servidor")
                {
                    conexion.Connection.On("ClienteValidacion", handler: (string Usr) =>
                    {
                        MainWindowViewModel model = new MainWindowViewModel(Usr, Statics.ClientStatus);
                        window.ShowWindow(model);
                        this.TryClose();
                        
                    });
                 var re = conexion.CallServerMethod("ServidorValidarUsuario", Arguments: new[] { User, Statics.Hash(UserPassword) });
                 return;  
                }

                else
                {
                    MessageBox.Show($"No esta conectado al servidor. Trabajara localmente, y la informacion se cargara cuando se conecte.");
                    MainWindowViewModel model = new MainWindowViewModel(User, Statics.ClientStatus);
                    window.ShowWindow(model);
                    this.TryClose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

    }
}

