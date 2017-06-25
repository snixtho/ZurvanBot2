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
            
            var gateway = new GatewayListener("wss://gateway.discord.gg/", "MjQzMDg0Nzc2NzkyMTI5NTM2.DDA97w.PH7KBiIIygTw3UXhU6F4Fg3r7vQ");
            
            gateway.OnReadyEvent += eventArgs =>
            {
                Log.Debug("event handler 1");
            };
            
            gateway.OnReadyEvent += eventArgs =>
            {
                Log.Debug("event handler 2");
            };
            
            gateway.OnReadyEvent += eventArgs =>
            {
                Log.Debug("event handler 3");
            };
            
            gateway.StartListeningAsync().Wait();
            Log.Info("Closed.");
        }
    }
}
