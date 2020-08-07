using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Main.ViewModels
{
    public class ShellViewModel : Screen 
    {

        public ShellViewModel()
        {

        }

        public string Usuario { get; set; }
        public string Password { get; set; }

        public bool CanEntrar(string Usuario, string Password) => !string.IsNullOrEmpty(Usuario) && !string.IsNullOrEmpty(Password);

        
        public void Entrar(string Usuario, string Password)
        {
            
            MainWindowView window = new MainWindowView();
            this.TryClose();
            window.Show();
        }




    }
}
