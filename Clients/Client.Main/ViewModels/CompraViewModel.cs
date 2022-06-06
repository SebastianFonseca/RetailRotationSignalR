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
    public class CompraViewModel : Screen
    {
        public MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public BindableCollection<PedidoModel> pedidosSeleccionados = new BindableCollection<PedidoModel>();
        public ComprasModel compra;
        public ComprasModel compra2;
        public string ctor = "";


        public CompraViewModel(MainWindowViewModel argVentana, BindableCollection<PedidoModel> pedidos)
        {
            try
            {
                VentanaPrincipal = argVentana;
                getProveedores();
                compra = new ComprasModel(pedidos);
                compra.responsable.cedula = this.VentanaPrincipal.usuario.cedula;
                InsertarCompra(compra);
                compra2 = new ComprasModel() { codigo = compra.codigo, fecha = compra.fecha };
                DisplayName = "Compra";
                          
                ctor = "New";
            }
            catch (Exception e )
            {

                MessageBox.Show(e.Message + "aqui");
            }
        }

        public async void InsertarCompra(ComprasModel compra)
        {
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    Task<object> re = conexion.CallServerMethod("ServidorNuevaCompra", Arguments: new object[] { compra });
                    await re;
                    if (re.Result.ToString() != "true") { DbConnection.NuevaCompraBool(compra); }
                }
                if (MainWindowViewModel.Status == "Trabajando localmente")
                {
                    DbConnection.NuevaCompraBool(compra); 
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "metodo insertar");
            }

        }



        public CompraViewModel(MainWindowViewModel argVentana, ComprasModel pCompra)
        {
            VentanaPrincipal = argVentana;
            compra = pCompra;
            compra2 = new ComprasModel() { codigo = compra.codigo, fecha = compra.fecha };
            Productos = pCompra.sumaPedidos;
            DisplayName = "Compra";
            getProveedores();
            ctor = "Update";
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

        public string Fecha => compra.fecha.ToString("dd/MM/yyyy");

        public string Codigo
        {
            get { return compra.codigo; ; }
        }

        public ProductoModel seleccionadoAnterior = new ProductoModel();

        private ProductoModel _seleccionado;
        public ProductoModel Seleccionado
        {
            get => _seleccionado;
            set
            {
                if (value != null)
                {
                    _seleccionado = value;
                    NotifyOfPropertyChange(() => Seleccionado);
                    compra2.productos = null;
                    compra2.productos = new BindableCollection<ProductoModel>() { seleccionadoAnterior };
                    ActualizarRegistro(compra2);
                    seleccionadoAnterior = value;
                }

            }
        }

        private BindableCollection<ProveedorModel> _proveedores;

        public BindableCollection<ProveedorModel> Proveedores
        {
            get => _proveedores;
            set => _proveedores = value;
        }

        public async void getProveedores()
        {
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {

                    Task<object> re = conexion.CallServerMethod("ServidorGetTodosProveedor", Arguments: new object[] { });
                    await re;
                    _proveedores = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ProveedorModel>>(re.Result.ToString());
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


        public void BackButton()
        {
            if (Productos != null)
            {
                Productos.Clear();
            }

            if (ctor == "New")
            {
                VentanaPrincipal.ActivateItem(new CompraResultadoBusquedaViewModel(VentanaPrincipal, compra));
            }
            else if (ctor == "Update")
            {
                VentanaPrincipal.ActivateItem(new ListadoCompraViewModel(VentanaPrincipal));

            }

        }

        public override void CanClose(Action<bool> callback)
        {
            compra2.productos = null;
            compra2.productos = new BindableCollection<ProductoModel>() { seleccionadoAnterior };
            ActualizarRegistro(compra2);
            callback(true);
        }

        public void Guardar()
        {
            compra2.productos = null;
            compra2.productos = new BindableCollection<ProductoModel>() { seleccionadoAnterior };
            ActualizarRegistro(compra2);
            VentanaPrincipal.ActivateItem(new CompraResultadoBusquedaViewModel(VentanaPrincipal, compra));

        }



        public async void ActualizarRegistro(ComprasModel compra) 
        {
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {

                    Task<object> re = conexion.CallServerMethod("ServidorUpdateRegistroCompra", Arguments: new object[] { compra });
                    await re;
                    if (re.Result.ToString() != "true") { DbConnection.UpdateRegistroCompra(compra); }
                }
                if (MainWindowViewModel.Status == "Trabajando localmente")
                {
                    DbConnection.UpdateRegistroCompra(compra);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


    }
}
