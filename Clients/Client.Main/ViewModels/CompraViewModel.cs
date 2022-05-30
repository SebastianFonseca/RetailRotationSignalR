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
        public ComprasModel compra2;
        public string ctor = "";


        public CompraViewModel(MainWindowViewModel argVentana, BindableCollection<PedidoModel> pedidos)
        {
            VentanaPrincipal = argVentana;
            compra = new ComprasModel(pedidos);
            compra2 = new ComprasModel() { codigo = compra.codigo, fecha = compra.fecha };
            compra.responsable.cedula = argVentana.usuario.cedula;
            DbConnection.NuevaCompraBool(compra); //Ver si esta conectado al servidor
            DisplayName = "Compra";
            getProveedores();
            ctor = "New";
        }
        public CompraViewModel(MainWindowViewModel argVentana, ComprasModel pCompra)
        {
            VentanaPrincipal = argVentana;
            compra = pCompra;
            compra2 = new ComprasModel() { codigo = compra.codigo, fecha = compra.fecha };
            Productos = pCompra.productos;
            DisplayName = "Compra";
            getProveedores();
            ctor = "Update";
        }


        public void BackButton()
        {
            if(Productos!= null)
                Productos.Clear();
            if (ctor == "New")
            {
                VentanaPrincipal.ActivateItem(new ComprasNuevoViewModel(VentanaPrincipal));
            }
            else if (ctor == "Update")
            {
                VentanaPrincipal.ActivateItem(new ListadoCompraViewModel(VentanaPrincipal));

            }


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

        private decimal _precio;

        public decimal Precio
        {
            get { return _precio; }
            set 
            {
                MessageBox.Show(value.ToString());
                _precio = value;
                NotifyOfPropertyChange(() => Precio);

            }
        }



        public ProductoModel seleccionadoAnterior = new ProductoModel();
        public BindableCollection<ProductoModel> seleccionadosanteriores = new BindableCollection<ProductoModel>();

        private ProductoModel _seleccionado;    

        public ProductoModel Seleccionado
        {
            get { return _seleccionado; }
            set 
            {
                if (value != null )
                {
                    MessageBox.Show(seleccionadoAnterior.nombre +"Compra:" + seleccionadoAnterior.compra + "Precio:"+seleccionadoAnterior.precioCompra + "Proveedor:"+seleccionadoAnterior.proveedor.cedula);
                    //seleccionadosanteriores.Add(value);
                    _seleccionado = value;
                    NotifyOfPropertyChange(() => Seleccionado);
                    compra2.productos = null;
                    compra2.productos = new BindableCollection<ProductoModel>() { seleccionadoAnterior };
                    DbConnection.UpdateRegistroCompra(compra2);
                    seleccionadoAnterior = value;


                    //if (value.codigoProducto != seleccionadoAnterior.codigoProducto)
                    //{
                    //   // MessageBox.Show(Seleccionado.nombre + Seleccionado.compra + Seleccionado.precioCompra + Seleccionado.proveedor.cedula + "anterior" + seleccionadoAnterior.nombre);
                    //    compra2.productos = null;
                    //    compra2.productos = new BindableCollection<ProductoModel>() { seleccionadoAnterior };
                    //    DbConnection.UpdateRegistroCompra(compra2);
                    //    seleccionadoAnterior = value;
                    //}
                }

                


            }
        }

        readonly BindableCollection<string> seleccionados = new BindableCollection<string>();
        public void CambioProducto()
        {
            if (Seleccionado != null)
            {
                MessageBox.Show(Seleccionado.nombre);

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


        public void Guardar()
        {
            string a = "";
            //foreach (ProductoModel producto in seleccionadosanteriores)
            //{
            //    //a = a + producto.nombre + " ";
            //    MessageBox.Show(seleccionadosanteriores.Count.ToString());
            //}
            ////MessageBox.Show(seleccionadosanteriores.Count.ToString());
            //MessageBox.Show(a);
            //if (Seleccionado != null)
            //    MessageBox.Show(Seleccionado.nombre);
            //string b="";
            //foreach (string name in seleccionados)
            //{
            //    b = b + name + " ";
            //}
            //MessageBox.Show(b);
        }

     }
}
