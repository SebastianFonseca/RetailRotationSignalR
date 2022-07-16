using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Client.Main.ViewModels
{
    public class POSArqueoViewModel:Screen
    {
        public IngresoModel Ingreso = new IngresoModel();

        public POSArqueoViewModel(EmpleadoModel cajero)
        {
            Ingreso.valor = DbConnection.valorTotalFacturas();
            Ingreso.cajero = cajero;
        }

        public decimal? Total => Ingreso.valor;
        public decimal? Efectivo { get {return Ingreso.efectivo; } set { Ingreso.efectivo = value; NotifyOfPropertyChange(() => Efectivo); NotifyOfPropertyChange(() => Diferencia); } }
        public decimal? Diferencia { get { if (Ingreso.valor != 0 && Ingreso.efectivo != 0) return Ingreso.valor - Ingreso.efectivo; else return null; } }
        public string Usuario { set { Ingreso.supervisor.cedula = value ; } }
        public string Responsable { get { return Ingreso.cajero.firstName +" "+  Ingreso.cajero.lastName; } }

        private string _contraseña;
        public string Contraseña
        {
            get { return _contraseña; }
            set { _contraseña = value; Ingreso.supervisor.password = value; }
        }


        public void Guardar()
        {
            if(Ingreso.efectivo == 0 && Ingreso.efectivo == null) { MessageBox.Show("Ingrese el valor de efectivo"); return; }
            object[] respuesta = DbConnection.Login(User: Ingreso.supervisor.cedula, Password: Ingreso.supervisor.password);

            if ((string)respuesta[0] == "Contraseña incorrecta.")
            {
                MessageBox.Show("Usuario o contraseña incorrectos");
                //this.TryClose();
                return;
            }
            if (respuesta.Length == 2)
            {
                dynamic persona = respuesta[1] as EmpleadoModel;
                if (persona.cargo == " Gerente general" || persona.cargo == " Administrador de sede")
                {
                    Ingreso.supervisor = persona;
                    Ingreso.diferencia = Ingreso.valor - Ingreso.efectivo;
                    if (DbConnection.NuevoIngreso(Ingreso))
                    {
                        MessageBox.Show($"Registrado. \nProcesó: {Ingreso.supervisor.cedula + " - " +  Ingreso.supervisor.firstName +  " "  + Ingreso.supervisor.lastName} \nCaja: { Ingreso.cajero.cedula + " - "+ Ingreso.cajero.firstName + " " +Ingreso.cajero.lastName} ");
                        this.TryClose();
                        return;
                    }
                    else
                    {
                        MessageBox.Show("No fue posible el registro. Informe a un adminsitrador.");
                        //this.TryClose();
                        return;
                    }


                }
                else
                {
                    MessageBox.Show("!No tiene autorizacion!");
                    this.TryClose();
                }
            }
            MessageBox.Show("No se registro. Error de Login " + respuesta[0]);
            this.TryClose();
        }

        public void TeclaPresionadaVentana(ActionExecutionContext context)
        {

            var keyArgs = context.EventArgs as KeyEventArgs;

            if (keyArgs != null && keyArgs.Key == Key.Escape)
            {
                this.TryClose();
                return;
            }
        }

    }
}
