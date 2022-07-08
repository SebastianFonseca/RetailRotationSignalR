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
    public class ComprasPagosPagarViewModel : Screen, System.ComponentModel.IDataErrorInfo
    {

        public MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public BindableCollection<PedidoModel> pedidosSeleccionados = new BindableCollection<PedidoModel>();
        public ProductoModel producto;
        public EgresoModel egreso = new EgresoModel();
        public ComprasPagosPagarViewModel(MainWindowViewModel argVentana, ProductoModel producto)
        {
            VentanaPrincipal = argVentana;
            egreso.fecha = DateTime.Today;
            egreso.producto = producto;
            egreso.responsable = VentanaPrincipal.usuario;
            this.producto = producto;
            
            if(producto.compra != null && producto.precioCompra!= null)
                 Valor = (producto.compra * producto.precioCompra).ToString();
        }

        private string _codigoSoporte;

        public string CodigoSoporte
        {
            get { return _codigoSoporte; }
            set 
            {
                if (Int32.TryParse(value, out int b))
                {
                    egreso.soporte = value;
                    _codigoSoporte = value;
                    NotifyOfPropertyChange(() => CodigoSoporte);
                }

            }
        }

        private string _valor;

        public string Valor
        {
            get { return _valor; }
            set
            {
                if (decimal.TryParse(value.ToString(), out decimal a))
                {
                    egreso.valor = a;
                }
                _valor = value;
                NotifyOfPropertyChange(() => Valor);


            }
        }

        public async void Guardar()
        {
            if (string.IsNullOrEmpty(Valor) | string.IsNullOrEmpty(CodigoSoporte))
            {
                MessageBox.Show("Complete los datos");
                return;
            }

            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    Task<object> re = conexion.CallServerMethod("ServidorpagoProveedor", Arguments: new object[] { egreso });
                    await re;
                    if (re.Result.ToString() == "true")
                    {
                        MessageBox.Show("Pago registrado");
                        this.TryClose();
                    }
                }
                else if (MainWindowViewModel.Status == "Trabajando localmente")
                {
                    MessageBox.Show("No es posible realizar pagos si no esta conectado");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
        }




        public string Error { get { return null; } }
        int flag = 0;
        public string this[string name]
        {
            get
            {
                string result = null;
                if (flag == 3)
                {
                    if (name == "Valor")
                    {
                        if (string.IsNullOrEmpty(Valor))
                        {
                            result = "Rellene este campo.";
                        }
                    }
                    else if (name == "CodigoSoporte")
                    {
                        if (string.IsNullOrEmpty( CodigoSoporte))
                        {
                            result = "Escriba un valor.";
                        }

                    }
                }
                else { flag += 1; }
                return result;
            }
        }
    }
}

