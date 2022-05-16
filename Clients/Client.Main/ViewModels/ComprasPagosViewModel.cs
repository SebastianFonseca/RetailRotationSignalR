using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace Client.Main.ViewModels
{
    public class ComprasPagosViewModel:Screen
    {
        MainWindowViewModel VentanaPrincipal;
        public ComprasPagosViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
        }
    
    }
}
