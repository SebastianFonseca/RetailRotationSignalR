using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Main.ViewModels
{
    class DC_AdministrativoViewModel : Screen
    {

        MainWindowViewModel VentanaPrincipal;

        public DC_AdministrativoViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
        }

        public void NuevaBodega()
        {
            VentanaPrincipal.ActivateItem(new NuevaBodegaViewModel());
        }

        public void NuevoUsuario()
        {
            VentanaPrincipal.ActivateItem(new NuevoUsuarioViewModel());
        }

        public void NuevoProveedor()
        {
            VentanaPrincipal.ActivateItem(new NuevoProveedorViewModel());
        }

    }
}
