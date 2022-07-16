using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main.ViewModels
{
    public class MovimientoEfectivoNuevoEgresoViewModel:Screen, System.ComponentModel.IDataErrorInfo
    {
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        EgresoModel egreso;
        public MovimientoEfectivoNuevoEgresoViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
            egreso = new EgresoModel()
            {
                responsable = VentanaPrincipal.usuario,
                local = VentanaPrincipal.usuario.puntoDeVenta,
                fecha = DateTime.Now
            };
            try
            {
                getIdEgreso();
                getItemsEgresos();
                getProveedores();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        private string _id;

        public string Id
        {
            get { return _id; }
            set 
            { 

                _id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        public async void getIdEgreso()
        {
            if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
            {
                Task<object> re = conexion.CallServerMethod("ServidorgetNextIdEgreso", Arguments: new object[] { });
                await re;
                
                Id = ConfigurationManager.AppSettings["Caja"].Split(':')[0] + ":" +  re.Result.ToString();
                egreso.id = Id;
            }
        }

        private BindableCollection<ItemMovimientoEfectivoModel> _items;

        public BindableCollection<ItemMovimientoEfectivoModel> Items
        {
            get { return _items; }
            set 
            { 
                _items = value;
                NotifyOfPropertyChange(() => Items);

            }
        }

        private ItemMovimientoEfectivoModel _itemSeleccionado;

        public ItemMovimientoEfectivoModel ItemSeleccionado
        {
            get { return _itemSeleccionado; }
            set 
            {
                _itemSeleccionado = value;
                egreso.itemMovimientoefectivo = value;
                NotifyOfPropertyChange(() => ItemSeleccionado);

            }
        }

        public async void getItemsEgresos()
        {

            if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
            {
                Task<object> re = conexion.CallServerMethod("ServidorgetItemsEgresos", Arguments: new object[] { });
                await re;
                Items = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ItemMovimientoEfectivoModel>>(re.Result.ToString());
            }
        }

        private decimal _valor;

        public decimal Valor
        {
            get
            { 
                return _valor;
            }
            set 
            {
                egreso.valor = value;
                _valor = value;
                NotifyOfPropertyChange(() => Valor);

            }
        }

        private BindableCollection<ProveedorModel> proveedorModel;

        public BindableCollection<ProveedorModel> Proveedores
        {
            get { return proveedorModel; }
            set 
            {
                proveedorModel = value;
                NotifyOfPropertyChange(() => Proveedores);

            }
        }

        private ProveedorModel proveedor;

        public ProveedorModel Proveedor
        {
            get 
            { 
                return proveedor;
            }
            set 
            {
                egreso.proveedor = value;
                proveedor = value;
                NotifyOfPropertyChange(() => Proveedor);

            }

        }

        public async void getProveedores()
        {

            if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
            {

                Task<object> re = conexion.CallServerMethod("ServidorGetTodosProveedor", Arguments: new object[] { });
                await re;
                Proveedores = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ProveedorModel>>(re.Result.ToString());
            }            

        }

        private string _soporte;

        public string Soporte
        {
            get { return _soporte; }
            set 
            {
                egreso.soporte = value;
                _soporte = value;
                NotifyOfPropertyChange(() => Soporte);

            }
        }


        private string _descripcion;

        public string Descripcion
        {
            get 
            { 
                return _descripcion; 
            }
            set
            {
                egreso.descripcion = value;
                _descripcion = value;
                NotifyOfPropertyChange(() => Descripcion);

            }
        }

        public void BackButton() 
        {
            VentanaPrincipal.ActivateItem(new MovimientoEfectivoViewModel(VentanaPrincipal));
        }

        public async void Guardar()
        {
            if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
            {
                Task<object> re = conexion.CallServerMethod("ServidorNuevoEgreso", Arguments: new object[] {egreso});
                await re;
                if (re.Result.ToString() == "true") 
                { 
                    MessageBox.Show("Se registro el nuevo egreso");
                    VentanaPrincipal.ActivateItem(new MovimientoEfectivoViewModel(VentanaPrincipal));
                }
            }

        }




        public string Error { get { return null; } }
        int flag = 0;
        public string this[string name]
        {
            get
            {
                string result = null;
                if (flag == 6)
                {
                    if (name == "ItemSeleccionado")
                    {
                        if (ItemSeleccionado == null)
                        {
                            result = "Selecciones un ítem.";
                        }
                    }
                    else if (name == "Valor")
                    {
                        if (Valor == 0)
                        {
                            result = "Escriba un valor.";
                        }

                    }

                    else if (name == "Proveedor")
                    {
                        if (Proveedor == null)
                        {
                            result = "Eleccione un proveedor.";
                        }

                    }
                    else if (name == "Soporte")
                    {
                        if (String.IsNullOrEmpty(Soporte))
                        {
                            result = "Escriba un número de soporte";
                        }


                    }
                    else if (name == "Descripcion")
                    {
                        if (String.IsNullOrEmpty(Descripcion))
                        {
                            result = "Escriba una descripcion";
                        }

                    }                   
                }

                else { flag += 1; }


                return result;
            }
        }


    }
}

