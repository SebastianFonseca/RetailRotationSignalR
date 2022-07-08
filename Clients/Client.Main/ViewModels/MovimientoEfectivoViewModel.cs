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
    public class MovimientoEfectivoViewModel: Screen
    {
        MainWindowViewModel VentanaPrincipal;
        ///Objeto responsable de la administracion de las ventanas.
        private readonly IWindowManager window = new WindowManager();
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();

        public MovimientoEfectivoViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
            try
            {
                getMovimientos(VentanaPrincipal.usuario.puntoDeVenta.codigo);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                throw;
            }
            
        }


        private BindableCollection<MovimientoEfectivoModel> movimientos;

        public BindableCollection<MovimientoEfectivoModel> Movimientos
        {
            get { return movimientos; }
            set 
            {
                movimientos = value;
                NotifyOfPropertyChange(() => Movimientos);
            }
        }

        public async void getMovimientos(string codigoLocal)
        {
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    Task<object> re = conexion.CallServerMethod("ServidorGetMovimientosEfectivoLocal", Arguments: new object[] { codigoLocal });
                    await re;
                    Movimientos = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<MovimientoEfectivoModel>>(re.Result.ToString());
                }
            }
            catch (Exception e )
            {
                MessageBox.Show(e.Message);
                throw;
            }

        }


        public void NuevoItem() 
        {
            if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
            { window.ShowWindow(new MovimientoEfectivoNuevoItemViewModel(VentanaPrincipal)); }
            else { MessageBox.Show("Solo se puede agregar un item si esta conectado al servidor"); }

        }
        public void NuevoProveedor()
        {
            if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                 { VentanaPrincipal.ActivateItem(new ProveedorNuevoViewModel(VentanaPrincipal, conProductos: false)); }
            else { MessageBox.Show("Solo se puede agregar un proveedor si esta conectado al servidor"); }
        }
        public void NuevoEgreso()
        {
            
          if((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
            { VentanaPrincipal.ActivateItem(new MovimientoEfectivoNuevoEgresoViewModel(VentanaPrincipal)); }
            else { MessageBox.Show("Solo se puede agregar un egreso si esta conectado al servidor"); }
        }
        public void NuevoIngreso() { }


        public async void click(MovimientoEfectivoModel mov)
        {
            if (mov.egreso.id != 0)
            {
                try
                {
                    if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                    {
                        Task<object> re = conexion.CallServerMethod("ServidorgetEgreso", Arguments: new object[] { mov.egreso.id.ToString() });
                        await re;
                        BindableCollection<EgresoModel> egresos = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<EgresoModel>>(re.Result.ToString());
                        VentanaPrincipal.ActivateItem(new MovimientoEfectivomostrarEgresoViewModel(VentanaPrincipal,egresos[0]));

                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }


    
    }

    
}
