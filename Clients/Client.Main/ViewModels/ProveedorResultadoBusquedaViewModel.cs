using Autofac;
using Client.Main.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Main.ViewModels
{
    class ProveedorResultadoBusquedaViewModel
    {

        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        ProveedorModel proveedorEnontrado = new ProveedorModel();
        public ProveedorResultadoBusquedaViewModel(MainWindowViewModel argVentana, ProveedorModel prov)
        {
            VentanaPrincipal = argVentana;
            proveedorEnontrado = prov;
        }
    }
}
