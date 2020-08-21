using Caliburn.Micro;

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Client.Main.ViewModels
{
    public class MainWindowViewModel : Conductor<object>
    {     
        public MainWindowViewModel(string argUsuario)
        {
            _usuario = argUsuario;
            base.OnActivate();
            
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



    }
}
