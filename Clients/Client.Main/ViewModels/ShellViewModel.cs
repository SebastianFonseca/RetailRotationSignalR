using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main.ViewModels
{
    public class ShellViewModel : Conductor<object>/*, IDataErrorInfo*/
    {
        ///Objeto responsable de la administracion de las ventanas.
        private readonly IWindowManager window = new WindowManager();

        ///Clase responsable de la conexion al servidor obteniendo la instanca unica (Autofac) 
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();

        ///Instancia de la clase usuario usada para obtener la informacion del usuario logueado.
        public EmpleadoModel usuario = new EmpleadoModel();

        ///Propiedades enlazadas con el textbox y el passwordbox  de la vista.        
        public string User
        {
            get => usuario.cedula;
            set
            {
                usuario.cedula = value;
                NotifyOfPropertyChange(() => User);
            }
        }


        public string UserPassword
        {
            get => usuario.password;
            set
            {
                usuario.password = value;
                NotifyOfPropertyChange(() => UserPassword);
            }
        }

        private string _stackVisibility = "Collapsed";
        public string StackVisibility
        {
            get { return _stackVisibility; }
            set
            {
                _stackVisibility = value;
                NotifyOfPropertyChange(() => StackVisibility);
            }
        }

        ///string Status_conexion = "Trabajando localmente";


        public ShellViewModel()
        {
    

        }

        bool flag2 = true;



        public string CanEntrar (string truefalse = "True"){ return truefalse; }

        public async void Entrar()
        {
            if (UserPassword == null | User  == null)
            {
                MessageBox.Show("Ingrese un usuario y una contraseña");
                return;
            }
            CanEntrar("False");
            StackVisibility = "Visible";

            try
            {

                ///Si ya hubo un intento de logueo y esta conectado al servidor.
                if (conexion.Connection != null && conexion.Connection.State == HubConnectionState.Connected )
                {
                    Task<object> re = conexion.CallServerMethod("ServidorValidarUsuario", Arguments: new[] { usuario.cedula, usuario.password });
                    await re;
                    object[] respuesta = System.Text.Json.JsonSerializer.Deserialize<object[]>(re.Result.ToString());
                    EmpleadoModel usr = new EmpleadoModel();
                    if (respuesta.Length == 2)
                    {
                        usr = System.Text.Json.JsonSerializer.Deserialize<EmpleadoModel>(respuesta[1].ToString());
                    }

                    if ((string)respuesta[0].ToString() == "Contraseña incorrecta.")
                    {
                        MessageBox.Show("Usuario o contraseña incorrectos");
                        return;
                    }
                    if ((string)respuesta[0].ToString() == "Registrado")
                    {
                        window.ShowWindow(new MainWindowViewModel(usr));                                
                        this.TryClose();
                        return;
                    }
                    else
                    {
                        MessageBox.Show(respuesta.ToString());
                    }
                }
                


                ///Para intentar loguarse por primera vez y conectarse al servidor.
                await Connect.ConnectToServer(User, "Admin");
                if (flag2)
                {
                    conexion.Connection.On("SetStatus", handler: (string a) =>
                    {
                        StackVisibility = "Collapsed";
                        MainWindowViewModel.Status = "Conectado al servidor";
                        MessageBox.Show(a);
                    });
                    conexion.Connection.On("SetStatusDisconnected", handler: (string a) =>
                    {
                        MainWindowViewModel.Status = "Trabajando localmente";
                        MessageBox.Show(a);
                    });
                    flag2 = false;
                }


                await conexion.CallServerMethod("TestMethod", Arguments: new[] { "Conectado al sevidor." });

                if (MainWindowViewModel.Status == "Conectado al servidor" & conexion.Connection.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected)
                {
                    Task<object> re = conexion.CallServerMethod("ServidorValidarUsuario", Arguments: new[] { User, UserPassword });
                    await re;

                    object[] respuesta = System.Text.Json.JsonSerializer.Deserialize<object[]>(re.Result.ToString());
                    EmpleadoModel usr = new EmpleadoModel();
                    if (respuesta.Length == 2) 
                    { 
                            usr = System.Text.Json.JsonSerializer.Deserialize<EmpleadoModel>(respuesta[1].ToString());
                    }

                    if ((string)respuesta[0].ToString() == "Registrado")
                    {
                        window.ShowWindow(new MainWindowViewModel(usr));
                        this.TryClose();
                        return;
                    }
                    else
                    {
                        MessageBox.Show("El nombre de usuario o la contraseña son incorrectos.");
                        return;
                    }
                }

                StackVisibility = "Collapsed";
                object[] verificar = DbConnection.Login(User: User, Password: UserPassword);
                if ((string)verificar[0] == "Registrado")
                {
                    window.ShowWindow(new MainWindowViewModel((EmpleadoModel)verificar[1]));
                    this.TryClose();
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectas.");
                }

            }
            catch (Exception e)
            { MessageBox.Show(e.Message); }

        }



    }
}

