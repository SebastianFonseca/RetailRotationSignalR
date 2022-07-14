using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;

namespace Client.Main.ViewModels
{
    public class POSLogClienteViewModel:Conductor<object>
    {
        ///Objeto responsable de la administracion de las ventanas.
        private readonly IWindowManager window = new WindowManager();
        public MainWindowViewModel VentanaCliente;
        public ClientesModel cliente;


        public POSLogClienteViewModel(MainWindowViewModel argVentana, ClientesModel cliente)
        {
            this.cliente = cliente;
            VentanaCliente = argVentana;
        }


        private string _cedulaCliente;

        public string CedulaCliente
        {
            get { return _cedulaCliente; }
            set
            {
                if (Int32.TryParse(value, out int ceudla))
                {
                    _cedulaCliente = value;
                    cliente.cedula = value;
                    NotifyOfPropertyChange(() => CedulaCliente);
                }
            }
        }
        public void Guardar()
        {
            if (CedulaCliente == null){ MessageBox.Show("Escriba un número de cédula"); return; }
            BindableCollection<ClientesModel> clientes = DbConnection.getClienteCedula(CedulaCliente);
            if (clientes.Count == 1)
            {
                
                cliente.firstName = clientes[0].firstName;
                cliente.lastName = clientes[0].lastName;
                cliente.puntos = clientes[0].puntos;

                this.TryClose();
            }
            else
            {
                MessageBox.Show("Número de cédula no registrado, registrelo!");
                window.ShowDialog(new AddClientVentanaViewModel(VentanaCliente));
                this.TryClose();
            }
        }

    }
}
