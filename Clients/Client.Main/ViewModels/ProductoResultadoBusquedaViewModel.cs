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
    class ProductoResultadoBusquedaViewModel: Screen
    {
        MainWindowViewModel VentanaPrincipal;
        ProductoModel Producto = new ProductoModel();
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();

        public ProductoResultadoBusquedaViewModel(MainWindowViewModel argVentana, ProductoModel producto)
        {
            VentanaPrincipal = argVentana;
            Producto = producto;
        }
    
        public string Codigo
        {
            get { return Producto.codigoProducto; }
            
        }
        public string Nombre
        {
            get { return Producto.nombre; }
        }

        public string UnidadVenta
        {
            get { return Producto.unidadVenta; }
            set { }
        }

        public string UnidadCompra
        {
            get { return Producto.unidadCompra; }
            set { }
        }

        public string FactorConversion
        {
            get { return Producto.factorConversion.ToString(); }
            set { }
        }
        public string Seccion
        {
            get { return Producto.seccion; }
            set { }
        }

        public string FechaVencimiento
        {
            get { return Producto.fechaVencimiento.ToShortDateString() ; }
 
        }
        public decimal? iva
        {
            get { return Producto.iva; }

        }

        public string CodigoBarras
        {
            get { return Producto.codigoBarras; }           
        }

        public void BackButton()
        {
            VentanaPrincipal.ActivateItem(new ProductoGestionViewModel(VentanaPrincipal));
        }


        public async void  Eliminar()
        {
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    MessageBoxResult result = MessageBox.Show($"Desea eliminar permanentemente de la base de datos el producto {Producto.codigoProducto} - {Producto.nombre}.", "", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        Task<object> re = conexion.CallServerMethod("ServidorDeleteProducto", Arguments: new[] { Producto.codigoProducto });
                        await re;

                        if (re.Result.ToString() == "Se ha eliminado al producto.")
                        {
                            MessageBox.Show($"Se ha eliminado el producto {Producto.codigoProducto} - {Producto.nombre}");
                            VentanaPrincipal.ActivateItem(new ProductoGestionViewModel(VentanaPrincipal));
                        }
                        else
                        {
                            MessageBox.Show(re.Result.ToString());
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Para eliminar un producto debe estar conectado al servidor.");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public void EditarInformacion()
        {
            VentanaPrincipal.ActivateItem(new ProductoEditarBusquedaViewModel(VentanaPrincipal, Producto));
        }

        public async void EditarPrecio()
        {

        }

    }
}






