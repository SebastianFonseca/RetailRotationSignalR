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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Client.Main.ViewModels
{

    public class POSViewModel : Screen
    {
        MainWindowViewModel Ventana;

        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();

        public ClientesModel cliente = ContainerConfig.scope.Resolve<ClientesModel>();

        public SharedConfirmClass sharedConfirmClass = ContainerConfig.scope.Resolve<SharedConfirmClass>();

        public FacturaModel factura;

        ///Objeto responsable de la administracion de las ventanas.
        private readonly IWindowManager window = new WindowManager();
        public POSViewModel(MainWindowViewModel argVentana)
        {
            this.Activated += OnActivated;
            factura = new FacturaModel();
            factura.productos = Productos;
            factura.cliente = cliente;
            factura.responsable = argVentana.usuario;
            Ventana = argVentana;
        }

        public void OnActivated(object sender, ActivationEventArgs activatoinArgs)
        {
            bool arqueospedientes = DbConnection.arqueoPendiente();
            bool facturasPendientes = DbConnection.faturasPendientes();

            if (facturasPendientes)
            {
                MessageBox.Show("Arqueos pendientes.");
                return;
            }  
            
            if (!arqueospedientes)
            {
                //Necesario para llevar el registro de las facturas que hacen parte del ingreso
                if (!DbConnection.primeraFactura(factura.codigo, "Inicio")) { return; }
            }
            
        }


        private BindableCollection<ProductoModel> _productos = new BindableCollection<ProductoModel>();
        public BindableCollection<ProductoModel> Productos
        {
            get => _productos;
            set { _productos = value; factura.productos = value; NotifyOfPropertyChange(() => Productos); }
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
                if (string.IsNullOrEmpty(value))
                { _cantidadVenta = null; return; }
                if (value == ".") { _cantidadVenta = value; return; }
                if (value[0] == '.') { value = 0 + value; }

                NumberStyles style;
                CultureInfo culture;
                style = NumberStyles.AllowDecimalPoint | NumberStyles.Float;
                culture = CultureInfo.CreateSpecificCulture("en-US");
                if (decimal.TryParse(value, style, culture, out decimal m)) { _cantidadVenta = value; /*MessageBox.Show(m.ToString());*/ }


            }
        }
        public string Cajero => Ventana.usuario.firstName + " " + Ventana.usuario.lastName;
        public string Local => Ventana.usuario.puntoDeVenta.nombre;
        public string Caja => ConfigurationManager.AppSettings["Caja"].Split(':')[1];

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

        private decimal _puntosCliente;
        public decimal PuntosCliente
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
                if (ProductoAgregar != null && CantidadVenta != null)
                {

                    return ProductoAgregar.nombre + $" {ProductoAgregar.precioVenta:$0#,#} {ProductoAgregar.unidadVenta} \n" +
                    $"{CantidadVenta} vale(n) { decimal.Multiply(decimal.Parse(CantidadVenta), (decimal)ProductoAgregar.precioVenta):$0#,#}";
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
                if (ProductoAgregar != null && ProductoAgregar.precioVentaConDescuento != null && CantidadVenta != null)
                {
                    decimal.TryParse(CantidadVenta, out decimal cantidad);
                    return $" Con Descuento {ProductoAgregar.precioVentaConDescuento:$0#,#} {ProductoAgregar.unidadVenta} \n" +
                        $"{CantidadVenta} vale(n) { cantidad * ProductoAgregar.precioVentaConDescuento:$0#,#} ";
                }
                else { return null; }
            }
            set { _precioProductoSeleccioado = value; NotifyOfPropertyChange(() => PrecioProductoSeleccioado); }
        }
        public decimal? Total => factura.valorTotal;
        public decimal? Descuento => factura.descuentoTotal;
        public decimal? IVA => factura.ivaTotal;

        private decimal? _subtotal = 0;
        public decimal? Subtotal
        {
            get => _subtotal;
            set { _subtotal = value; NotifyOfPropertyChange(() => Subtotal); }
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

        public void EscribiendoBusqueda()
        {
            BusquedasProducto.Clear();
            try
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
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void EliminarProducto(ProductoModel prod)
        {
            window.ShowDialog(new POSAutorizarEliminarProductosViewModel(factura: factura, producto: prod));
            Subtotal = factura.valorTotal + factura.descuentoTotal;
            NotifyOfPropertyChange(() => Descuento);
            NotifyOfPropertyChange(() => IVA);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => Subtotal);

            return;
        }

        private void comprobarInicio()
        {

        }
              
        public override void CanClose(Action<bool> callback)
        {
            if (Productos.Count != 0)
            {
                factura.superAuto = factura.responsable;
                factura.observaciones = "Intenta cerrar sin guardar";
                DbConnection.NuevaFacturaBorradaBool(factura);
                MessageBox.Show("No cierre la ventana sin registrar la factura");
                return;
            }
            else
            {
                callback(true);
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
                if (CantidadVenta == null) { MessageBox.Show("Ingrese la cantidad"); return; }

                ///Evita que se intente agregar a la factura cuando no hay un producto seleccionado o cuado el seleccionado contunua siendo el anterior
                if (string.IsNullOrEmpty(BuscarTbx)) { return; }

                ///Evita que se intente cobrar cuando hay un precio de 0
                if (ProductoAgregar.precioVenta == 0) { MessageBox.Show("Error en el sistema. Precio de venta. Notifique a un administrador."); return; };

                ///Multiplica la cantidad por el precio para mostarlo en la pantalla y para sumar el total, se calcula primero porque de cualquier manera deeb calcularse para halla el descuento
                ProductoAgregar.cantidadVenta = decimal.Parse(CantidadVenta);
                ProductoAgregar.totalValorVenta = ProductoAgregar.cantidadVenta * ProductoAgregar.precioVenta;

                if (ProductoAgregar.porcentajePromocion != null && ProductoAgregar.porcentajePromocion != 0 && ProductoAgregar.precioVenta != null)
                {
                    ///Encuentra el precio de venta con promocion

                    ProductoAgregar.precioVentaConDescuento = decimal.Subtract((decimal)ProductoAgregar.precioVenta, decimal.Multiply((decimal)ProductoAgregar.precioVenta, ((decimal)ProductoAgregar.porcentajePromocion / 100)));

                    ///Encuentra el valor de descuento total teniendo cuenta el valor calculado antes con precio full                  
                    factura.descuentoTotal = factura.descuentoTotal + (ProductoAgregar.totalValorVenta - (ProductoAgregar.cantidadVenta * ProductoAgregar.precioVentaConDescuento));

                    ///Modifica el valor total de la venta teniendo en cuenta el descuento
                    ProductoAgregar.totalValorVenta = ProductoAgregar.cantidadVenta * ProductoAgregar.precioVentaConDescuento;
                }

                ///Suma al total el valor del nuevo producto
                factura.valorTotal = factura.valorTotal + ProductoAgregar.totalValorVenta;

                ///El valor de subtotal 
                Subtotal = factura.valorTotal + factura.descuentoTotal;

                ///Calcula el valor del iva cobrado si es el caso
                if (ProductoAgregar.iva != null && ProductoAgregar.iva != 0)
                {
                    factura.ivaTotal = factura.ivaTotal + (ProductoAgregar.totalValorVenta - (ProductoAgregar.totalValorVenta / (1 + (ProductoAgregar.iva / 100))));
                }

                ///por default el prudcto seleccionado sera el que se va agrgando cada vez
                ProductoAgregar.isSelected = true;

                ///Flag para evitar agrefar dos veces un mismo producto 
                bool agregar = true;

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
                if (BusquedasProducto.Count < 1) { return; }
                if (BusquedasProducto.Any<ProductoModel>(p => p.isSelected == true))
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
            }

            if (keyArgs != null && keyArgs.Key == Key.Up)
            {
                ///Permite que cuado se oprima la tecla se subir cambie la seleccion del producto
                if (BusquedasProducto.Count < 1) { return; }
                if (BusquedasProducto.Any<ProductoModel>(p => p.isSelected == true))
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
        }

        public void TeclaPresionadaVentana(ActionExecutionContext context)
        {

            var keyArgs = context.EventArgs as KeyEventArgs;

            if (keyArgs != null && keyArgs.Key == Key.F1)
            {
                window.ShowDialog(new POSLogClienteViewModel(Ventana, cliente));
                NotifyOfPropertyChange(() => NombreCliente);
                NotifyOfPropertyChange(() => PuntosCliente);
                return;
            }

            if (keyArgs != null && keyArgs.Key == Key.F5)
            {
                if (factura.productos.Count == 0) { MessageBox.Show("Ingrese productos."); return; }
                window.ShowDialog(new POSPagarViewModel(factura));
                if (sharedConfirmClass.done)
                {
                    factura = new FacturaModel();
                    factura.responsable = Ventana.usuario;
                    Productos.Clear();
                    factura.productos = Productos;
                    cliente.firstName = "";
                    cliente.lastName = "";
                    cliente.cedula = "";
                    cliente.puntos = 0;
                    ProductoAgregar = new ProductoModel();
                    BusquedasProducto.Clear();
                    CantidadVenta = null;
                    NombreCliente = null;
                    PuntosCliente = 0;
                    NombreProductoSeleccioado = null;
                    PrecioProductoSeleccioado = null;
                    factura.valorTotal = 0;
                    Subtotal = 0;
                    factura.descuentoTotal = 0;
                    factura.ivaTotal = 0;
                    BuscarTbx = "";
                    BusquedasVisibilidad = "Hidden";
                    NotifyOfPropertyChange(() => Total);
                    NotifyOfPropertyChange(() => Subtotal);
                    NotifyOfPropertyChange(() => Descuento);
                    NotifyOfPropertyChange(() => IVA);
                }
                return;
            }

            if (keyArgs != null && keyArgs.Key == Key.F9)
            {
                if (DbConnection.faturasPendientes())
                {
                    if (Productos.Count == 0)
                    {
                        window.ShowDialog(new POSArqueoViewModel(Ventana.usuario));
                        ///IMPORTANTE! Cerrar la ventana para que deba abrirse de nuevo y se genere un nuevo inicio de facturas
                        if (!DbConnection.arqueoPendiente()) { this.TryClose(); return; }
                    }
                    else 
                    { 
                        MessageBox.Show("Registre la factura actual"); 
                        //this.TryClose(); 
                    }
                    return;
                }
                else
                {
                    MessageBox.Show("No ha registrado ninguna factura");
                    return;
                }             
            }

            if (keyArgs != null && keyArgs.Key == Key.Escape)
            {
                this.TryClose();
                return;
            }

        }

        public void TeclaPresionadaListbox(ActionExecutionContext context, ProductoModel prod)
        {
            var keyArgs = context.EventArgs as KeyEventArgs;
            if (keyArgs != null && keyArgs.Key == Key.Enter)
            {
                window.ShowDialog(new POSAutorizarEliminarProductosViewModel(factura: factura, producto: prod));
                return;

            }

            //if (keyArgs != null && keyArgs.Key == Key.Down)
            //{
            //    ///Permite que cuado se oprima la tecla se bajar cambie la seleccion del producto
            //    if (Productos.Count < 1) { return; }
            //    if (Productos.Any<ProductoModel>(p => p.isSelected == true))
            //    {
            //        int indice = Productos.IndexOf(Productos.First<ProductoModel>(p => p.isSelected == true));
            //        Productos.All(prod => prod.isSelected = false);
            //        if (indice == Productos.Count - 1)
            //        {
            //            Productos[0].isSelected = true;
            //            return;
            //        }
            //        Productos[indice + 1].isSelected = true;
            //    }
            //}

            //if (keyArgs != null && keyArgs.Key == Key.Up)
            //{
            //    ///Permite que cuado se oprima la tecla se subir cambie la seleccion del producto
            //    if (Productos.Count < 1) { return; }
            //    if (Productos.Any<ProductoModel>(p => p.isSelected==true))
            //    {
            //        int indice = Productos.IndexOf(Productos.First<ProductoModel>(p => p.isSelected == true));
            //        Productos.All(prod => prod.isSelected = false);
            //        if (indice == 0)
            //        {
            //            Productos[Productos.Count - 1].isSelected = true;
            //            return;
            //        }
            //        Productos[indice - 1].isSelected = true;
            //    }
            //}

        }


    }
}
