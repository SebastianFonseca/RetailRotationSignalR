using System;
using System.Collections.Generic;
using System.Text;
using Caliburn.Micro;
using Autofac;

namespace Client.Main.ViewModels 
{


    public class InformesViewModel : Screen 
    {

        public MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public InformesViewModel(MainWindowViewModel ventanaPrincipal)
        {
            this.VentanaPrincipal = ventanaPrincipal;
        }


        public void BuscarLocales()
        {
            VentanaPrincipal.ActivateItem(new InformesBuscarLocalesViewModel(VentanaPrincipal));
        }

        public void BuscarProveedores()
        {
            VentanaPrincipal.ActivateItem(new InformesBuscarProveedoresViewModel(VentanaPrincipal) );
        }

        public void BuscarClientes()
        {
            VentanaPrincipal.ActivateItem(new InformesBuscarClientesViewModel(VentanaPrincipal));

        }











    }
}
