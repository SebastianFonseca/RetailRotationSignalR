using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Main.ViewModels
{
    class AdministracionInventarioViewModel: Screen
    {
        MainWindowViewModel VentanaPrincipal;
        public AdministracionInventarioViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
        }

        public void NuevoLocal()
        {
            VentanaPrincipal.ActivateItem(new LocalNuevoViewModel(VentanaPrincipal));
        }


    }
}
