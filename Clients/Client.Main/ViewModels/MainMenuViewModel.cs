using Autofac;
using Caliburn.Micro;
using Client.Main.ViewModels;
using Client.Main.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Client.Main.ViewModels
{

    class MainMenuViewModel :  Conductor<object>/*Conductor<object>*/
    {
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        private readonly IWindowManager window = new WindowManager();

        MainWindowViewModel VentanaPrincipal;
        public MainMenuViewModel(MainWindowViewModel argVentana)
        {
            
            VentanaPrincipal = argVentana;
            act();
        }

        public async void act()

        {
            if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
            {
                try
                {
                    //Descarga del servidor los cambios en la base de datos
                    await Utilities.Sincronizar.SincronizarRegistro();

                    //Registra en la base de datos del servidor los cambios locales que se han registrado
                    await Utilities.Sincronizar.actualizarRegistrosLocales();
                }

                catch (Exception e)
                {

                    MessageBox.Show(e.Message);

                }
            }
        }


        ShellViewModel model = new ShellViewModel();


        #region Gerencia
        public void GestionDeProductos()
        {
            VentanaPrincipal.ActivateItem(new ProductoGestionViewModel(VentanaPrincipal));

        }

        public void Informes()
        {
            VentanaPrincipal.ActivateItem(new InformesViewModel(VentanaPrincipal));

        }

        public void Administrativo()
        {
            if (VentanaPrincipal.Cargo == " Gerente general") //Permitir el acceso al encargado de compras y de distribucion para que puedan agregarnuevos proveedores.
            {
                //if (MainWindowViewModel.Status == "Conectado al servidor")
                //{               
                VentanaPrincipal.ActivateItem(new GerenciaAdministrativoViewModel(VentanaPrincipal));
                //}
                //else
                //{
                //    MessageBox.Show("Debe estar conectado al servidor.");
                //}
            }
            else
            {
                MessageBox.Show("No tiene autorizacion para usar este modulo.");
            }

        }
        #endregion

        #region Envios
        public void Listados()
        {
            VentanaPrincipal.ActivateItem(new ListadoCompraViewModel(VentanaPrincipal));
        }

        public void Pagos()
        {
            VentanaPrincipal.ActivateItem(new ComprasPagosViewModel(VentanaPrincipal));

        }


        #endregion

        #region Administracion
        public void MovimientoDeEfectivo()
        {
            VentanaPrincipal.ActivateItem(new MovimientoEfectivoViewModel(VentanaPrincipal));
        }

        public void Inventario()
        {
            VentanaPrincipal.ActivateItem(new AdministracionInventarioViewModel(VentanaPrincipal));
        }
        #endregion

        #region Clientes
        public void AgregarCliente()
        {
            VentanaPrincipal.ActivateItem(new AddClientViewModel(VentanaPrincipal));
        }

        public void BuscarCliente()
        {
            VentanaPrincipal.ActivateItem(new AddClientBuscarViewModel(VentanaPrincipal));
        }
        #endregion

        #region Cajero
        public void Pos()
        {
            window.ShowWindow(new POSViewModel(VentanaPrincipal));            
        }
        #endregion

        #region Salir

        public void CerrarSesion()
        {
            window.ShowWindow(model);
            VentanaPrincipal.TryClose();
            //MessageBox.Show(MainWindowViewModel.Status);
        }
        #endregion





    }



}
