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

        public void NuevaBodega()
        {
            VentanaPrincipal.ActivateItem(new NuevaBodegaViewModel());
        }

        public void NuevoUsuario()
        {
            try
            {
                VentanaPrincipal.ActivateItem(new NuevoUsuarioViewModel(VentanaPrincipal));

            }
            catch (Exception e)
            {

                MessageBox.Show(e.StackTrace);
            }
        }

        public void NuevoProveedor()
        {
            VentanaPrincipal.ActivateItem(new NuevoProveedorViewModel());
        }

    }
}
