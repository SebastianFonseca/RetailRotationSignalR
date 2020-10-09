using Caliburn.Micro;
using Client.Main.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Client.Main.ViewModels
{
    public class MainWindowViewModel : Conductor<object>
    {

        ///Objeto responsable de la administracion de las ventanas.
        private readonly IWindowManager window = new WindowManager();

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
        private string _fondo = "#F3E0C2";

        public string Fondo
        {
            get { return _fondo; }
            set { _fondo  = value; }
        }
        
        void ChangeBackground()
        {

            _fondo = "#000000";
        }

        public MainWindowViewModel(string argUsuario, string argStatus)
        {
            _usuario = argUsuario;
            _status = argStatus;
            base.OnActivate();
            
        }

        public void CerrarSesion()
        {
            ShellViewModel model = new ShellViewModel();
            window.ShowWindow(model);    
            this.TryClose();

        }

    }
}
