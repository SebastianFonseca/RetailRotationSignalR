using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using Caliburn.Micro;
using Client.Main.Models;
using System.Windows;
using System.Threading.Tasks;
using System.Collections.Generic;
using LiveCharts;

namespace Client.Main.ViewModels
{
    public class InformesBuscarLocalesResultadoViewModel : Screen
    {

        public MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public LocalModel local;
        public InformesBuscarLocalesResultadoViewModel(MainWindowViewModel ventanaPrincipal, LocalModel local)
        {
            this.local = local;
            this.VentanaPrincipal = ventanaPrincipal;
            try
            {
                getInfo(local.codigo);

            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message);
            }
        }

        public async void getInfo(string codigo)
        {
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {

                    Task<object> re = conexion.CallServerMethod("ServidorgetIngresos", Arguments: new object[] { codigo });
                    await re;
                    BindableCollection<IngresoModel> ingresos = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<IngresoModel>>(re.Result.ToString());

                    foreach (IngresoModel ingreso in ingresos)
                    {
                        valor.Add((decimal)ingreso.valor);
                        fecha.Add(ingreso.fecha.ToString("yyyy-MM-dd"));
                    }

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

        private DateTime _fInicio = DateTime.Today;

        public DateTime FechaInicio
        {
            get { return _fInicio; }
            set
            { 
                if(value <= DateTime.Today)
                { 
                    _fInicio = value;
                    NotifyOfPropertyChange(()=> FechaInicio);
                }
                
            }
        }


        private DateTime _fFinal= DateTime.Today;

        public DateTime FechaFinal
        {
            get { return _fFinal; }
            set
            { 
                if(value <= DateTime.Today)
                {
                    //if(FechaInicio == DateTime.Today) { MessageBox.Show("Seleccione la fecha de inicio primero"); return; }
                    if(FechaInicio <= value) 
                    {
                        _fFinal = value;
                        Consultar();
                    }
                        

                    NotifyOfPropertyChange(() => FechaFinal);
                }
            }
        }

        public decimal? Facturas { get; set; } = 0;

        public decimal? Empleados { get; set; } = 0;

        public decimal? Ingreso { get; set; } = 0;

        public decimal? Egresos { get; set; } = 0;

        public string Nombre { get { return local.nombre; } }
        public ChartValues<decimal> valor { get; set; } = new ChartValues<decimal>();
        public List<string> fecha { get; set; } = new List<string>();
       
         public async void Consultar()
         {
            if(FechaFinal == null | FechaFinal == null)
            {
                if(FechaFinal < FechaInicio)
                {
                    MessageBox.Show("La fecha de inicio es mayor a la fecha final, intervalo incorrecto");
                    return;
                }
                MessageBox.Show("Seleccione las fechas");
                return;
            }
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {

                    Task<object> re = conexion.CallServerMethod("ServidorgetInfoLocal", Arguments: new object[] { local.codigo, FechaInicio.Date, FechaFinal.Date });
                    await re;
                    if (re.Result == null)
                    {
                        MessageBox.Show("No se obtuvo respuesta del servidor");
                        return;
                    }

                    decimal?[] info = System.Text.Json.JsonSerializer.Deserialize<decimal?[]>(re.Result.ToString());
                    Ingreso = info[0];
                    Egresos = info[1];
                    Facturas = info[2];
                    Empleados = info[3];

                    NotifyOfPropertyChange(() => Ingreso);
                    NotifyOfPropertyChange(() => Egresos);
                    NotifyOfPropertyChange(() => Facturas);
                    NotifyOfPropertyChange(() => Empleados);

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


    }
}
