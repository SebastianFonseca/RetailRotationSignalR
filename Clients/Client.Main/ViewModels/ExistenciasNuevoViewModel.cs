using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using Client.Main.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main.Views
{
    public class ExistenciasNuevoViewModel : Screen
    {
        MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();

        public ExistenciasModel existencias;

        public ExistenciasNuevoViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
            existencias = new ExistenciasModel()
            {
                codigo = DateTime.Now.ToString("dd''MM''yyyy''HH''mm''ss") + VentanaPrincipal.usuario.puntoDeVenta.codigo,
                responsable = VentanaPrincipal.usuario,
                puntoVenta = VentanaPrincipal.usuario.puntoDeVenta,
            };
            getprod();

        }

        public async void getprod()
        {
            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {
                    Task<object> re = conexion.CallServerMethod("ServidorgetIdProductos", Arguments: new object[] { });
                    await re;
                    Productos = System.Text.Json.JsonSerializer.Deserialize<BindableCollection<ProductoModel>>(re.Result.ToString());
                }
                if (MainWindowViewModel.Status == "Trabajando localmente")
                {
                    Productos = DbConnection.getProductos();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        public BindableCollection<ProductoModel> Productos
        {
            get => existencias.productos;
            set
            {
                existencias.productos = value;
                NotifyOfPropertyChange(() => Productos);
            }

        }

        public string Codigo
        {
            get => existencias.codigo;
            set
            {
                existencias.codigo = value;
                NotifyOfPropertyChange(() => Codigo);
            }
        }

        private string _dia = DateTime.Today.Day.ToString();

        public string Dia
        {

            get => _dia;
            set
            {
                short a;
                if (Int16.TryParse(value, out a))
                {
                    if (Int32.Parse(value) < DateTime.Now.Day)
                    {
                        _dia = value;
                        NotifyOfPropertyChange(() => Dia);
                    }


                }
            }
        }

        private string _mes = DateTime.Today.Month.ToString();

        public string Mes
        {
            get => _mes;
            set
            {
                short a;
                if (Int16.TryParse(value, out a))
                {
                    if (Int32.Parse(value) <= DateTime.Now.Month)
                    {
                        _mes = value;
                        NotifyOfPropertyChange(() => Mes);
                    }
                }

            }
        }
        private string _año = DateTime.Today.Year.ToString();

        public string Año
        {
            get => _año;
            set
            {
                short a;
                if (Int16.TryParse(value, out a))
                {
                    if (Int32.Parse(value) < DateTime.Now.Year)
                    {
                        _año = value;
                        NotifyOfPropertyChange(() => Año);

                    }
                }

            }
        }

        public EmpleadoModel Responsable
        {
            get => existencias.responsable;
            set
            {
                existencias.responsable = value;
                NotifyOfPropertyChange(() => Responsable);

            }
        }

        public void BackButton()
        {
            Productos.Clear();
            VentanaPrincipal.ActivateItem(new AdministracionInventarioViewModel(VentanaPrincipal));
        }
        public async void Guardar()
        {
            try
            {
                existencias.fecha = new DateTime(Int32.Parse(Año), Int32.Parse(Mes), Int32.Parse(Dia));

            }
            catch (Exception e)
            {
                MessageBox.Show("Verifique la fecha ingresada \n" + e.Message);
                return;
            }
            foreach (ProductoModel producto in Productos)
            {
                if (producto.existencia == null)
                {
                    MessageBox.Show($"Registre el valor corespondiente a {producto.nombre}");
                    return;
                }
            }

            if (string.IsNullOrEmpty(Dia) || string.IsNullOrEmpty(Mes) || string.IsNullOrEmpty(Año))
            {
                MessageBox.Show("Ingrese una fecha");
                return;
            }

            try
            {
                if ((MainWindowViewModel.Status == "Conectado al servidor") & (conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected))
                {

                    Task<object> re = conexion.CallServerMethod("ServidorNuevaExistencia", Arguments: new[] { existencias });
                    await re;
                    if (re.Result.ToString() == "Se ha registrado el nuevo documento.")
                    {
                        MessageBox.Show(re.Result.ToString());
                        VentanaPrincipal.ActivateItem(new AdministracionInventarioViewModel(VentanaPrincipal));
                        return;
                    }
                    if (re.Result.ToString() == "Existencia ya registrada.")
                    {
                        MessageBox.Show("Existencia ya registrada");
                    }
                    MessageBox.Show(re.Result.ToString());
                }
                else
                {
                    if (MainWindowViewModel.Status == "Trabajando localmente")
                    {
                        MessageBox.Show(Utilities.DbConnection.NuevaExistencia(existencias));
                        VentanaPrincipal.ActivateItem(new AdministracionInventarioViewModel(VentanaPrincipal));
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

    }
}
