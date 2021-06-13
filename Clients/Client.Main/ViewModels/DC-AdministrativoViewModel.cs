using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Client.Main.ViewModels
{
    class DC_AdministrativoViewModel : Screen
    {

        MainWindowViewModel VentanaPrincipal;

        public DC_AdministrativoViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
        }

        public void NuevoLocal()
        {
            VentanaPrincipal.ActivateItem(new NuevoLocalViewModel(VentanaPrincipal));
        }

        public void NuevoUsuario()
        {
                VentanaPrincipal.ActivateItem(new NuevoUsuarioViewModel(VentanaPrincipal));
        }

        public void NuevoProveedor()
        {
            VentanaPrincipal.ActivateItem(new NuevoProveedorViewModel());
        }

    }
}
