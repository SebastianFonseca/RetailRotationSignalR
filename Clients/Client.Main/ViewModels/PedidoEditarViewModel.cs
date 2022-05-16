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
            pedido = (PedidoModel)existencia;
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

        public async void Guardar()
        {

            foreach (ProductoModel producto in Productos)
            {
                if (producto.pedido == null)
                {
                    MessageBox.Show($"Registre el valor corespondiente a {producto.nombre}");
                    return;
                }
            }

            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {

                    Task<object> re = conexion.CallServerMethod("ServidorNuevoPedido", Arguments: new[] { pedido });
                    await re;
                    if (re.Result.ToString() == "Se ha registrado el nuevo documento.")
                    {
                        MessageBox.Show(re.Result.ToString());
                        VentanaPrincipal.ActivateItem(new AdministracionInventarioViewModel(VentanaPrincipal));
                        return;
                    }
                    if (re.Result.ToString() == "Pedido ya registrado.")
                    {
                        MessageBox.Show("Pedido ya registrado anteriormente.");
                    }
                    MessageBox.Show(re.Result.ToString());
                }
                else
                {
                    if (MainWindowViewModel.Status == "Trabajando localmente")
                    {
                        MessageBox.Show(Utilities.DbConnection.NuevoPedido(pedido));
                        VentanaPrincipal.ActivateItem(new AdministracionInventarioViewModel(VentanaPrincipal));
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }



    }
}
