using Caliburn.Micro;
using Client.Main.ViewModels;
using Client.Main.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Client.Main.ViewModels
{

    class MainMenuViewModel : Conductor<object>
    {

        private readonly IWindowManager window = new WindowManager();

        MainWindowViewModel VentanaPrincipal;
        public MainMenuViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
        }

        
        ShellViewModel model = new ShellViewModel();
        public void CerrarSesion()
    {
        window.ShowWindow(model);
        VentanaPrincipal.TryClose();

    }
        public void AgregarCliente()
        {
            VentanaPrincipal.ActivateItem(new AddClientViewModel(VentanaPrincipal));          
        }

        public void Administrativo()
        {
            VentanaPrincipal.ActivateItem(new DC_AdministrativoViewModel(VentanaPrincipal));
        }



    }



}
