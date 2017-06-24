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
            using (var ws = new WebSocket ("wss://gateway.discord.gg/?v=5&encoding=json"))
            {
                /* var heartbeatInterval = 1000;
                var lastSeq = -1;
                var locker = new object();
                
                var heart = new Thread(() =>
                {
                    while (ws.IsAlive)
                    {
                        string s;
                        lock (locker)
                        {
                            s = @" {
                                ""op"": 1,
                                ""d"": " + (lastSeq == -1 ? 0 : lastSeq) + @"
                            }";
                        }
                        
                        Console.WriteLine("Heartbeat.");

                        if (ws.IsAlive)
                            ws.Send(s);
                        else
                            break;
                        
                        Thread.Sleep(heartbeatInterval);
                    }
                    
                    Console.WriteLine("Heartbeat stopped.");
                });
                
                ws.OnMessage += (sender, e) =>
                {
                    var jo = JObject.Parse(e.Data);
                    var op = (int)jo["op"];

                    if (op == 10)
                    {
                        Console.WriteLine("Server: " + op + " Hello");
                        heartbeatInterval = (int)jo["d"]["heartbeat_interval"];
                        Console.WriteLine("Heartbeat interval: " + heartbeatInterval);

                        heart.Start();

                        // authenticate
                        {
                            var s = @"{
    ""op"": 2,
    ""d"": {
        ""token"": ""MjQzMDg0Nzc2NzkyMTI5NTM2.DC_KCA.IfP0ZhwJjjHOp_80_VbQQZCLCTw"",
        ""properties"": {
            ""$os"": ""linux"",
            ""$browser"": ""zurvanbot"",
            ""$device"": ""zurvanbot"",
            ""$referrer"": """",
            ""$referring_domain"": """"
        },
        ""compress"": false
    }
}";
                            
                            ws.Send(s);
                        }
                    }
                    else if (op == 0)
                    {
                        var t = (string) jo["t"];
                        if (t.Equals("READY"))
                        {
                            Console.WriteLine("Ready.");
                        }
                        else
                        {
                            Console.WriteLine("Dispatch: " + t);
                            Console.WriteLine(e.Data);
                        }
                    }
                    else
                    {
                        Console.WriteLine("OP: " + op);
                    }
                };

                ws.OnOpen += (sender, eventArgs) =>
                {
                    
                };

                ws.Connect ();
                Console.ReadKey (true); */
                
                // Log.Instance().LogLevel = Log.Elevation.Warning;
                
                var gateway = new GatewayListener("wss://gateway.discord.gg/", "MjQzMDg0Nzc2NzkyMTI5NTM2.DDA97w.PH7KBiIIygTw3UXhU6F4Fg3r7vQ");
                
                gateway.OnReadyEvent += eventArgs =>
                {
                    Console.WriteLine(eventArgs.Guilds.Length);
                };
                
                gateway.StartListeningAsync().Wait();


                Log.Info("Closed.");
            }
        }
    }
}
