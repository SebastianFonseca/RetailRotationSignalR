using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using Client.Main.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using System.Configuration;

namespace Client.Main.ViewModels
{

    public class POSViewModel:Screen
    {
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();


        public POSViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;           
        }

        private BindableCollection<ProductoModel> _productos = new BindableCollection<ProductoModel>();

        public BindableCollection<ProductoModel> Productos
        {
            get { return _productos ; }
            set { _productos = value;  NotifyOfPropertyChange(() => Productos); }
        }

        public string Cajero
        {
            get { return VentanaPrincipal.usuario.firstName + " " + VentanaPrincipal.usuario.lastName; }
        }
        public string Local
        {
            get { return VentanaPrincipal.usuario.puntoDeVenta.nombre; }
        }
        public string Caja
        {
            get { return ConfigurationManager.AppSettings["Caja"]; }
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
                if (ProductoAgregar != null) { return $"{ProductoAgregar.precioVenta.ToString():#,##,} {ProductoAgregar.unidadVenta}"; }
                else { return null; }
            }
            set { _precioProductoSeleccioado = value; NotifyOfPropertyChange(() => PrecioProductoSeleccioado); }
        }



        private decimal? _cantidadVenta;

        public decimal? CantidadVenta
        {
            get { return _cantidadVenta; }
            set
            {
                if (decimal.TryParse(value.ToString(), out decimal valor))
                {
                    _cantidadVenta = valor;
                }
                
                NotifyOfPropertyChange(() => CantidadVenta);
            }
        }

        public decimal? Total
        {
            get 
            {
                decimal? total = 0;
                foreach (ProductoModel producto in Productos)
                {
                    total = total + producto.totalValorVenta;
                }
                return total;
            }
        }

        private string _focus = "CantidadVenta";

        public string Focus
        {
            get { return _focus; }
            set { _focus = value; NotifyOfPropertyChange(() => Focus); }
        }


        public void ExecuteFilterView(ActionExecutionContext context)
        {
            var keyArgs = context.EventArgs as KeyEventArgs;

            if (keyArgs != null && keyArgs.Key == Key.Enter)
            {
                if (CantidadVenta == 0)
                {
                    MessageBox.Show("Ingrese la cantidad");
                    return;
                }
                ProductoAgregar.cantidadVenta = CantidadVenta;
                ProductoAgregar.totalValorVenta = ProductoAgregar.cantidadVenta * ProductoAgregar.precioVenta;
                Productos.Add(ProductoAgregar);
                NotifyOfPropertyChange(() => Productos);
                NotifyOfPropertyChange(() => Total);
                BuscarTbx = "";
                Focus = "CantidadVenta";
                CantidadVenta = null;
                NotifyOfPropertyChange(() => CantidadVenta);

            }
        }
        

        private string _buscarTbx;
        public string BuscarTbx
        {
            get { return _buscarTbx; }
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



        private BindableCollection<ProductoModel> _busquedasProducto = new BindableCollection<ProductoModel>();
        public BindableCollection<ProductoModel> BusquedasProducto
        {
            get
            {
                return _busquedasProducto;
            }
            set
            {
                _busquedasProducto = value;
                NotifyOfPropertyChange(() => BusquedasProducto);
            }
        }


        private ProductoModel _poductoAgregar;
        public ProductoModel ProductoAgregar
        {
            get { return _poductoAgregar; }
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


                    if ( BusquedasProducto.Count == 0)
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


        public  void click(ProductoModel prod)
        {
                MessageBox.Show(prod.nombre);
        }


        public void imprimir()
        {
            string a = "";
            foreach (ProductoModel item in Productos)
            {
                a = a + " " + item.nombre;
            }
            MessageBox.Show(a);
        }


    }
}
