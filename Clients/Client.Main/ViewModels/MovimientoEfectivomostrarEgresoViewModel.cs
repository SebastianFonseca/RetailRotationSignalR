using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main.ViewModels
{
    public class MovimientoEfectivomostrarEgresoViewModel : Screen
    {

        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public EgresoModel egreso = new EgresoModel();

        public MovimientoEfectivomostrarEgresoViewModel(MainWindowViewModel argVentana, EgresoModel egreso)
        {
            this.egreso = egreso;
            VentanaPrincipal = argVentana;
        }

        public string Id => egreso.id;
        public string Item => egreso.itemMovimientoefectivo.codigoItem.ToString() + " - " + egreso.itemMovimientoefectivo.descripcion;
        public string Empleado => egreso.responsable.cedula.ToString();
        public string Local { get { return egreso.local.codigo; } }
        public string Proveedor { get { return egreso.proveedor.cedula; } } 
        public string Fecha { get { return egreso.fecha.ToString("dd-MM-yyyy"); } }
        public string Soporte { get { return egreso.soporte; } }
        public decimal Valor { get { return egreso.valor; } }
        public string Descripcion { get { return egreso.descripcion; } }
        public void BackButton() => VentanaPrincipal.ActivateItem(new MovimientoEfectivoViewModel(VentanaPrincipal));

    }
}
