using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using Client.Main.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Client.Main.ViewModels
{
    public class POSAutorizarEliminarProductosViewModel: Screen
    {

        public FacturaModel factura;
        public ProductoModel producto;

        public POSAutorizarEliminarProductosViewModel(FacturaModel factura, ProductoModel producto)
        {
            this.factura = factura;
            this.producto = producto;
            NotifyOfPropertyChange(() => Nombre);
            NotifyOfPropertyChange(() => Valor);

        }

        private string _usuario;

        public string Usuario
        {
            get { return _usuario; }
            set { _usuario = value; }
        }


        private string _contraseña;

        public string Contraseña
        {
            get { return _contraseña; }
            set { _contraseña = value; }
        }

        string Nombre => producto.nombre;
        string Valor => producto.totalValorVenta.ToString();

        public void Entrar()
        {
            object[] respuesta = DbConnection.Login(Usuario,Contraseña);
            if ((string)respuesta[0] == "Contraseña incorrecta.")
            {
                MessageBox.Show("Contraseña o usuario incorrectos");
                return;
            }
            if (respuesta.Length == 2)
            {
                dynamic persona = respuesta[1] as EmpleadoModel;
                if(persona.cargo == " Gerente general" || persona.cargo == " Administrador de sede")
                {
                    ProductoModel p = factura.productos.First<ProductoModel>(p => p.codigoProducto == producto.codigoProducto);
                    if (producto != null)
                    {
                        factura.productos.Remove(p);
                        MessageBox.Show("Producto eliminado");
                        factura.productos.Refresh();
                        this.TryClose();
                    }
                }
                else
                {
                    MessageBox.Show("!No tiene autorizacion!");
                    this.TryClose();
                }
            }


        }



        ///Codigo necesario para la validacion de los datos ingresados en las cajas de texto del formulario.
        public string Error { get { return null; } }
        int flag = 0;
        public string this[string name]
        {
            get
            {
                string result = null;
                if (flag == 3)
                {
                    if (name == "Usuario")
                    {
                        if (String.IsNullOrEmpty(Usuario))
                        {
                            result = "Escriba un usuario.";
                        }
                    }
                    else if (name == "Contraseña")
                    {
                        if (String.IsNullOrEmpty(Contraseña))
                        {
                            result = "Escriba una contaseña.";
                        }
                    }

                }
                else { flag += 1; }
                return result;
            }
        }





        /*            ,             , */
    }
}
