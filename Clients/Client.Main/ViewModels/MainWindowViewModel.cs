using Autofac;
using Caliburn.Micro;
using Client.Main.Utilities;
using Client.Main.Views;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Windows;

namespace Client.Main.ViewModels
{
    public class MainWindowViewModel : Conductor<object>
    {
        Connect conexion = ContainerConfig.scope.Resolve<Connect>();

        public async void OnClose(CancelEventArgs e)
        {
            await conexion.CallServerMethod("ClienteDesconectado", Arguments: new[] { Usuario });
        }


        private string _usuario;

        public string Usuario
        {
            get { return _usuario; }
            set 
            { 
                _usuario = value;
                NotifyOfPropertyChange(() => Usuario);
            }
        }


        private static string _status = "Trabajando localmente";
        public static string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                NotifyPropertyChange(() => Status);
            }
        }

        public static event PropertyChangedEventHandler StaticPropertyChanged;

        private static void NotifyPropertyChange<T>(Expression<Func<T>> property)
        {
            string propertyName = (((MemberExpression)property.Body).Member as PropertyInfo).Name;
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        private static string _cargo;

        public static string Cargo
        {
            get { return _cargo; }
            set
            { 
                _cargo = value;
                NotifyPropertyChange(() => Cargo);
            }
            
        }


        public MainWindowViewModel(string argUsuario,  string argCargo)
        {

            _usuario = argUsuario;            
            _cargo = argCargo;
            //MessageBox.Show(_cargo);
            ActivateItem(new MainMenuViewModel(this));
            base.OnActivate();
            
        }

        public void ButtonMainMenu()
        {
            ActivateItem(new MainMenuViewModel(this));            
        }

    }
}
               