﻿using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main.ViewModels
{
    class ProveedorBuscarViewModel : PropertyChangedBase, IDataErrorInfo
    {
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public ProveedorBuscarViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;

        }

        private string _buscarTbx;
        public string BuscarTbx
        {
            get { return _buscarTbx; }
            set
            {
                if (value == null)
                {
                    MessageBox.Show("Buscar null");
                }
                _buscarTbx = value;
                NotifyOfPropertyChange(() => BuscarTbx);
            }
        }

        private BindableCollection<ProveedorModel> _busquedas = new BindableCollection<ProveedorModel>();
        public BindableCollection<ProveedorModel> Busquedas
        {
            get
            {
                return _busquedas;
            }
            set
            {
                _busquedas = value;
                NotifyOfPropertyChange(() => Busquedas);
            }
        }

        private BindableCollection<ProveedorModel> _busquedasProducto = new BindableCollection<ProveedorModel>();
        public BindableCollection<ProveedorModel> BusquedasProducto
        {
            get { return _busquedasProducto; }
            set
            {
                _busquedasProducto = value;
                NotifyOfPropertyChange(() => BusquedasProducto);
            }
        }

        ProveedorModel seleccion = new ProveedorModel();

        private ProveedorModel _usuarioSeleccionado;
        public ProveedorModel UsuarioSeleccionado
        {
            get { return _usuarioSeleccionado; }
            set
            {
                if (value != null)
                {
                    seleccion = value;
                    BuscarTbx = value.cedula + "-" + value.firstName + " " + value.lastName;
                }
                _usuarioSeleccionado = value;

                NotifyOfPropertyChange(() => UsuarioSeleccionado);
            }
        }

        public async void Buscar()
        {
            if (String.IsNullOrEmpty(BuscarTbx))
            {
                MessageBox.Show("Escriba un nombre o número de cédula");
            }
            else
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    try
                    {
                        Task<object> re = conexion.CallServerMethod("ServidorGetProveedor", Arguments: new[] { BuscarTbx.Split('-')[0].Trim() });
                        await re;
                        ProveedorModel proveedor = System.Text.Json.JsonSerializer.Deserialize<ProveedorModel>(re.Result.ToString());    
                        if (proveedor.cedula == null)
                        {
                            MessageBox.Show("Número de cédula, nombre o apellido no resgistrados");
                        }
                        else
                        {                                                        
                            VentanaPrincipal.ActivateItem(new ProveedorResultadoBusquedaViewModel(VentanaPrincipal, proveedor));
                            BusquedasVisibilidad = "Visible";
//                            ComboboxDesplegado = "True";
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
                else
                {
                    MessageBox.Show("No es posible realizar la busqueda si no esta conectado al servidor.");
                }
            }
        }

        public void BackButton()
        {
            if(Busquedas.Count != 0) Busquedas.Clear();
            if(BusquedasProducto.Count != 0) BusquedasProducto.Clear();
            VentanaPrincipal.ActivateItem(new DC_AdministrativoViewModel(VentanaPrincipal));
        }

        public string Error { get { return null; } }
        int flag = 0;
        public string this[string name]
        {
            get
            {
                string result = null;
                if (flag == 2)
                {
                    if (name == "BuscarTbx")
                    {
                        if (String.IsNullOrEmpty(BuscarTbx))
                        {
                            result = "Escriba algun valor.";
                        }
                    }
                }
                else { flag += 1; }
                return result;
            }
        }



        private string _busquedasVisibiliad = "Hidden";

        public string BusquedasVisibilidad
        {
            get
            {
                if (String.IsNullOrEmpty(BuscarTbx)) { return "Hidden"; }
                return _busquedasVisibiliad;
            }
            set { _busquedasVisibiliad = value; NotifyOfPropertyChange(() => BusquedasVisibilidad); }
        }

        public async void EscribiendoBusqueda()
        {
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    BindableCollection<ProveedorModel> proveedores = new BindableCollection<ProveedorModel>();
                    Task<object> re = conexion.CallServerMethod("ServidorGetProveedores", Arguments: new[] { BuscarTbx });
                    await re;
                    //ProveedorModel[] mn = System.Text.Json.JsonSerializer.Deserialize<ProveedorModel[]>(re.Result.ToString());
                    
                     proveedores = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ProveedorModel>>(re.Result.ToString());

                    if (proveedores.Count != 0)
                    {
                        Busquedas.Clear();
                        BusquedasProducto.Clear();
                        if (proveedores[proveedores.Count-1].ciudad == "separador")
                        {
                            proveedores.RemoveAt(proveedores.Count - 1);
                            Busquedas = proveedores;

                        }
                        else if (proveedores[0].ciudad == "separador")
                        {
                            proveedores.RemoveAt(0);
                            BusquedasProducto = proveedores;
                        }
                        else
                        {
                            for (int i = 0; i <= proveedores.Count; i++)
                            {
                                if (proveedores[0].ciudad == "separador")
                                {
                                    proveedores.RemoveAt(0);
                                    break;
                                }
                                Busquedas.Add(proveedores[0]);
                                proveedores.RemoveAt(0);

                            }
                            BusquedasProducto = proveedores;
                        }
                    }                    
                    if ( Busquedas.Count == 0 & BusquedasProducto.Count == 0)
                    {
                        BusquedasVisibilidad = "Hidden";
                    }
                    else
                    {
                        BusquedasVisibilidad = "Visible";
                    }
                }
                else
                {
                    MessageBox.Show("No es posible realizar la busqueda si no esta conectado al servidor.");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
