using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServerConsole
{
    public class RetailHUB : Hub
    {
        public async Task SendMessage(string user, string password)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", user, password);
        }
    }

}
