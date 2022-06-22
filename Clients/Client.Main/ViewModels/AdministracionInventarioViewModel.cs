using Caliburn.Micro;
using Client.Main.Views;
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

        public void NuevaExistencia()
        {
            VentanaPrincipal.ActivateItem(new ExistenciasNuevoViewModel(VentanaPrincipal));
        }

       public void BuscarExistencia()
        {
            VentanaPrincipal.ActivateItem(new ExistenciasBuscarViewModel(VentanaPrincipal));
        }

        public void NuevoPedido()
        {
            VentanaPrincipal.ActivateItem(new PedidoNuevoViewModel(VentanaPrincipal));

        }

        public void BuscarPedido()
        {
            VentanaPrincipal.ActivateItem(new PedidoBuscarViewModel(VentanaPrincipal));

        }

        public void NuevoRecibido()
        {
            VentanaPrincipal.ActivateItem(new RecibidoNuevoViewModel(VentanaPrincipal));

        }


        public void BuscarRecibido()
        {
            VentanaPrincipal.ActivateItem(new RecibidoBuscarViewModel(VentanaPrincipal));

        }


    }
}
