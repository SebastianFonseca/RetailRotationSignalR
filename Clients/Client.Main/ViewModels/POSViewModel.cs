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
        public FacturaModel factura = new FacturaModel();

        ///Objeto responsable de la administracion de las ventanas.
        private readonly IWindowManager window = new WindowManager();

        public POSViewModel(MainWindowViewModel argVentana)
        {
            factura.productos = Productos;
            factura.cliente = cliente;
            factura.responsable = argVentana.usuario;
            VentanaCliente = argVentana;
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

        // Es string para que no aparezca nada en la pantalla al incio y se pueda evitar que se agreguen letras con el tryparse 
        private string _cantidadVenta;
        public string CantidadVenta
        {
            get => _cantidadVenta;
            set
            {
                if (decimal.TryParse(value, out decimal valor) | value==null)
                {
                    ProductoAgregar.cantidadVenta = valor;
                    _cantidadVenta = value;
                    NotifyOfPropertyChange(() => CantidadVenta);
                    NotifyOfPropertyChange(() => PrecioProductoSeleccioado);
                    NotifyOfPropertyChange(() => NombreProductoSeleccioado);

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
                if (ProductoAgregar != null)
                {
                    return ProductoAgregar.nombre + $" {ProductoAgregar.precioVenta:$0#,#} {ProductoAgregar.unidadVenta} \n"+
                    $"{CantidadVenta} valen { decimal.Parse(CantidadVenta) * ProductoAgregar.precioVenta:$0#,#}";  
                }
                else { return null; }
            }
            set { _nombreProductoSeleccioado = value; NotifyOfPropertyChange(() => NombreProductoSeleccioado); }
        }

        private string _precioProductoSeleccioado;
        public string PrecioProductoSeleccioado
        {
            get
            {
                if (ProductoAgregar != null && ProductoAgregar.precioVentaConDescuento != null)
                {
                    return $" Con Descuento {ProductoAgregar.precioVentaConDescuento:$0#,#} {ProductoAgregar.unidadVenta} \n" +
                        $"{CantidadVenta} valen {decimal.Parse(CantidadVenta) * ProductoAgregar.precioVentaConDescuento:$0#,#} ";                                           
                }
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


        private decimal? _subtotal = 0;

        public decimal? Subtotal
        {
            get { return _subtotal; }
            set { _subtotal = value; NotifyOfPropertyChange(() => Subtotal); }
        }

        private decimal? _descuento = 0;
        public decimal? Descuento
        {
            get => _descuento;
            set { _descuento = value; NotifyOfPropertyChange(() => Descuento); }
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
                            //NotifyOfPropertyChange(() => PrecioProductoSeleccioado);
                            //NotifyOfPropertyChange(() => NombreProductoSeleccioado);
                            //NotifyOfPropertyChange(() => ConsultaPrecio);
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
                ///Evita que se intente agregar a la factura un producto sin cantidad
                if (CantidadVenta == null){MessageBox.Show("Ingrese la cantidad");return;}

                ///Evita que se intente agregar a la factura cuando no hay un producto seleccionado o cuado el seleccionado contunua siendo el anterior
                if (string.IsNullOrEmpty(BuscarTbx)) { return; }

                ///Evita que se intente cobrar cuando hay un precio de 0
                if (ProductoAgregar.precioVenta == 0) { MessageBox.Show("Error en el sistema. Precio de venta. Notifique a un administrador.");return; };

                ///Multiplica la cantidad por el precio para mostarlo en la pantalla y para sumar el total, se calcula primero porque de cualquier manera deeb calcularse para halla el descuento
                ProductoAgregar.cantidadVenta = decimal.Parse(CantidadVenta);
                ProductoAgregar.totalValorVenta = ProductoAgregar.cantidadVenta * ProductoAgregar.precioVenta;

                if (ProductoAgregar.porcentajePromocion != null && ProductoAgregar.porcentajePromocion != 0 && ProductoAgregar.precioVentaConDescuento != null )
                {
                    //decimal a = 100;
                    ///Encuentra el precio de venta con promocion
                    ProductoAgregar.precioVentaConDescuento = decimal.Subtract( (decimal)ProductoAgregar.precioVenta , decimal.Multiply((decimal)ProductoAgregar.precioVenta , ((decimal)ProductoAgregar.porcentajePromocion / 100 )));

                    ///Encuentra el valor de descuento total teniendo cuenta el valor calculado antes con precio full                  
                    Descuento = Descuento + (ProductoAgregar.totalValorVenta - (ProductoAgregar.cantidadVenta * ProductoAgregar.precioVentaConDescuento));

                    ///Modifica el valor total de la venta teniendo en cuenta el descuento
                    ProductoAgregar.totalValorVenta = ProductoAgregar.cantidadVenta * ProductoAgregar.precioVentaConDescuento;
                } 
              
                ///Suma al total el valo del nuevo producto
                Total = Total + ProductoAgregar.totalValorVenta;

                ///El valor de subtotal 
                Subtotal = Total + Descuento;

                ///Calcula el valor del iva cobrado si es el caso
                if (ProductoAgregar.iva != null && ProductoAgregar.iva != 0)
                {
                    IVA = IVA + (ProductoAgregar.totalValorVenta - (ProductoAgregar.totalValorVenta / (1 + (ProductoAgregar.iva / 100))));
                }

                ///Flag para evitar agrefar dos veces un mismo producto 
                bool agregar=true;

                ///Se busca si el producto ya fue agregado para sumar la cantidad al mismo
                foreach (ProductoModel productoRepetido in Productos.Where<ProductoModel>(p => p.codigoProducto == ProductoAgregar.codigoProducto))
                {
                    ///Ya que se encontro un producto que ya se habia agregado se le suma la nueva cantidad y el nuevo total
                    productoRepetido.cantidadVenta = productoRepetido.cantidadVenta + ProductoAgregar.cantidadVenta;
                    productoRepetido.totalValorVenta = productoRepetido.totalValorVenta + ProductoAgregar.totalValorVenta;
                   ///Evita que se agregrue denuevo el product, pues ya estaba agregado y lo que se hizo fue sumar la nueva cantidad
                    agregar = false;
                }

                ///Si el producto no estaba agregado se agrega
                if (agregar) { Productos.Add(ProductoAgregar); }

                ///Se notifica de los cambios para que se refresque la pantalla
                NotifyOfPropertyChange(() => Descuento);
                NotifyOfPropertyChange(() => Productos);
                NotifyOfPropertyChange(() => Total);
                NotifyOfPropertyChange(() => IVA);
                NotifyOfPropertyChange(() => Subtotal);

                ///Se borran los valores de la caja de texto para la nueva busqueda
                BuscarTbx = "";
               
                ///La cantidad se pone en null para la nuev busqueda, se  notifica a la propiedad para refrescar la pantalla
                _cantidadVenta = null;
                NotifyOfPropertyChange(() => CantidadVenta);

                ///Para agregar un nuevo producto y notificar
                ProductoAgregar = null;
                NotifyOfPropertyChange(() => ProductoAgregar);
                
                ///Se limpian los productos encontrados anteriormente
                BusquedasProducto.Clear();
            }

            if (keyArgs != null && keyArgs.Key == Key.Down)
            {
                ///Permite que cuado se oprima la tecla se bajar cambie la seleccion del producto
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
                ///Permite que cuado se oprima la tecla se subir cambie la seleccion del producto

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
