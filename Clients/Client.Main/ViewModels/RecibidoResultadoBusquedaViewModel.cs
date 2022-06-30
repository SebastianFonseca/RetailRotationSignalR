using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main.ViewModels
{
    class RecibidoResultadoBusquedaViewModel : Screen
    {
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public RecibidoModel recibido;
        public BindableCollection<ProductoModel> productosAnteriores = new BindableCollection<ProductoModel>();


        public RecibidoResultadoBusquedaViewModel(MainWindowViewModel argVentana, RecibidoModel Seleccionado)
        {
            CambiosEnvio = "False";
            foreach (ProductoModel producto in Seleccionado.productos)
            {
                productosAnteriores.Add(new ProductoModel() {codigoProducto = producto.codigoProducto,recibido = producto.recibido }); 
                if (producto.compraPorLocal != producto.recibido){CambiosEnvio = "True";}
            }
            recibido = Seleccionado;
            VentanaPrincipal = argVentana;
        }


        private string _cambiosEnvio;

        public string CambiosEnvio
        {
            get { return _cambiosEnvio; }
            set 
            { 
                _cambiosEnvio = value;
                NotifyOfPropertyChange(() => CambiosEnvio);
            }
        }


        public BindableCollection<ProductoModel> Productos
        {
            get => recibido.productos;
            set
            {
                recibido.productos = value;
                NotifyOfPropertyChange(() => Productos);
            }

        }


        public string Fecha => recibido.fechaRecibido.ToShortDateString();
        public string Codigo => recibido.codigo;
        public string Placa => recibido.placas;

        private string _conductor;

        public string Conductor
        {
            get
            {
                if (recibido.nombreConductor != null)
                {
                    return recibido.nombreConductor;
                }

                return _conductor;
            }
            set
            {
                _conductor = Statics.PrimeraAMayuscula(value);
                recibido.nombreConductor = Statics.PrimeraAMayuscula(value);
                NotifyOfPropertyChange(() => Conductor);
            }
        }


        public void BackButton()
        {
            VentanaPrincipal.ActivateItem(new RecibidoBuscarViewModel(VentanaPrincipal));
        }

        public async void Guardar()
        {

            foreach (ProductoModel producto in recibido.productos)
            {
                if (producto.recibido == null)
                {
                    MessageBox.Show($"Complete los datos de: {producto.nombre}");
                    return;
                }
                if (producto.unidadVenta == "Kil" && Math.Abs((decimal)(producto.compraPorLocal - producto.recibido)) > 5)
                {
                    MessageBox.Show($"Verifique la cantidad de {producto.nombre}");
                    return;
                }
                else if (producto.unidadVenta != "Kil" && producto.compraPorLocal - producto.recibido != 0)
                {
                    MessageBox.Show($"Verifique la cantidad de {producto.nombre}");
                    return;
                }

                try
                {
                    ProductoModel productoSinCambios = productosAnteriores.Single<ProductoModel>(p => p.codigoProducto == producto.codigoProducto);
                    if (producto.recibido != productoSinCambios.recibido)
                    {
                        productoSinCambios.recibido = producto.recibido - productoSinCambios.recibido ;
                        recibido.productosActualizados.Add(productoSinCambios);
                    }

                }
                catch (System.InvalidOperationException)
                {
                    MessageBox.Show("Error en el inventario, codigos de productos se repiten.");
                }
            }            
            if (recibido.nombreConductor == null)
            {
                MessageBox.Show("Escriba el nombre del conducor");
                return;
            }
            
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    Task<object> re = conexion.CallServerMethod("ServidorupdateRecibido", Arguments: new[] { recibido });
                    await re;
                    if (re.Result.ToString() == "true")
                    {
                        MessageBox.Show("Datos actualizados");
                    }

                    VentanaPrincipal.ActivateItem(new ListadoCompraViewModel(VentanaPrincipal));
                }
                else if (MainWindowViewModel.Status == "Trabajando localmente")
                {
                    if (DbConnection.updateRecibido(recibido))
                    {
                        MessageBox.Show("Datos actualizados");
                    }
                    VentanaPrincipal.ActivateItem(new AdministracionInventarioViewModel(VentanaPrincipal));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}




//public async void Guardar()
//{

//    foreach (ProductoModel producto in recibido.productos)
//    {
//        if (producto.recibido == null)
//        {
//            MessageBox.Show($"Complete los datos de: {producto.nombre}");
//            return;
//        }
//        if (producto.unidadVenta == "Kil" && Math.Abs((decimal)(producto.compraPorLocal - producto.recibido)) > 5)
//        {
//            MessageBox.Show($"Verifique la cantidad de {producto.nombre}");
//            return;
//        }
//        else if (producto.unidadVenta != "Kil" && producto.compraPorLocal - producto.recibido != 0)
//        {
//            MessageBox.Show($"Verifique la cantidad de {producto.nombre}");
//            return;
//        }

//        try
//        {
//            ProductoModel productoSinCambios = productosAnteriores.Single<ProductoModel>(p => p.codigoProducto == producto.codigoProducto);
//            if (producto.recibido != productoSinCambios.recibido)
//            {
//                productoSinCambios.cambioInventario = producto.recibido - productoSinCambios.recibido;
//                recibido.productosActualizados.Add(productoSinCambios);
//            }

//        }
//        catch (System.InvalidOperationException)
//        {
//            MessageBox.Show("Error en el inventario, codigos de productos se repiten.");
//        }
//    }
//    if (recibido.nombreConductor == null)
//    {
//        MessageBox.Show("Escriba el nombre del conducor");
//        return;
//    }

//    try
//    {
//        if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
//        {
//            Task<object> re = conexion.CallServerMethod("ServidorupdateRecibido", Arguments: new[] { recibido });
//            await re;
//            if (re.Result.ToString() == "true")
//            {
//                MessageBox.Show("Datos actualizados");
//            }

//            VentanaPrincipal.ActivateItem(new ListadoCompraViewModel(VentanaPrincipal));
//        }
//        else if (MainWindowViewModel.Status == "Trabajando localmente")
//        {
//            if (DbConnection.updateRecibido(recibido))
//            {
//                MessageBox.Show("Datos actualizados");
//            }
//            VentanaPrincipal.ActivateItem(new AdministracionInventarioViewModel(VentanaPrincipal));
//        }
//    }
//    catch (Exception e)
//    {
//        MessageBox.Show(e.Message);
//    }
////}
