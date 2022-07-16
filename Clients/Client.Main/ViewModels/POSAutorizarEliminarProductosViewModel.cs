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

       public string Nombre => producto.nombre;
       public decimal? Valor => producto.totalValorVenta;

 

        public void Entrar()
        {
            if (string.IsNullOrEmpty(Usuario) | string.IsNullOrEmpty(Contraseña)) { MessageBox.Show("Escriba el usuario y la contraeña"); return; }
            object[] respuesta = DbConnection.Login(Usuario,Contraseña);
            if ((string)respuesta[0] == "Contraseña incorrecta.")
            {
                MessageBox.Show("Usuario o contraseña incorrectos");
                this.TryClose();
                return;
            }
            if (respuesta.Length == 2)
            {
                dynamic persona = respuesta[1] as EmpleadoModel;
                if(persona.cargo == " Gerente general" || persona.cargo == " Administrador de sede")
                {
                    ProductoModel p = factura.productos.FirstOrDefault<ProductoModel>(p => p.codigoProducto == producto.codigoProducto);

                    if (p != null)
                    {
                        FacturaModel facturaBorrada = new FacturaModel() 
                        {
                            codigo = factura.codigo,
                            cliente = factura.cliente,
                            responsable = factura.responsable,
                            superAuto = persona,
                            puntoDePago = factura.puntoDePago,
                            fecha = factura.fecha,
                            valorTotal = factura.valorTotal                            

                        };
                        facturaBorrada.productos = new BindableCollection<ProductoModel>() { p };

                        facturaBorrada.observaciones = "Borrado";
                        if (!DbConnection.NuevaFacturaBorradaBool(facturaBorrada)) { MessageBox.Show("Error: Error en el registro de la factura, informe a un administrador."); /*No registra el borrado pero continua la facturacion*/ };

                        factura.valorTotal = factura.valorTotal - p.totalValorVenta;

                        p.totalValorVenta = p.cantidadVenta * p.precioVenta;
                        if (p.porcentajePromocion != null && p.porcentajePromocion != 0 && p.precioVenta != null)
                        {
                            ///Encuentra el valor de descuento total teniendo cuenta el valor calculado antes con precio full                  
                            factura.descuentoTotal = factura.descuentoTotal - ( p.totalValorVenta - (p.cantidadVenta * p.precioVentaConDescuento));
                        }


                        ///Calcula el valor del iva cobrado si es el caso
                        if (p.iva != null && p.iva != 0)
                        {
                            factura.ivaTotal = factura.ivaTotal - (p.totalValorVenta - (p.totalValorVenta / (1 + (p.iva / 100))));
                        }


                        //Elimina el producto de la facura actual
                        factura.productos.Remove(p);
                        MessageBox.Show("Producto borrado");
                        factura.productos.Refresh();
                        this.TryClose();
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Error borrando. Producto no encontrado?");
                    }
                }
                else
                {
                    MessageBox.Show("!No tiene autorizacion!");
                    this.TryClose();
                }
            }
            MessageBox.Show("No se elimino el producto. Error de Login " + respuesta[0]);
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
    }
}
