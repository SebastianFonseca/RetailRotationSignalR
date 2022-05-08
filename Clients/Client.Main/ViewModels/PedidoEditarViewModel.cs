using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main.ViewModels
{
    public class PedidoEditarViewModel : Screen
    {
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public PedidoModel pedido = new PedidoModel();


        public PedidoEditarViewModel(MainWindowViewModel argVentana, ExistenciasModel existencia)
        {
            VentanaPrincipal = argVentana;
            pedido = existencia;
        }

        public BindableCollection<ProductoModel> Productos
        {
            get => pedido.productos;
            set
            {
                pedido.productos = value;
                NotifyOfPropertyChange(() => Productos);
            }

        }

        public string Codigo
        {
            get => pedido.codigo;
            set
            {
                pedido.codigo = value;
                NotifyOfPropertyChange(() => Codigo);
            }
        }


        public string Dia
        {

            get => pedido.fecha.Day.ToString();

        }


        public string Mes
        {
            get => pedido.fecha.Month.ToString();


            
        }

        public string Año
        {
            get => pedido.fecha.Year.ToString();

        }

        public EmpleadoModel Responsable
        {
            get => pedido.responsable;

        }

        public void BackButton()
        {
            
            VentanaPrincipal.ActivateItem(new PedidoNuevoViewModel(VentanaPrincipal));
        }





    }
}
