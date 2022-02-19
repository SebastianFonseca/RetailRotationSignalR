using Caliburn.Micro;
using Client.Main.ViewModels;
using Client.Main.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Client.Main.ViewModels
{

    class MainMenuViewModel : Conductor<object>
    {

        private readonly IWindowManager window = new WindowManager();

        MainWindowViewModel VentanaPrincipal;
        public MainMenuViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
        }        
        ShellViewModel model = new ShellViewModel();


        #region Gerencia
        public void GestionDeProductos()
        {
            VentanaPrincipal.ActivateItem(new ProductoGestionViewModel(VentanaPrincipal));

        }

        public void FacturasDeCompras()
        {

        }

        public void Informes()
        {

        }

        public void Administrativo()
        {
            if (MainWindowViewModel.Cargo == " Gerente general")
            {
                //if (MainWindowViewModel.Status == "Conectado al servidor")
                //{               
                    VentanaPrincipal.ActivateItem(new DC_AdministrativoViewModel(VentanaPrincipal));
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
        public void Compras()
        {

        }
        public void Distribucion()
        {

        }
        #endregion

        #region Administracion
        public void MovimientoDeEfectivo()
        {

        }

        public void Inventario()
        {

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
        public void Cajero()
        {

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
