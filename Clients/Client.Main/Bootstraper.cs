using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Caliburn.Micro;
using Client.Main.ViewModels;

namespace Client.Main
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}
