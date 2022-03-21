using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Client.Main.ViewModels
{
    class GerenciaAdministrativoViewModel : Screen
    {

        MainWindowViewModel VentanaPrincipal;

        public GerenciaAdministrativoViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
        }

        public void NuevoLocal()
        {
            VentanaPrincipal.ActivateItem(new LocalNuevoViewModel(VentanaPrincipal));
        }
        public void BuscarLocal()
        {
            VentanaPrincipal.ActivateItem(new LocalBuscarViewModel(VentanaPrincipal));

        }


        public void NuevoUsuario()
        {
                VentanaPrincipal.ActivateItem(new NuevoUsuarioViewModel(VentanaPrincipal));
        }
        public void BuscarUsuario()
        {
            VentanaPrincipal.ActivateItem(new NuevoUsuarioBusquedaViewModel(VentanaPrincipal));

        }

        public void NuevoProveedor()
        {
            VentanaPrincipal.ActivateItem(new ProveedorNuevoViewModel(VentanaPrincipal));
        }
        public void BuscarProveedor()
        {
            VentanaPrincipal.ActivateItem(new ProveedorBuscarViewModel(VentanaPrincipal));

        }
    }
}
