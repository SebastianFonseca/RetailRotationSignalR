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
    class ComprasPagosEditarViewModel : Screen
    {
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public BindableCollection<PedidoModel> pedidosSeleccionados = new BindableCollection<PedidoModel>();
        private readonly IWindowManager window = new WindowManager();
        public string codigoCedula;

        public ComprasPagosEditarViewModel(MainWindowViewModel argVentana, string codigoCedula)
        {
            this.codigoCedula = codigoCedula;
            Info = codigoCedula;
            VentanaPrincipal = argVentana;
            getRegistros(codigoCedula);
        }


        public async void getRegistros(string caracteres)
        {

            if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
            {
                Task<object> re = conexion.CallServerMethod("ServidorgetRegistroCompraCodigoCedula", Arguments: new object[] { caracteres });
                await re;
                Productos = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ProductoModel>>(re.Result.ToString());
            }
            else if (MainWindowViewModel.Status == "Trabajando localmente")
            {
                Productos = DbConnection.getRegistroCompraCodigoCedula(caracteres);
            }

            foreach (ProductoModel producto in Productos)
            {
                // producto.precioCompra = producto.precioCompra;
                if (producto.precioCompra != null & producto.compra != null)
                    Total = (decimal)(Total + (producto.precioCompra * producto.compra));
            }
            
        }

        private decimal _total;
        public decimal Total
        {
            get { return _total; }
            set
            {
                _total = value;
                NotifyOfPropertyChange(() => Total);
            }
        }



        private BindableCollection<ProductoModel> _productos;

        public BindableCollection<ProductoModel> Productos
        {
            get { return _productos; }
            set
            {
                _productos = value;

                NotifyOfPropertyChange(() => Productos);
            }
        }

        private string _info;

        public string Info
        {
            get { return _info; }
            set
            {
                _info = value;
                NotifyOfPropertyChange(() => Info);
            }
        }


        public void BackButton()
        {
            if (Productos != null)
            {
                Productos.Clear();
            }

            VentanaPrincipal.ActivateItem(new ComprasPagosViewModel(VentanaPrincipal));
        }

        public void Click()
        {
            if (Seleccionado.compra==null | Seleccionado.precioCompra==null | Seleccionado.proveedor==null)
            {
                MessageBox.Show("Debe editar el documento de compra, datos incompletos");
            }
            else 
            {
                window.ShowDialog(new ComprasPagosPagarViewModel(producto: Seleccionado,  argVentana: VentanaPrincipal));
                Total = 0;
                getRegistros(codigoCedula); 
            }

        }

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

                }

            }
        }


    }
}