using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace Client.Main.ViewModels
{
    class InformesBuscarClientesResultadoViewModel:Screen
    {

        public MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        ClientesModel resultadoCliente;
        public InformesBuscarClientesResultadoViewModel(MainWindowViewModel ventanaPrincipal, ClientesModel cliente)
        {
            this.resultadoCliente = cliente;
            this.VentanaPrincipal = ventanaPrincipal;
            getFacturas(cliente.cedula);
        }

        public async void getFacturas(string cedula)
        {
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {

                    Task<object> re = conexion.CallServerMethod("ServidorgetFacturasCliente", Arguments: new object[] { cedula });
                    await re;
                    object[] respuesta = System.Text.Json.JsonSerializer.Deserialize<object[]>(re.Result.ToString());
                    this.resultadoCliente.facturas = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<FacturaModel>>(respuesta[0].ToString());

                    decimal.TryParse(respuesta[1].ToString(), out decimal promdias);
                    PromedioDiasCompras = promdias;

                    if (resultadoCliente.facturas != null) 
                    { 
                    UltimaCompra = $" {resultadoCliente.facturas[0].fecha.ToString("dd-MM-yyyy")}, {(DateTime.Today - resultadoCliente.facturas[0].fecha).Days} día(s) atras. ";
                    ComprasTotal = resultadoCliente.facturas.Sum<FacturaModel>(f => f.valorTotal);
                    }


                    NotifyOfPropertyChange(()=> Facturas);
                    NotifyOfPropertyChange(() => PromedioDiasCompras);
                    NotifyOfPropertyChange(() => ComprasTotal);
                    NotifyOfPropertyChange(() => UltimaCompra);
                }
                if (MainWindowViewModel.Status == "Trabajando localmente")
                {
                    MessageBox.Show("No esta conectado al servidor");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public string Name => resultadoCliente.firstName + " " + resultadoCliente.lastName;
        public string CC => resultadoCliente.cedula;
        public decimal Puntos => resultadoCliente.puntos;
        public BindableCollection<FacturaModel> Facturas
        {
            get { return resultadoCliente.facturas; }
        }
        public decimal? PromedioDiasCompras { get; set; }
        public string UltimaCompra { get; set; }
        public decimal? ComprasTotal { get; set; }

        public void BackButton()
        {

            VentanaPrincipal.ActivateItem(new InformesBuscarClientesViewModel(VentanaPrincipal));
        }



    }
}
