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
    public class RecibidoEditarViewModel : Screen
    {
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public string ctor = "";
        public RecibidoModel recibido;
        public RecibidoEditarViewModel(MainWindowViewModel argVentana, EnvioModel envio)
        {
            recibido = envio;
            VentanaPrincipal = argVentana;
            ctor = "new";
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


        public string Fecha
        {
            get { return recibido.fechaRecibido.ToShortDateString(); }
        }
        public string Codigo
        {
            get { return recibido.codigo; }
        }


        private string _conductor;

        public string Conductor
        {
            get
            {
                if (recibido.nombreConductor != null) return recibido.nombreConductor;
                return _conductor;
            }
            set
            {
                Statics.PrimeraAMayuscula(value);
                _conductor = value;
                recibido.nombreConductor = value;
                NotifyOfPropertyChange(() => Conductor);
            }
        }


        public override void CanClose(Action<bool> callback)
        {
            MessageBoxResult result = MessageBox.Show($"Los datos no se han guardado, ¿Desea guardarlos?", "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) { Guardar(); }
            else { callback(true); }

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
                    MessageBox.Show($"Revise el recibido de {producto.nombre}");
                    return;
                }
                if (producto.compraPorLocal - producto.recibido!=0)
                {
                    MessageBox.Show($"Revise el recibido de {producto.nombre}");
                    return;
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
                    if (ctor == "new")
                    {
                        Task<object> re = conexion.CallServerMethod("ServidorNuevorecibidoBool", Arguments: new[] { recibido });
                        await re;
                        if (re.Result.ToString() == "true")
                        { MessageBox.Show("Datos del recibido guardados"); }
                    }
                    if (ctor == "update")
                    {
                        Task<object> re = conexion.CallServerMethod("Servidorupdaterecibido", Arguments: new[] { recibido });
                        await re;
                        if (re.Result.ToString() == "True")
                            MessageBox.Show("Datos actualizados");
                        VentanaPrincipal.ActivateItem(new ListadoCompraViewModel(VentanaPrincipal));
                    }

                }
                else if (MainWindowViewModel.Status == "Trabajando localmente")
                {

                    if (ctor == "new")
                    {
                        if (DbConnection.NuevoRecibidoBool(recibido))
                            MessageBox.Show("Datos del recibido guardados");
                    }
                    if (ctor == "update")
                    {
                        //if (DbConnection.updaterecibido(recibido))
                        //    MessageBox.Show("Datos actualizados");
                        //VentanaPrincipal.ActivateItem(new ListadoCompraViewModel(VentanaPrincipal));
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
