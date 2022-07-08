using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using Client.Main.Views;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main.ViewModels
{
    public class MovimientoEfectivoNuevoItemViewModel : Conductor<object>
    {
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        ItemMovimientoEfectivoModel item = new ItemMovimientoEfectivoModel();
        MainWindowViewModel VentanaPrincipal;

        public MovimientoEfectivoNuevoItemViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
            getNextId();
        }

        public async void getNextId()
        {
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    Task<object> re = conexion.CallServerMethod("ServidorgetNextIdMovimientoEfectivo", Arguments: new object[] { });
                    await re;
                    Codigo = re.Result.ToString();
                }
                if (MainWindowViewModel.Status == "Trabajando localmente")
                {
                   // MessageBox.Show("Solo se pueden agragar ítems cuando este conectado al servidor");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private string _codigo;

        public string Codigo
        {
            get { return _codigo; }
            set 
            {
                _codigo = value;
                NotifyOfPropertyChange(() => Codigo);
            }
        }


        private string _descripcion;

        public string Descripcion
        {
            get { return _descripcion; }
            set
            { 
                _descripcion = value;
                item.descripcion  = value;
                NotifyOfPropertyChange(()=>Descripcion);
            }
        }

        private bool _ingreso;

        public bool Ingreso
        {
            get { return _ingreso; }
            set 
            {
                if (value) item.tipo = "Ingreso";
                _ingreso = value;
                NotifyOfPropertyChange(() => Ingreso);

            }
        }

        private bool _egreso;

        public bool Egreso
        {
            get { return _egreso; }
            set 
            {
                if (value) item.tipo = "Egreso";
                _egreso = value;
                NotifyOfPropertyChange(() => Egreso);

            }
        }

        public async void Guardar()
        {
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    Task<object> re = conexion.CallServerMethod("ServidornuevoItemMovimientoEfectivo", Arguments: new object[] { item });
                    await re; 
                    if(re.Result.ToString() == "true")
                    {
                        MessageBox.Show("Se creo el nuevo ítem");
                        this.TryClose();
                    }
                }
                if (MainWindowViewModel.Status == "Trabajando localmente")
                {
                    MessageBox.Show("Solo se pueden agragar ítems cuando este conectado al servidor");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

    }
}
