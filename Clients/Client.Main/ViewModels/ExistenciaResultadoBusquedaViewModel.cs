using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Main.ViewModels
{
    public class ExistenciaResultadoBusquedaViewModel:Screen
    {
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public ExistenciasModel pExistencia;
        public ExistenciaResultadoBusquedaViewModel(MainWindowViewModel argVentana, ExistenciasModel existencia)
        {
            VentanaPrincipal = argVentana;
            pExistencia = existencia;

        }
    }
}
