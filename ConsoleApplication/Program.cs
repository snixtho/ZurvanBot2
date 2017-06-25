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
            
            var gateway = new GatewayListener("wss://gateway.discord.gg/", "token");

            gateway.OnMessageCreate += eventArgs =>
            {
                Log.Info("'" + eventArgs.Message.content + "'");
                
                if (eventArgs.Message.content.Trim().ToLower().Equals("leave voice"))
                {
                    gateway.DisconnectFromVoice(130641144705974272);
                } else if (eventArgs.Message.content.Trim().ToLower().Equals("join voice"))
                {
                    gateway.JoinVoiceChannel(130641144705974272, 130641144705974273);
                }
            };
            
            /* gateway.OnMessageCreate += eventArgs =>
            {
                Log.Info(eventArgs.Message.author.username + ": " + eventArgs.Message.content, "message");
            }; */
            
            gateway.StartListeningAsync().Wait();
            Log.Info("Closed.");
        }
    }
}
