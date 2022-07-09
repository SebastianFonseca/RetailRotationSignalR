using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using Client.Main.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Client.Main.ViewModels
{

    public class POSViewModel: Screen
    {
        MainWindowViewModel VentanaPrincipal;

        public POSViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
        }

        private BindableCollection<ProductoModel> _productos = new BindableCollection<ProductoModel>() { new ProductoModel() {codigoProducto = "A1",nombre="Aguacate",precioVenta=3000,unidadVenta = "Kil." } };

        public BindableCollection<ProductoModel> Productos
        {
            get { return _productos; }
            set { _productos = value; }
        }


        public void getWidth()
        {
           // MessageBox.Show(System.Windows.SystemParameters.w .ToString());
        }
        

    }
}
