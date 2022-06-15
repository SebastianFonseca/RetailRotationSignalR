using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main.ViewModels
{
    public class ComprasPorLocalViewModel : Screen
    {
        public MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
       // public PedidoModel pedido;
        public EnvioModel envio = new EnvioModel();
        public string ctor = "";

        public ComprasPorLocalViewModel(MainWindowViewModel argVentana, PedidoModel pedido)
        {
           // this.pedido = pedido;
            envio = pedido;
            VentanaPrincipal = argVentana;
            DisplayName = "Pedido de "+ pedido.puntoVenta.nombre;
            ctor = "new";
        }

        public ComprasPorLocalViewModel(MainWindowViewModel argVentana, EnvioModel envio)
        {
            this.envio = envio;            
            VentanaPrincipal = argVentana;
            DisplayName = "Pedido de " + envio.puntoVenta.nombre;
            ctor = "update";
        }



        public BindableCollection<ProductoModel> Productos
        {
            get => envio.productos;
            set
            {
                envio.productos = value;
                NotifyOfPropertyChange(() => Productos);
            }

        }


        public string Fecha
        {
            get { return envio.fechaEnvio.ToShortDateString(); }
        }
        public string Codigo  
        {
            get { return envio.codigo; }
        }

        public bool flag = true;
        public bool cambio1 = false;

        private string _placasN = "";

        public string PlacasN
        {
            get 
            {
                //return envio.placasCarro.Split('-')[1];
                if (ctor == "update" && flag) { flag = false; return envio.placasCarro.Split('-')[1]; }
                if (ctor == "update" && cambio1 == false) { return envio.placasCarro.Split('-')[1]; }
                return _placasN;
            }
            set
            {
                cambio1 = true;
                _placasN = value;
                NotifyOfPropertyChange(() => PlacasN);
            }
        }

        public bool flag2 = true;
        public bool cambio = false;

        private string _placasL = "";

        public string PlacasL
        {
            get
            {
                //return envio.placasCarro.Split('-')[0];
                
                if (ctor == "update" && flag2) { flag2 = false; return envio.placasCarro.Split('-')[0]; }
                if (ctor == "update" && cambio == false) { return envio.placasCarro.Split('-')[0]; }
                return _placasL;
            }
            set 
            {
                cambio = true;
                _placasL = value.ToUpper();
                NotifyOfPropertyChange(() => PlacasL);
            }
        }
        private string _conductor;

        public string Conductor
        {
            get 
            {
                if (envio.nombreConductor != null) return envio.nombreConductor;
                return _conductor;
            }
            set
            {
                Statics.PrimeraAMayuscula(value);
                _conductor = value;
                envio.nombreConductor = value;
                NotifyOfPropertyChange(() => Conductor);
            }
        }


        public override async void CanClose(Action<bool> callback)
        {
            if (ctor == "update")
            {
                callback(true);
                return;
            }

            if (!string.IsNullOrEmpty(Conductor) & PlacasL.Length == 3 & PlacasN.Length == 3)
            {
                envio.placasCarro = PlacasL + "-" + PlacasN;

                try
                {
                    if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                    {
                        if (ctor == "new")
                        {
                            Task<object> re = conexion.CallServerMethod("ServidorNuevoEnvioBool", Arguments: new[] { envio });
                            await re;
                            if (re.Result.ToString() == "true")
                                MessageBox.Show("Datos del envio guardados");
                        }

                    }
                    else if (MainWindowViewModel.Status == "Trabajando localmente")
                    {

                        if (ctor == "new")
                        {
                            if (DbConnection.NuevoEnvioBool(envio))
                                MessageBox.Show("Datos del envio guardados");
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                callback(true);
            }
            else
            {
                MessageBox.Show("Complete los datos del conductor y las placas antes de cerrar el formulario");
            }
            
        }



        public async void Guardar()
        {
            envio.placasCarro = PlacasL + "-" + PlacasN;
            if ( !string.IsNullOrEmpty(Conductor) & envio.placasCarro.Length == 7  )
            {
                try
                {
                    if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                    {
                        if (ctor == "new")
                        {
                            Task<object> re = conexion.CallServerMethod("ServidorNuevoEnvioBool", Arguments: new[] {envio});
                            await re;                            
                            if (re.Result.ToString() == "true")
                            { MessageBox.Show("Datos del envio guardados"); }
                        }
                        if (ctor == "update")
                        {
                            Task<object> re = conexion.CallServerMethod("ServidorupdateEnvio", Arguments: new[] { envio });
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
                            if (DbConnection.NuevoEnvioBool(envio))
                                MessageBox.Show("Datos del envio guardados");
                        }
                        if (ctor == "update")
                        {
                            if (DbConnection.updateEnvio(envio))
                                MessageBox.Show("Datos actualizados");
                            VentanaPrincipal.ActivateItem(new ListadoCompraViewModel(VentanaPrincipal));
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }                
            }
            else
            {
                MessageBox.Show("Asegurese de rellenas los campos correctamente");
            }

        }


    }
}
