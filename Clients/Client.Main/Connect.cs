using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
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

        static   Connect()
        {
            hubConnection = new HubConnectionBuilder().WithUrl("http://localhost:5000/retailHub").Build();
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
