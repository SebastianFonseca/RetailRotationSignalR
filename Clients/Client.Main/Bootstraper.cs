using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Client.Main.ViewModels;
using Client.Main.Views;
using Autofac;
using Autofac.Core.Lifetime;
using System.Threading;
using System.Globalization;
using System.Windows.Markup;

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

            ///Se establece la cult-info IMPORTANTE pues cuado se hacen operaciones con decimales estos parametros son decisivos
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
                        XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.Name)));

        }

    }
}
