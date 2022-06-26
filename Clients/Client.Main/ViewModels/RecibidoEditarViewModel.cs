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
        public RecibidoModel recibido;
        public RecibidoEditarViewModel(MainWindowViewModel argVentana, EnvioModel envio)
        {
            Placa = envio.placasCarro;
            recibido = envio;
            recibido.responsable.puntoDeVenta.codigo = DbConnection.getLocalUbicacion();
            VentanaPrincipal = argVentana;

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
        public string Placa { get; set; }


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

        public bool cerrar = false;
        public override void CanClose(Action<bool> callback)
        {
            if (cerrar != true)
            {
                MessageBoxResult result = MessageBox.Show($"Los datos no se han guardado, ¿Desea guardarlos?", "", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes) { Guardar(); }
                else { callback(true); }
            }
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
                    MessageBox.Show($"Verifique la cantidad de {producto.nombre}");
                    return;
                }
                else if (producto.unidadVenta != "Kil" && producto.compraPorLocal - producto.recibido != 0)
                {
                    MessageBox.Show($"Verifique la cantidad de {producto.nombre}");
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
                    Task<object> re = conexion.CallServerMethod("ServidorNuevorecibidoBool", Arguments: new[] { recibido });
                    await re;
                    if (re.Result.ToString() == "true")
                    {
                        MessageBox.Show("Datos del recibido guardados"); 
                    }
                    cerrar = true;
                    VentanaPrincipal.ActivateItem(new AdministracionInventarioViewModel(VentanaPrincipal));

                }
                else if (MainWindowViewModel.Status == "Trabajando localmente")
                {
                    if (DbConnection.NuevoRecibidoBool(recibido))
                    {
                        MessageBox.Show("Datos del recibido guardados");
                    }
                    cerrar = true;
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
