﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.Main.Views
{
    /// <summary>
    /// Interaction logic for ProveedorNuevoView.xaml
    /// </summary>
    public partial class ProveedorNuevoView : UserControl
    {
        public ProveedorNuevoView()
        {
            InitializeComponent();
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Productos_Selected(object sender, RoutedEventArgs e)
        {

        }
    }
}
