using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace Client.Main.ViewModels
{
    class InformesBuscarProveedoresResultadoBusquedaViewModel : Screen
    {
        public ProveedorModel proveedor;

        public MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();

        public InformesBuscarProveedoresResultadoBusquedaViewModel(MainWindowViewModel ventanaPrincipal, ProveedorModel proveedor)
        {
            this.proveedor = proveedor;
            this.VentanaPrincipal = ventanaPrincipal;
            getInfo(proveedor.cedula);
        }

        public async void getInfo(string cedula)
        {
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {

                    Task<object> re = conexion.CallServerMethod("ServidorgetTodosLosRegistrosDeCompraCedula", Arguments: new object[] { cedula });
                    await re;
                    proveedor.productos = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ProductoModel>>(re.Result.ToString());

                    if(proveedor.productos != null)
                    {
                        foreach (ProductoModel p in proveedor.productos)
                        {
                            if (p.precioCompra == null | p.compra == null | p.precioCompra == 0 | p.compra == 0)
                            { RegistrosIncompletos += 1; continue; }
                            TotalComprado += p.compra * p.precioCompra;
                            if (p.estado == "Pendiente") Pendiente += p.compra * p.precioCompra; 
                        }
                    }
                    NotifyOfPropertyChange(() => Productos);
                    NotifyOfPropertyChange(()=> Pendiente);
                    NotifyOfPropertyChange(() => TotalComprado);
                    NotifyOfPropertyChange(() => TotalPagado);
                    NotifyOfPropertyChange(() => Productos);
                    NotifyOfPropertyChange(() => RegistrosIncompletos);
                }
                if (MainWindowViewModel.Status == "Trabajando localmente")
                {
                    MessageBox.Show("No esta conectado al servidor");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public BindableCollection<ProductoModel> Productos { get { return proveedor.productos; } }
        public string Nombre { get { return $"{proveedor.firstName} {proveedor.lastName}"; } }
        public int RegistrosIncompletos { get; set; } = 0;
        public decimal? TotalComprado { get; set; } = 0;
        public decimal? TotalPagado { get { return TotalComprado - Pendiente; } }
        public decimal? Pendiente { get; set; } = 0;


        public void BackButton()
        {

            VentanaPrincipal.ActivateItem(new InformesBuscarProveedoresViewModel(VentanaPrincipal));
        }

    }
}

