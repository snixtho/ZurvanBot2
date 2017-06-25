using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using ZurvanBot.Discord;
using ZurvanBot.Discord.Gateway;
using ZurvanBot.Discord.Resources.Params;
using ZurvanBot.Util;
using ZurvanBot.Util.HTTP;

namespace ZurvanBot
{
    internal class Program
    {
        public static void Main(string[] args)
        {   
            // Console.WriteLine(DateTime.Now.ToString("dd.MM.yyyy-HH:mm:ss"));
            
            var gateway = new GatewayListener("wss://gateway.discord.gg/", "MzI4NjI0OTM4MzkzMjcyMzMx.DDGnXg.nx-9o5TEoW8dwFMwADoXqHtmzjk");
            
            gateway.OnMessageCreateEvent += eventArgs =>
            {
                Log.Info(eventArgs.Message.author.username + ": " + eventArgs.Message.content, "message");
            };
            
            gateway.StartListeningAsync().Wait();
            Log.Info("Closed.");
        }
    }
}
