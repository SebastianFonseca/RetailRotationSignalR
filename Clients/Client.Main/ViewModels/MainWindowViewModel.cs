using Caliburn.Micro;
using Client.Main.Utilities;
using Client.Main.Views;
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
        public void Cerrando()
        {
            MessageBox.Show("Cerrando");
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


        public MainWindowViewModel(string argUsuario, string argStatus)
        {
            _usuario = argUsuario;
            _status = argStatus;
            ActivateItem(new MainMenuViewModel(this));
            base.OnActivate();
            
        }

        public void ButtonMainMenu()
        {
            ActivateItem(new MainMenuViewModel(this));            
        }

    }
}
               