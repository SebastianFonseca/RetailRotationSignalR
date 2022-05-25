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
    public class CompraViewModel:Screen
    {
        public MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public BindableCollection<PedidoModel> pedidosSeleccionados = new BindableCollection<PedidoModel>();
        public ComprasModel compra;

        public CompraViewModel(MainWindowViewModel argVentana, BindableCollection<PedidoModel> pedidos)
        {
            VentanaPrincipal = argVentana;
            compra = new ComprasModel(pedidos);
            compra.responsable.cedula = argVentana.usuario.cedula;
            DbConnection.NuevaCompraBool(compra); //Ver si esta conectado al servidor
            DisplayName = "Compra";
            getProveedores();
        }



        public void BackButton()
        {
            if(Productos!= null)
                Productos.Clear();
            VentanaPrincipal.ActivateItem(new ComprasNuevoViewModel(VentanaPrincipal));
        }

        public BindableCollection<ProductoModel> Productos
        {
            get => compra.sumaPedidos;
            set
            {
                compra.sumaPedidos = value;
                NotifyOfPropertyChange(() => Productos);
            }

        }

        private string _cantComprada;

        public string CantComprada
        {
            get { return _cantComprada; }
            set 
            {
                MessageBox.Show(value);
                _cantComprada = value;
                NotifyOfPropertyChange(() => CantComprada);
            }
        }
        private ProductoModel _seleccionado;    

        public ProductoModel Seleccionado
        {
            get { return _seleccionado; }
            set 
            { 
                _seleccionado = value;
                NotifyOfPropertyChange(() => Seleccionado);
                MessageBox.Show(value.nombre);

            }
        }





        public string Fecha
        {
            get { return compra.fecha.ToString("dd/MM/yyyy"); ; }
        }
        public string Codigo
        {
            get { return compra.codigo; ; }
        }

        private BindableCollection<ProveedorModel> _proveedores;

        public BindableCollection<ProveedorModel> Proveedores
        {
            get { return _proveedores; }
            set { _proveedores = value; }
        }


        public async void getProveedores()
        {
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    ///Por implementar
                    //Task<object> re = conexion.CallServerMethod("ServidorgetIdProductos", Arguments: new object[] { });
                    //await re;
                    //Productos = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ProductoModel>>(re.Result.ToString());
                }
                if (MainWindowViewModel.Status == "Trabajando localmente")
                {
                    _proveedores = DbConnection.getProveedores();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }




     }
}
