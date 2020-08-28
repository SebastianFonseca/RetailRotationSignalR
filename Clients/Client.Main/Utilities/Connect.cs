using Client.Main.Utilities;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;


namespace Client.Main
{
    public class Connect
    {

        private static HubConnection hubConnection;
        public HubConnection Connection
        {
            get { return hubConnection; }
        }


        public static void LogIntoServer(string User, string hashedPassword)
        {
            try
            {


                if (hubConnection == null)
                {
                    hubConnection = new HubConnectionBuilder()
                    .WithUrl("http://localhost:5000/retailHub", options =>

                     options.AccessTokenProvider = () =>
                     {
                         IJWTContainerModel model = GetJWTContainerModel(User, hashedPassword);

                         IJWTService authService = new JWTService(model.SecretKey);

                         var token = authService.GenerateToken(model);

                         //MessageBox.Show(token);

                         return Task.FromResult(token);



                         static JWTContainerModel GetJWTContainerModel(string name, string password)
                         {
                             return new JWTContainerModel()
                             {
                                 Claims = new Claim[]
                             {
                                 new Claim(ClaimTypes.Name, name),
                                 new Claim(ClaimTypes.Hash, password)
                                }
                             };
                         }

                     }

                     ).Build();
                }
            }
            catch (Exception es)
            {

                MessageBox.Show(es.Message);
            }
        


        }
         
        public async void ConnectServer(string MethodName, Object[] Arguments)
        {            
            try
            {
                if (hubConnection.State == HubConnectionState.Disconnected)
                    await hubConnection.StartAsync();

                await hubConnection.InvokeCoreAsync(MethodName, args: Arguments);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
