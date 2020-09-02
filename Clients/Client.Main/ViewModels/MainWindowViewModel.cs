using Caliburn.Micro;

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Client.Main.ViewModels
{
    public class MainWindowViewModel : Conductor<object>
    {     

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

        private string _status;

        public string Status
        {
            get { return _status; }
            set { 
                _status = value;
                NotifyOfPropertyChange(() =>Status);
            }
        }

        public MainWindowViewModel(string argUsuario, string argStatus)
        {
            _usuario = argUsuario;
            _status = argStatus;
            base.OnActivate();
            
        }


    }
}
