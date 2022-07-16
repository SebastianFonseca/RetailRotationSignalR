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
    public class MovimientoEfectivoMostrarIngresoViewModel : Screen
    {

         MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public IngresoModel ingreso = new IngresoModel();

        public MovimientoEfectivoMostrarIngresoViewModel(MainWindowViewModel argVentana, IngresoModel ingreso)
        {

            this.ingreso = ingreso;
            VentanaPrincipal = argVentana;
        }

        public string Id => ingreso.id;

        public string PuntoPago => ingreso.puntoPago;

        public string PuntoVenta => ingreso.puntoVenta.codigo;


        public string Fecha => ingreso.fecha.ToString("dd - MM - yyyy");

        public decimal? Total => ingreso.valor;


        public decimal? Efectivo => ingreso.efectivo;


        public decimal? Diferencia => ingreso.diferencia;


        public string Supervisor => ingreso.supervisor.cedula;


        public string Cajero => ingreso.cajero.cedula;


        public void BackButton() => VentanaPrincipal.ActivateItem(new MovimientoEfectivoViewModel(VentanaPrincipal));


    }
}
