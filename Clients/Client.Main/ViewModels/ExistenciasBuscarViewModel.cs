﻿using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main.ViewModels
{
    public class ExistenciasBuscarViewModel: Screen
    {

        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();
        public ExistenciasBuscarViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;

        }

        private bool _buscartbxb = true;

        public bool Buscartbxb
        {
            get { return _buscartbxb; }
            set
            {
                _buscartbxb = value;
                NotifyOfPropertyChange(() => Buscartbxb);
            }
        }



        private string _buscarTbx;
        public string BuscarTbx
        {
            get => _buscarTbx;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Buscartbxfechab = false;
                    if (!string.IsNullOrEmpty(BuscarTbxFecha))
                        Buscartbxb = true;
                }
                else
                {
                    Buscartbxfechab = true;

                }
                _buscarTbx = value;
                NotifyOfPropertyChange(() => BuscarTbx);
            }
        }


        private bool _buscartbxfechab = true;

        public bool Buscartbxfechab
        {
            get { return _buscartbxfechab; }
            set 
            { 
                _buscartbxfechab = value;
                NotifyOfPropertyChange(() => Buscartbxfechab);
            }
        }

        private string _buscarTbxFecha;

        public string BuscarTbxFecha
        {
            get { return _buscarTbxFecha; }
            set 
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Buscartbxb = false;
                }
                else
                
                {
                    Buscartbxb = true;

                }

                _buscarTbxFecha = value;
                NotifyOfPropertyChange(() => BuscarTbxFecha);

            }
        }



        private BindableCollection<ExistenciasModel> _busquedas = new BindableCollection<ExistenciasModel>();
        public BindableCollection<ExistenciasModel> Busquedas
        {
            get => _busquedas;
            set
            {
                _busquedas = value;
                NotifyOfPropertyChange(() => Busquedas);
            }
        }


        ExistenciasModel Seleccionada;

        private ExistenciasModel _existenciaSeleccionada;
        public ExistenciasModel ExistenciaSeleccionada
        {
            get => _existenciaSeleccionada;
            set
            {
                if (value != null)
                {
                    Seleccionada = value;
                    BuscarTbx = value.codigo + " " + value.fecha.ToString("yyyy-MM-dd") ;
                    BusquedasVisibilidad = "Hidden";
                }
                _existenciaSeleccionada = value;

                NotifyOfPropertyChange(() => ExistenciaSeleccionada);
            }
        }

        public async void Buscar()
        {
            if (String.IsNullOrEmpty(BuscarTbx) && string.IsNullOrEmpty(BuscarTbxFecha))
            {
                MessageBox.Show("Rellene alguno de los campos.");
                return;
            }
            else
            {
                if (Seleccionada == null)
                {
                    MessageBox.Show("Seleccione alguna de las busquedas");
                    return;
                }
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    try
                    {
                        Task<object> re = conexion.CallServerMethod("servidorGetProductoExistencia", Arguments: new[] { Seleccionada.codigo });
                        await re;

                        Seleccionada.productos = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ProductoModel>>(re.Result.ToString());


                        VentanaPrincipal.ActivateItem(new ExistenciaResultadoBusquedaViewModel(VentanaPrincipal, Seleccionada));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
                else if ((MainWindowViewModel.Status == "Trabajando localmente"))
                {
                    Seleccionada.productos = DbConnection.getProductoExistencia(Seleccionada.codigo);
                    VentanaPrincipal.ActivateItem(new ExistenciaResultadoBusquedaViewModel(VentanaPrincipal, Seleccionada));

                }

            }
        }

        public void BackButton()
        {
            if (Busquedas.Count != 0)
            {
                Busquedas.Clear();
            }

            VentanaPrincipal.ActivateItem(new AdministracionInventarioViewModel(VentanaPrincipal));
        }

        public string Error => null;
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
                if (String.IsNullOrEmpty(BuscarTbx) && string.IsNullOrEmpty(BuscarTbxFecha)) { return "Hidden"; }
                return _busquedasVisibiliad;
            }
            set
            { 
                _busquedasVisibiliad = value;
                NotifyOfPropertyChange(() => BusquedasVisibilidad); 
            }
        }
        public async void EscribiendoBusqueda()
        {
            if (string.IsNullOrEmpty(BuscarTbx) & string.IsNullOrEmpty(BuscarTbxFecha))
            {
                BusquedasVisibilidad = "Hidden";
            }
            Busquedas.Clear();
            if (!string.IsNullOrEmpty(BuscarTbx))
            {
                try
                {
                    if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                    {
                        BindableCollection<ExistenciasModel> existencias = new BindableCollection<ExistenciasModel>();
                        Task<object> re = conexion.CallServerMethod("servidorGetExistencias", Arguments: new[] { BuscarTbx });
                        await re;

                        Busquedas = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ExistenciasModel>>(re.Result.ToString());

                    }
                    else if((MainWindowViewModel.Status == "Trabajando localmente"))
                    {
                        Busquedas = DbConnection.getExistencias(BuscarTbx);
                    }

                    if (Busquedas!= null & Busquedas.Count == 0)
                    {
                        BusquedasVisibilidad = "Hidden";
                    }
                    else
                    {
                        BusquedasVisibilidad = "Visible";
                    }

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            else
            { 
                if (BuscarTbxFecha != null && BuscarTbxFecha.Length == 10 )
                {
                    try
                    {
                        if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                        {
                            BindableCollection<ExistenciasModel> existencias = new BindableCollection<ExistenciasModel>();
                            Task<object> re = conexion.CallServerMethod("servidorGetExistencias", Arguments: new[] { BuscarTbxFecha });
                            await re;

                            Busquedas = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ExistenciasModel>>(re.Result.ToString());

                        }
                        else if ((MainWindowViewModel.Status == "Trabajando localmente"))
                        {
                            Busquedas = DbConnection.getExistencias(BuscarTbxFecha);
                        }

                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
                if ( Busquedas != null & Busquedas.Count == 0)
                {
                    BusquedasVisibilidad = "Hidden";
                }
                else
                {
                    BusquedasVisibilidad = "Visible";
                }
            }
        }
    }
}
