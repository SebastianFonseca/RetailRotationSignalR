using Autofac;
using Caliburn.Micro;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Main
{
    public static class ContainerConfig
    {
        static IContainer container = ContainerConfig.Configure();
        public static ILifetimeScope scope = container.BeginLifetimeScope();
        public static IContainer Configure()
        {

            var builder = new ContainerBuilder();

            builder.RegisterType<Connect>().SingleInstance();


            return builder.Build();

        }
    }
}
