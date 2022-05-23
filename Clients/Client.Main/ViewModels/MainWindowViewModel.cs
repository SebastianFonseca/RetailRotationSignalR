using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
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


        public string Usuario
        {
            get { return usuario.cedula +" - " + usuario.firstName + " " + usuario.lastName; }
            set 
            { 
                usuario.cedula = value;
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

        //private static string _cargo;

        public  string Cargo
        {
            get { return usuario.cargo ; }
            set
            { 
                usuario.cargo = value;
                NotifyPropertyChange(() => Cargo);
            }
            
        }
        public EmpleadoModel usuario = new EmpleadoModel();

        public MainWindowViewModel(EmpleadoModel usuario)
        {

            this.usuario = usuario;
            //MessageBox.Show(_cargo);
            ActivateItem(new MainMenuViewModel(this));
            base.OnActivate();
            DisplayName = " ";
           
        }



        public void ButtonMainMenu()
        {
            ActivateItem(new MainMenuViewModel(this));            
        }

    }
}
               