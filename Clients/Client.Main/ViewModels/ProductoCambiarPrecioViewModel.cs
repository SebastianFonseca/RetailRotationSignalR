using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main.ViewModels
{
    public class ProductoCambiarPrecioViewModel:Screen
    {

        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        ProductoModel Producto = new ProductoModel();
        ProductoModel productoUltimoRegistro = new ProductoModel();


        public ProductoCambiarPrecioViewModel(MainWindowViewModel argVentana, ProductoModel prod)
        {
            VentanaPrincipal = argVentana;
            Producto = prod;
            this.Promocion =  Producto.porcentajePromocion.ToString();
            try
            {
                getUltimoRegistroCompra(Producto.codigoProducto);
                getTotalEnvio(Producto);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
        
        }

        public async void getUltimoRegistroCompra(string codigoProducto)
        {
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    Task<object> re = conexion.CallServerMethod("ServidorgetUltimoRegistroCompraProducto", Arguments: new object[] { codigoProducto });
                    await re;

                    BindableCollection<ProductoModel> ultimoRegistroProducto = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ProductoModel>>(re.Result.ToString());
                    productoUltimoRegistro = ultimoRegistroProducto[0];
                    UltimaCompra = $"{ultimoRegistroProducto[0].compra} {ultimoRegistroProducto[0].unidadCompra} × {ultimoRegistroProducto[0].precioCompra:$0#,#} = {ultimoRegistroProducto[0].compra * ultimoRegistroProducto[0].precioCompra:$0#,#}";

                }
                if (MainWindowViewModel.Status == "Trabajando localmente")
                {
                    MessageBox.Show("Un cambio de precio solo es posible conectado al servidor.");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "metodo getUltimoRegistroCompra");
            }

        }

        public async void getTotalEnvio(ProductoModel Producto)
        {
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    Task<object> re = conexion.CallServerMethod("ServidorgetTotalEnvioProduco", Arguments: new object[] { Producto.codigoProducto });
                    await re;
                    decimal.TryParse(re.Result.ToString(), out decimal total);
                    EnvioTotal = $"Envio total: {re.Result.ToString()} {productoUltimoRegistro.unidadVenta}. Precio de compra: {(productoUltimoRegistro.compra * productoUltimoRegistro.precioCompra) / total:$0#,#}";
                    decimal margen  = 1.3m ;
                    NuevoPrecio = Decimal.Multiply ((decimal)(productoUltimoRegistro.compra * productoUltimoRegistro.precioCompra) / total, margen);

                }
                if (MainWindowViewModel.Status == "Trabajando localmente")
                {
                    //MessageBox.Show("Un cambio de precio solo es posible conectado al servidor.");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "metodo getTotalEnvio");
            }

        }



        private string _codigo;
        public string Codigo
        {
            get { return Producto.codigoProducto; }
            set 
            {
                _codigo = value;
                NotifyOfPropertyChange(()=> Codigo);
            }
        }

        private string _productoNombre;

        public string ProductoNombre
        {
            get { return Producto.nombre; }
            set { _productoNombre = value; NotifyOfPropertyChange(() => ProductoNombre);
            }
        }

        private decimal? _precioActual;

        public decimal? PrecioActual
        {
            get { return Producto.precioVenta; }
            set { _precioActual = value; NotifyOfPropertyChange(() => PrecioActual);
            }
        }

        private decimal? _nuevoPrecio;

        public decimal? NuevoPrecio
        {
            get { return _nuevoPrecio; }
            set 
            { 
                _nuevoPrecio = value; 
                NotifyOfPropertyChange(() => NuevoPrecio);

            }
        }

        private string _ultimaCompra;

        public string UltimaCompra
        {
            get { return _ultimaCompra; }
            set { _ultimaCompra = value; NotifyOfPropertyChange(() => UltimaCompra);
            }
        }

        private string _envioTotal;

        public string EnvioTotal
        {
            get { return _envioTotal; }
            set { _envioTotal = value; NotifyOfPropertyChange(() => EnvioTotal); }
        }

        private bool _activar;

        public bool Activar
        {
            get { return _activar; }
            set
            {
                if (value) Producto.porcentajePromocion = Int32.Parse(Promocion); 
                _activar = value;
                NotifyOfPropertyChange(() => Activar);

            }
        }

        private bool _desactivar;

        public bool Desactivar
        {
            get { return _desactivar; }
            set
            {
                if (value) Producto.porcentajePromocion = null;
                _desactivar = value;
                NotifyOfPropertyChange(() => Desactivar);

            }
        }

        private string _promocion;

        public string Promocion
        {
            get { return _promocion; }
            set
            {
                if (Int32.TryParse(value, out int result))
                {
                    _promocion = value;
                    NotifyOfPropertyChange(() => Promocion);
                }
            }
        }

        public async void Guardar()
        {
            Producto.precioVenta = NuevoPrecio;
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    Task<object> re = conexion.CallServerMethod("ServidoractualizarPrecioProducto", Arguments: new object[] { Producto });
                    await re;

                    if (re.Result.ToString() == "true")
                    {
                        MessageBox.Show($"Precio de venta actualizado. {Producto.codigoProducto} - {Producto.nombre} {Producto.precioVenta:$0#,#}");
                        VentanaPrincipal.ActivateItem(new ProductoResultadoBusquedaViewModel(VentanaPrincipal, Producto));
                    }
                    else 
                    {
                        MessageBox.Show("No se pudo actualizar el precio. Error: " + re.Result.ToString());
                    }

                }
                if (MainWindowViewModel.Status == "Trabajando localmente")
                {
                    MessageBox.Show("Un cambio de precio solo es posible conectado al servidor.");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "metodo Guardar");
            }
        }

        public void BackButton()
        {
            VentanaPrincipal.ActivateItem(new ProductoGestionViewModel(VentanaPrincipal));
        }


    }
}
