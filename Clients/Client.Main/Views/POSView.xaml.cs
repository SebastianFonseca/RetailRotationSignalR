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
using System.Windows.Shapes;

namespace Client.Main.Views
{
    /// <summary>
    /// Interaction logic for POSView.xaml
    /// </summary>
    public partial class POSView : Window
    {
        public POSView()
        {
            InitializeComponent();
        }

        private void LetterValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-Z]+$");
            e.Handled = regex.IsMatch(e.Text);
        }



    }
}
