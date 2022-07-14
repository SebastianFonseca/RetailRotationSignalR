using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Client.Main.Utilities;
using Client.Main.Models;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Autofac;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using System.Windows.Input;

namespace Client.Main.ViewModels
{
    class POSPagarViewModel:Screen
    {
        public ClientesModel cliente = ContainerConfig.scope.Resolve<ClientesModel>();
        public SharedConfirmClass sharedConfirmClass = ContainerConfig.scope.Resolve<SharedConfirmClass>();
        public FacturaModel factura;

        public POSPagarViewModel(FacturaModel factura)
        {
            this.factura = factura;
            _valorPagado = factura.valorTotal;
            Efectivo = true;
        }

        public string Total => $"{factura.valorTotal:$0#,#}";

        private decimal? _valorPagado;
        public decimal? ValorPagado
        {
            get 
            {
                return _valorPagado; 
            }
            set 
            {
                if (decimal.TryParse(value.ToString(), out decimal a))
                {
                    _valorPagado = value;
                    NotifyOfPropertyChange(() => ValorPagado);
                    NotifyOfPropertyChange(() => Cambio);
                }
            }
        }

        public decimal? Cambio { get { if (ValorPagado != null && factura.valorTotal!= null) return ValorPagado - factura.valorTotal; else return null; } }

        private bool _efectivo;

        public bool Efectivo
        {
            get { return _efectivo; }
            set
            {
                _efectivo = value;
                NotifyOfPropertyChange(() => Efectivo);

            }
        }

        private bool _tarjeta;

        public bool Tarjeta
        {
            get { return _tarjeta; }
            set 
            { 
                _tarjeta = value;
                NotifyOfPropertyChange(() => Tarjeta);

            }
        }

        private bool _cheque;

        public bool Cheque
        {
            get { return _cheque; }
            set 
            { 
                _cheque = value;
                NotifyOfPropertyChange(() => Cheque);

            }
        }

        public void Pagar()
        {

            if (ValorPagado != null && ValorPagado != 0)
            {
                if (decimal.Subtract((decimal)factura.valorTotal, (decimal)ValorPagado) > 0)
                {
                    MessageBox.Show("Verifique el valor pagado.");
                    return;
                }
                if (DbConnection.NuevaFacturaBool(factura))
                {
                    MessageBox.Show("Factura agregada");
                    sharedConfirmClass.done = true;
                    this.TryClose();
                    return;
                }
            }
            else { MessageBox.Show("Ingrese el valor pagado."); return; }
        }


        public void TeclaPresionadaVentana(ActionExecutionContext context)
       {

            var keyArgs = context.EventArgs as KeyEventArgs;

            if (keyArgs != null && keyArgs.Key == Key.E)
            {
                Tarjeta = false;
                Cheque = false;
                Efectivo = true;
 
                return;
            }

            if (keyArgs != null && keyArgs.Key == Key.C)
            {
                Tarjeta = false;
                Efectivo = false;
                Cheque = true;
                return;
            }
            if (keyArgs != null && keyArgs.Key == Key.T)
            {
                Efectivo = false;
                Cheque = false;
                Tarjeta = true;
                return;
            }

            if (keyArgs != null && keyArgs.Key == Key.Escape)
            {
                this.TryClose();
                return;
            }


        }


    }


}

