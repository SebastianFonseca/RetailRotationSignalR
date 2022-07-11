using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using Client.Main.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Client.Main.ViewModels
{

    public class POSViewModel : Screen
    {
        MainWindowViewModel VentanaCliente;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public ClientesModel cliente = ContainerConfig.scope.Resolve<ClientesModel>();

        ///Objeto responsable de la administracion de las ventanas.
        private readonly IWindowManager window = new WindowManager();

        public POSViewModel(MainWindowViewModel argVentana)
        {
            VentanaCliente = new MainWindowViewModel(argVentana.usuario);
        }

        private BindableCollection<ProductoModel> _productos = new BindableCollection<ProductoModel>();
        public BindableCollection<ProductoModel> Productos
        {
            get => _productos;
            set { _productos = value; NotifyOfPropertyChange(() => Productos); }
        }

        private BindableCollection<ProductoModel> _busquedasProducto = new BindableCollection<ProductoModel>();
        public BindableCollection<ProductoModel> BusquedasProducto
        {
            get => _busquedasProducto;
            set
            {
                _busquedasProducto = value;
                NotifyOfPropertyChange(() => BusquedasProducto);
            }
        }

        private ProductoModel _poductoAgregar = new ProductoModel();
        public ProductoModel ProductoAgregar
        {
            get => _poductoAgregar;
            set
            {
                if (value != null)
                {
                    //BuscarTbx = value.codigoProducto + " - " + value.nombre;
                    _poductoAgregar = value;
                    NotifyOfPropertyChange(() => PrecioProductoSeleccioado);
                    NotifyOfPropertyChange(() => NombreProductoSeleccioado);
                }
            }
        }

        private string _cantidadVenta;
        public string CantidadVenta
        {
            get => _cantidadVenta;
            set
            {
                if (decimal.TryParse(value, out decimal valor))
                {
                    ProductoAgregar.cantidadVenta = valor;
                    _cantidadVenta = value;
                    NotifyOfPropertyChange(() => CantidadVenta);
                }

            }
        }
        public string Cajero => VentanaCliente.usuario.firstName + " " + VentanaCliente.usuario.lastName;
        public string Local => VentanaCliente.usuario.puntoDeVenta.nombre;
        public string Caja => ConfigurationManager.AppSettings["Caja"];

        private string _nombreCliente;
        public string NombreCliente
        {
            get
            {
                if (cliente.firstName != null && cliente.lastName != null) { return cliente.firstName.Trim() + " " + cliente.lastName.Trim(); }
                else { return null; }
            }
            set
            {
                _nombreCliente = value; NotifyOfPropertyChange(() => NombreCliente);
            }
        }

        private int _puntosCliente;
        public int PuntosCliente
        {
            get => cliente.puntos;
            set
            {
                cliente.puntos = value;
                _puntosCliente = value;
                NotifyOfPropertyChange(() => PuntosCliente);
            }
        }

        private string _nombreProductoSeleccioado;
        public string NombreProductoSeleccioado
        {
            get
            {
                if (ProductoAgregar != null) { return ProductoAgregar.nombre; }
                else { return null; }
            }
            set { _nombreProductoSeleccioado = value; NotifyOfPropertyChange(() => NombreProductoSeleccioado); }
        }

        private string _precioProductoSeleccioado;
        public string PrecioProductoSeleccioado
        {
            get
            {
                if (ProductoAgregar != null) { return $"{ProductoAgregar.precioVenta:$0#,#} {ProductoAgregar.unidadVenta}."; }
                else { return null; }
            }
            set { _precioProductoSeleccioado = value; NotifyOfPropertyChange(() => PrecioProductoSeleccioado); }
        }

        private decimal? _total = 0;
        public decimal? Total
        {
            get => _total;
            set { _total = value; NotifyOfPropertyChange(() => Total); }
        }

        private decimal? _iva = 0;

        public decimal? IVA
        {
            get => _iva;
            set { _iva = value; NotifyOfPropertyChange(() => IVA); }
        }

        private string _buscarTbx;
        public string BuscarTbx
        {
            get => _buscarTbx;
            set
            {
                if (value == null)
                {
                    MessageBox.Show("Buscar null");
                }
                _buscarTbx = Statics.PrimeraAMayuscula(value);
                NotifyOfPropertyChange(() => BuscarTbx);


            }
        }

        private string _busquedasVisibiliad = "Hidden";
        public string BusquedasVisibilidad
        {
            get
            {
                if (String.IsNullOrEmpty(BuscarTbx)) { return "Hidden"; }
                return _busquedasVisibiliad;
            }
            set { _busquedasVisibiliad = value; NotifyOfPropertyChange(() => BusquedasVisibilidad); }
        }

        public async void EscribiendoBusqueda()
        {
            BusquedasProducto.Clear();
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {

                    Task<object> re2 = conexion.CallServerMethod("ServidorGetProductos", Arguments: new[] { BuscarTbx });
                    await re2;
                    BusquedasProducto = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ProductoModel>>(re2.Result.ToString());


                    if (BusquedasProducto.Count == 0)
                    {
                        BusquedasVisibilidad = "Hidden";
                    }
                    else
                    {
                        BusquedasVisibilidad = "Visible";
                    }
                }
                else
                {

                    if (MainWindowViewModel.Status == "Trabajando localmente")
                    {
                        BusquedasProducto = DbConnection.getProductos(caracteres: BuscarTbx);
                        if (BusquedasProducto.Count == 0)
                        {
                            _poductoAgregar = null;
                            BusquedasVisibilidad = "Hidden";
                        }
                        else
                        {
                            BusquedasProducto[0].isSelected = true;
                            BusquedasVisibilidad = "Visible";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void TeclaPresionadaControl(ActionExecutionContext context)
        {

            var keyArgs = context.EventArgs as KeyEventArgs;
            if (ProductoAgregar == null)
            {
                return;
            }

            if (keyArgs != null && keyArgs.Key == Key.Enter)
            {
                if (CantidadVenta == null)
                {
                    MessageBox.Show("Ingrese la cantidad");
                    return;
                }
                if (string.IsNullOrEmpty(BuscarTbx)) { return; }
                ProductoAgregar.cantidadVenta = decimal.Parse(CantidadVenta);
                ProductoAgregar.totalValorVenta = ProductoAgregar.cantidadVenta * ProductoAgregar.precioVenta;

                Total = Total + ProductoAgregar.totalValorVenta;
                if (ProductoAgregar.iva != null && ProductoAgregar.iva != 0)
                {
                    IVA = IVA + (ProductoAgregar.totalValorVenta - (ProductoAgregar.totalValorVenta / (1 + (ProductoAgregar.iva / 100))));
                }

                


                Productos.Add(ProductoAgregar);

                NotifyOfPropertyChange(() => Productos);
                NotifyOfPropertyChange(() => Total);
                NotifyOfPropertyChange(() => IVA);
                BuscarTbx = "";
                _cantidadVenta = null;
                NotifyOfPropertyChange(() => CantidadVenta);
                ProductoAgregar = null;
                NotifyOfPropertyChange(() => ProductoAgregar);
                BusquedasProducto.Clear();
            }

            if (keyArgs != null && keyArgs.Key == Key.Down)
            {
                int indice = BusquedasProducto.IndexOf(BusquedasProducto.First<ProductoModel>(p => p.isSelected == true));
                BusquedasProducto.All(prod => prod.isSelected = false);
                if (indice == BusquedasProducto.Count - 1)
                {
                    BusquedasProducto[0].isSelected = true;
                    return;
                }
                BusquedasProducto[indice + 1].isSelected = true;
            }

            if (keyArgs != null && keyArgs.Key == Key.Up)
            {
                int indice = BusquedasProducto.IndexOf(BusquedasProducto.First<ProductoModel>(p => p.isSelected == true));
                BusquedasProducto.All(prod => prod.isSelected = false);
                if (indice == 0)
                {
                    BusquedasProducto[BusquedasProducto.Count - 1].isSelected = true;
                    return;
                }
                BusquedasProducto[indice - 1].isSelected = true;
            }
        }

        public void TeclaPresionadaVentana(ActionExecutionContext context)
        {

            var keyArgs = context.EventArgs as KeyEventArgs;

            if (keyArgs != null && keyArgs.Key == Key.F1)
            {
                window.ShowDialog(new POSLogClienteViewModel(VentanaCliente));
                NotifyOfPropertyChange(() => NombreCliente);
                NotifyOfPropertyChange(() => PuntosCliente);
                return;
            }
            if (keyArgs != null && keyArgs.Key == Key.Escape)
            {
                this.TryClose();
                return;
            }

        }


        public void EliminarProducto(ProductoModel prod)
        {
            MessageBox.Show(prod.nombre);
        }

    }
}
