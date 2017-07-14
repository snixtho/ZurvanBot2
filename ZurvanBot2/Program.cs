using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using ZurvanBot.Discord;
using ZurvanBot.Discord.Gateway;
using ZurvanBot.Discord.Resources.Objects;
using ZurvanBot.Discord.Resources.Params;
using ZurvanBot.Util;

namespace ZurvanBot {
    internal class Program {
        public static void Main(string[] args) {
            // Log.Instance().LogLevel = Log.Elevation.Verbose;
            var auth = new Authentication(Authentication.AuthType.Bot,
                "MjQzMDg0Nzc2NzkyMTI5NTM2.DEkFWg._RTats2L7nIoLSJ-ZvKAQVTzRzw");
            var gateway = new GatewayListener("wss://gateway.discord.gg/", auth);

            gateway.OnMessageCreate += eventArgs => {
                if (eventArgs.Message.content.Equals("users")) {
                    gateway.State.ForEachUser((user, guildId) => {
                        Console.WriteLine("Username: " + user.user.username + "(" + guildId + ")");
                    });
                }
            };

            gateway.StartListeningAsync().Wait();
        }
    }
}