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
    public class ComprasNuevoViewModel: Screen
    {
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public BindableCollection<PedidoModel> pedidosSeleccionados = new BindableCollection<PedidoModel>();


        public ComprasNuevoViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
            getPedidos();
        }




        public async void getPedidos()
        {
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    Task<object> re = conexion.CallServerMethod("servidorGetTodasLasExistencias", Arguments: new object[] { });
                    await re;
                   // Pedidos = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ExistenciasModel>>(re.Result.ToString());
                }
                if (MainWindowViewModel.Status == "Trabajando localmente")
                {
                    Pedidos = DbConnection.getPedidoConProductos();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        private BindableCollection<PedidoModel> _pedidos;

        public BindableCollection<PedidoModel> Pedidos
        {
            get { return _pedidos; }
            set
            {
                //pedidosSeleccionados.Add(value[0]);
                //MessageBox.Show(value.Count.ToString());
                _pedidos = value;
                NotifyOfPropertyChange(() => Pedidos);

            }
        }





        public async void ItemSeleccionado()
        {
            //MessageBox.Show(pedidosSeleccionados.Count.ToString() + Pedido.codigo.ToString()) ;
            //try
            //{
            //    if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
            //    {
            //        Task<object> re = conexion.CallServerMethod("getExistenciasConProductos", Arguments: new object[] { Existencia.codigo });
            //        await re;
            //        BindableCollection<ExistenciasModel> seleccionada = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ExistenciasModel>>(re.Result.ToString());
            //        VentanaPrincipal.ActivateItem(new PedidoEditarViewModel(VentanaPrincipal, seleccionada[0]));
            //    }
            //    if (MainWindowViewModel.Status == "Trabajando localmente")
            //    {
            //        VentanaPrincipal.ActivateItem(new PedidoEditarViewModel(VentanaPrincipal, DbConnection.getExistenciasConProductos(Existencia.codigo)[0]));
            //    }
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message);
            //}

        }

        public async void Crear()
        {
            foreach (PedidoModel item in Pedidos)
            {
                if (item.isSelected)
                {
                    pedidosSeleccionados.Add(item);
                }
            }
            VentanaPrincipal.ActivateItem(new CompraViewModel(VentanaPrincipal, pedidosSeleccionados));

        }



    }
}
