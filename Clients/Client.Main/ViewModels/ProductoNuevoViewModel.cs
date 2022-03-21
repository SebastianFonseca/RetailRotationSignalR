using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main.ViewModels
{
    class ProductoNuevoViewModel : Screen, IDataErrorInfo 
    {
        public MainWindowViewModel VentanaPrincipal;
        ProductoModel Producto = new ProductoModel();
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();

        public ProductoNuevoViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
            
        }


        string _letraCodigo;
        public string LetraCodigo
        {
            get { return _letraCodigo; }
            set 
            { 
                _letraCodigo = value;
                NotifyOfPropertyChange(() => LetraCodigo);
            }
        }


        string _numeroCodigo;
        public string NumeroCodigo
        {
            get { return _numeroCodigo; }
            set
            { 
                _numeroCodigo = value;
                NotifyOfPropertyChange(() => NumeroCodigo);

            }
        }


        public string Nombre
        {
            get { return Producto.nombre; }
            set
            { 
                Producto.nombre = value;
                NotifyOfPropertyChange(() => Nombre);

            }
        }

        public string UnidadVenta
        {
            get { return Producto.unidadVenta; }
            set
            {
                Producto.unidadVenta = value.Substring(38);
                NotifyOfPropertyChange(() => UnidadVenta);

            }
        }

        public string Seccion
        {
            get { return Producto.seccion; }
            set 
            { 
                Producto.seccion = value.Substring(38) ;
                NotifyOfPropertyChange(() => Seccion);

            }
        }

        public DateTime FechaVencimiento
        {
            get { return Producto.fechaVencimiento; }
            set
            { 
                Producto.fechaVencimiento = value;
                NotifyOfPropertyChange(() => FechaVencimiento);

            }
        }
        public decimal IVA
        {
            get { return Producto.iva; }
            set 
            { 
                Producto.iva = value;
            }
        }

        public string CodigoBarras
        {
            get { return Producto.codigoBarras; }
            set { Producto.codigoBarras = value; }
        }

        public void BackButton()
        {
            VentanaPrincipal.ActivateItem(new ProductoGestionViewModel(VentanaPrincipal));
        }


        public async void Guardar()
        {
            Producto.codigoProducto = LetraCodigo + NumeroCodigo;
            //DbConnection.SincronizarReplicacionMerge();
            if (!string.IsNullOrWhiteSpace(LetraCodigo) && !string.IsNullOrWhiteSpace(NumeroCodigo) && !string.IsNullOrWhiteSpace(Producto.codigoProducto) && !string.IsNullOrWhiteSpace(Producto.nombre) &&
                !string.IsNullOrWhiteSpace(Producto.unidadVenta) && !string.IsNullOrWhiteSpace(Producto.seccion))
            {
                if (string.IsNullOrEmpty(Producto.codigoBarras))
                {
                    Producto.codigoBarras = "";
                }
                if (string.IsNullOrEmpty(Producto.iva.ToString()))
                {
                    Producto.iva = 0;
                }
                try
                {
                    if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                    {
                        Task<object> re = conexion.CallServerMethod("ServidorNuevoProducto", Arguments: new[] { Producto });
                        await re;
                        if ((re.Result.ToString()) == "Codigo de producto ya registrado.")
                        {
                            MessageBox.Show($"Codigo de producto ya registrado.");
                            LetraCodigo = "";
                            NumeroCodigo = "";
                            return;
                        }
                        if ((re.Result.ToString()) == "Codigo de barras ya registrado.")
                        {
                            MessageBox.Show($"Codigo de barras ya registrado.");
                            CodigoBarras = "";
                            return;
                        }
                        if ((re.Result.ToString()) == "Nombre de producto ya registrado.")
                        {
                            MessageBox.Show($"Nombre de producto ya registrado.");
                            Nombre = "";
                            return;
                        }

                        if ((re.Result.ToString()).Substring(0,10) == "El usuario")
                        {
                            MessageBox.Show(re.Result.ToString());
                            VentanaPrincipal.ActivateItem(new ProductoNuevoViewModel(VentanaPrincipal));
                            return;
                        }
                        MessageBox.Show(re.Result.ToString());
                    }
                    else
                    {
                        MessageBox.Show("Para agregar un nuevo producto debe estar conectado al servidor.");
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            else
            {
                MessageBox.Show("Primero debe rellenar los datos.");
            }
        }

        public string Error { get { return null; } }
        int flag = 0;
        public string this[string name]
        {
            get
            {
                string result = null;
                if (flag == 8)
                {
                    if (name == "LetraCodigo")
                    {
                        if (String.IsNullOrEmpty(LetraCodigo))
                        {
                            result = "Este campo no puede estar vacío.";
                        }
                    }
                    else if (name == "NumeroCodigo")
                    {
                        if (String.IsNullOrEmpty(NumeroCodigo))
                        {
                            result = "Este campo no puede estar vacío.";
                        }
                    }
                    else if (name == "Nombre")
                    {
                        if (String.IsNullOrEmpty(Nombre))
                        {
                            result = "Este campo no puede estar vacío.";
                        }
                    }
                    else if (name == "UnidadVenta")
                    {
                        if (String.IsNullOrEmpty(UnidadVenta))
                        {
                            result = "Este campo no puede estar vacío.";
                        }
                    }
                    else if (name == "Seccion")
                    {
                        if (String.IsNullOrEmpty(Seccion))
                        {
                            result = "Este campo no puede estar vacío.";
                        }
                    }
                    else if (name == "FechaVencimiento")
                    {
                        if (FechaVencimiento < DateTime.Today )
                        {
                            result = "Verifique la fecha ingresada.";
                        }
                    }
                    else if (name == "IVA")
                    {
                        if ( IVA > 100 )
                        {
                            result = "Este valor no puede ser mayor a 100%.";
                        }
                    }
                }
                else { flag += 1; }
                return result;
            }
        }
    }
}
