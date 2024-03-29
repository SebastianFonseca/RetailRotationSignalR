﻿using Autofac;
using Client.Main.Utilities;
using Client.Main.ViewModels;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;


namespace Client.Main
{

    public  class Connect
    {

        private static HubConnection hubConnection;

        public HubConnection Connection
        {
            get { return hubConnection; }
        }

       static string  Usuario;
        /// <summary>
        /// Starts the connection with te server, the User and HashedPassword info are into the token.
        /// </summary>
        /// <param name="User"></param>
        /// <param name="HashedPassword"></param>
        public static async Task<object> /*void*/ ConnectToServer(string User)
        {
            Usuario = User;
            try
            {
                if (hubConnection == null || hubConnection.State == HubConnectionState.Disconnected || hubConnection.State == HubConnectionState.Connecting)
                {
                    hubConnection = new HubConnectionBuilder() 
                    .WithUrl(GetServerAddress(), options =>
                    {

                         options.Transports = HttpTransportType.WebSockets;

                         options.SkipNegotiation = true;

                         options.AccessTokenProvider = () =>
                         {
                             IJWTContainerModel model = GetJWTContainerModel(User);

                             IJWTService authService = new JWTService(model.SecretKey);

                             var token = authService.GenerateToken(model);

                             return Task.FromResult(token);

                             static JWTContainerModel GetJWTContainerModel(string name)
                             {
                                 return new JWTContainerModel()
                                 {
                                     Claims = new Claim[]
                                     {
                                          new Claim(ClaimTypes.Name, name),
                                     }
                                 };
                             }
                         };

                     }


                     ).Build(); /*WithAutomaticReconnect().*/

                    hubConnection.Closed += HubConnection_Closed;

                    hubConnection.Reconnecting += HubConnection_Closed;

                    hubConnection.Reconnected += HubConnection_Reconnected;

                    Task t = hubConnection.StartAsync();

                    await t;
                    return t;
                }
            }
            catch (Exception es)
            {
             
               // MessageBox.Show(es.Message + "ConnectToServer" + es.InnerException.Message );   return null;
            }
            return null;
        }

        /// <summary>
        ///  Metodo suscrito al evento que se dispara cuando se cierra la conexion
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private static Task HubConnection_Closed(Exception arg)
        {
            MainWindowViewModel.Status = "Trabajando localmente";
            return null;

        }


        /// <summary>
        /// Metodo suscrito al evento que se dispara cuando el cliente se reconecta
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private static async Task HubConnection_Reconnected(string arg)
        {
            Connect conexion = ContainerConfig.scope.Resolve<Connect>();
            MainWindowViewModel.Status = "Conectado al servidor";
            await conexion.CallServerMethod("ClienteReconectado", Arguments: new[] { Usuario });


        }


        /// <summary>
        /// Invoke a specific method on the server side.
        /// </summary>
        /// <param name="MethodName">The method name to invoke.</param>
        /// <param name="Arguments">The needed arguments fot the specified method.</param>
        public async Task<object> CallServerMethod(string MethodName, Object[] Arguments)
        {
            if (/*hubConnection.ConnectionId != null && hubConnection.State != HubConnectionState.Connecting*/ hubConnection.State == HubConnectionState.Connected) { 
                try
                {
                   /// MessageBox.Show(Connection.State.ToString());
                    Task<object> result = hubConnection.InvokeCoreAsync<object>(MethodName, args: Arguments);
                    await result ;
                    
                    if (/*hubConnection.ConnectionId != null &&*/ hubConnection.State != HubConnectionState.Connecting && result.Status == TaskStatus.RanToCompletion )
                    {
                        return result.Result;
                    }
                }            
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return null;
                }
            }
            return null;

        }

        


        /// <summary>
        /// Return the server address from the App.config file
        /// </summary>
        /// <returns></returns>
        static string GetServerAddress()
        {
            return ConfigurationManager.AppSettings["ServerAddress"];
        }
    }
}


//private static async Task OnClosed(Exception arg)
//{
//   Connect conexion = ContainerConfig.scope.Resolve<Connect>();
//   MainWindowViewModel.Status = "Trabajando localmente";

//   await  conexion.CallServerMethod("ClienteDesconectado", Arguments: new[] { Usuario });
//    if (arg.Message.Substring(0, 61) == "Reconnect retries have been exhausted after 5 failed attempts")
//    {
//        while (hubConnection.State == HubConnectionState.Disconnected)
//        {
//            try
//            {
//                await Task.Delay(TimeSpan.FromSeconds(0.5));
//                //MessageBox.Show(hubConnection.State.ToString());
//                await hubConnection.StartAsync();
//                MainWindowViewModel.Status = "Conectado al servidor";
//                await conexion.CallServerMethod("ClienteReconectado", Arguments: new[] { Usuario });



//            }
//            catch (Exception e)
//            {
//                if (e.Message == "No se puede establecer una conexión ya que el equipo de destino denegó expresamente dicha conexión.")
//                {
//                   MainWindowViewModel.Status = "Trabajando localmente";
//                }
//                else
//                    MessageBox.Show(e.Message);
//            }
//        }
//    }

//}
