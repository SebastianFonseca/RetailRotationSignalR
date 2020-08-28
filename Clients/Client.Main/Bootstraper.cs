using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Client.Main.ViewModels;
using Client.Main.Views;
using Autofac;
using Autofac.Core.Lifetime;

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
