using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main.ViewModels
{
    public class ComprasDistribucionPorLocalViewModel :Conductor<IScreen>.Collection.OneActive
    {
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public BindableCollection<PedidoModel> pedidos = new BindableCollection<PedidoModel>();
        public  MainWindowViewModel VentanaPrincipal;
        public ComprasDistribucionPorLocalViewModel(BindableCollection<PedidoModel> pedidos, MainWindowViewModel VentanaPrincipal)
        {
            this.VentanaPrincipal = VentanaPrincipal;
            Items.Add(new CompraViewModel(VentanaPrincipal, pedidos));
            foreach (PedidoModel pedido in pedidos)
            {
                Items.Add(new ComprasPorLocalViewModel(VentanaPrincipal, pedido));
            }            
        }

        public ComprasDistribucionPorLocalViewModel(MainWindowViewModel VentanaPrincipal, ComprasModel compra)
        {
            this.VentanaPrincipal = VentanaPrincipal;
            Items.Add(new CompraResultadoBusquedaViewModel(VentanaPrincipal, compra));
            getEnvios(compra);

        }


        public async void getEnvios(ComprasModel compra) 
        {
            foreach (string codigo in compra.codPedidos)
            {
                try
                {
                    if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                    {

                        Task<object> re = conexion.CallServerMethod("ServidorgetEnvioConProductos", Arguments: new[] { codigo + ":" + compra.codigo });
                        await re;

                        BindableCollection<EnvioModel> envios = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<EnvioModel>>(re.Result.ToString());
                        Items.Add(new ComprasPorLocalResultadoBusquedaViewModel(VentanaPrincipal,envios[0]));

                    }
                    else if (MainWindowViewModel.Status == "Trabajando localmente")
                    {
                        BindableCollection<EnvioModel> envios = DbConnection.getEnvioConProductos(codigo+":"+compra.codigo);
                        Items.Add(new ComprasPorLocalResultadoBusquedaViewModel(VentanaPrincipal, envios[0]));
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

    }
}
