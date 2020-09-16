using Client.Main.Utilities;
using Client.Main.ViewModels;
using Microsoft.AspNetCore.SignalR.Client;
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


namespace Client.Main
{

    public  class Connect
    {

        private static HubConnection hubConnection;

        public HubConnection Connection
        {
            get { return hubConnection; }
        }
        /// <summary>
        /// Starts the connection with te server, the User and Role info are into the token.
        /// </summary>
        /// <param name="User"></param>
        /// <param name="Role"></param>
        public static async void ConnectToServer(string User, string Role)
        {
            try
            {
                if (hubConnection == null)
                {
                    hubConnection = new HubConnectionBuilder()
                    .WithUrl(GetServerAddress(), options =>

                     options.AccessTokenProvider = () =>
                     {
                         IJWTContainerModel model = GetJWTContainerModel(User, Role);

                         IJWTService authService = new JWTService(model.SecretKey);

                         var token = authService.GenerateToken(model);
                        
                         return Task.FromResult(token);

                         static JWTContainerModel GetJWTContainerModel(string name, string Role = "Admin")
                         {
                             return new JWTContainerModel()
                             {
                                Claims = new Claim[]
                                {
                                     new Claim(ClaimTypes.Name, name),
                                     new Claim(ClaimTypes.Role, Role)
                                }
                             };
                         }
                     }
                     ).WithAutomaticReconnect().Build();
                    hubConnection.Closed += HubConnection_Closed;
                    Task t = hubConnection.StartAsync();
                    await t;
                }
            }
            catch ///(Exception es)
            {
                return;
                ///MessageBox.Show(es.Message + "ConnectToServer");
            }

        }
        public async Task<object> CallServerMethod(string MethodName, Object[] Arguments)
        {            
            try
            {
            Task result = hubConnection.InvokeCoreAsync(MethodName, args: Arguments);
                await result;
             if (hubConnection.ConnectionId != null && hubConnection.State != HubConnectionState.Connecting)
            {
                return result;
            }    
            }
            catch
            {
                return null;
            }
            return null;

        }
        private static  async Task HubConnection_Closed(Exception arg)
        {
            Statics.ClientStatus = "Trabajando localmente";
            
            MessageBox.Show("Trabajando localmente, se intentara reestablecer la conexión en segundo plano.");
            try
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await hubConnection.StartAsync();

            }
            catch (Exception)
            {

                throw;
            }

        }


        /// <summary>
        /// Invoke a specific method on the server side.
        /// </summary>
        /// <param name="MethodName">The method name to invoke.</param>
        /// <param name="Arguments">The needed arguments fot the specified method.</param>


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
