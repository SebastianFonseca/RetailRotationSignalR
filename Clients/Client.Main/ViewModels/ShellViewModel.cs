﻿using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Views;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Composition;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace Client.Main.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        //Objeto responsable de la administracion de las ventanas.
        private readonly IWindowManager window = new WindowManager();

        //Instanciacion de la clase responsable de conectarse al servidor. 
        static Connect conexion = new Connect();        
        public ShellViewModel()
        { 
        }
        //Propiedades enlazadas con el textbox y el passwordbox  de la vista.
        public string User { get; set; }
        private string _password;
        public string UserPassword
        {
            get { return _password; }
            set { _password = value;
                NotifyOfPropertyChange(() => UserPassword);                
                NotifyOfPropertyChange(() => CanEntrar);
            }
        }
        //Expresion lambda encargada de activar/desactivar el boton 'Entrar' cuando los textbox se han llenado.
        public bool CanEntrar =>  !string.IsNullOrEmpty(User) && !string.IsNullOrEmpty(UserPassword);

        //Evento disparado al presionar el boton 'Entrar'
        public  void Entrar()

        {

            //Registro del metodo 'ClienteValidacion' responsable de validar los usuarios, cerrar la 'ShellView' y abrir la 'MainWindowView'.
            conexion.Connection.On("ClienteValidacion", (string Usr) =>
            {
                var model = new MainWindowViewModel(Usr);              
                window.ShowWindow(model);  this.TryClose();
            });

            try
            {
                conexion.ConnectServer("ServidorValidarUsuario", Arguments: new[] { User, UserPassword });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }




    }
}