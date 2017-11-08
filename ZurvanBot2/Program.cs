using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using ZurvanBot.Discord;
using ZurvanBot.Discord.Gateway;
using ZurvanBot.Discord.Resources.Objects;
using ZurvanBot.Discord.Resources.Params;
using ZurvanBot.Util;
using Sodium;

namespace ZurvanBot {
    internal class Program {
        
        public static void Main(string[] args) {
            Log.Instance().LogLevel = Log.Elevation.Verbose;
            var auth = new Authentication(Authentication.AuthType.Bot, "MjQzMDg0Nzc2NzkyMTI5NTM2.DFY3nw.xlUTNbws0KQDheiSwL1cIH61-XM");
            var discord = new DiscordClient(auth);

            discord.Gateway.OnMessageCreate += eventArgs => {
                if (eventArgs.Message.content.Equals("vj")) {
                    Log.Info("Joining voice channel ...");
                    discord.ConnectToVoice(95057537379872768, 95057540647235584);
                } else if (eventArgs.Message.content.Equals("vd")) {
                    Log.Info("Leaving voice channel ...");
                    // discord.Gateway.JoinVoiceChannel(95057537379872768, 95057540647235584);
                    discord.DisconnectFromVoice(95057537379872768);
                } else if (eventArgs.Message.content.Equals("vplay")) {
                    discord.Voice.SendIsSpeaking();

                    try {
                        var ffmpeg_inf = new ProcessStartInfo {
                            FileName = "ffmpeg",
                            Arguments = "-i \"song.mp3\" -ac 2 -f s16le -ar 48000 pipe:1",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        };
                        var ffmpeg = Process.Start(ffmpeg_inf);
                        var ffout = ffmpeg.StandardOutput.BaseStream;

                        using (var ms = new MemoryStream()) // if ffmpeg quits fast, that'll hold the data
                        {
                            ffout.CopyTo(ms);
                            ms.Position = 0;

                            var buff = new byte[3840]; // buffer to hold the PCM data
                            var br = 0;
                            while ((br = ms.Read(buff, 0, buff.Length)) > 0) {
                                Log.Info("Sending voice data ...");
                                if (br < buff.Length
                                ) // it's possible we got less than expected, let's null the remaining part of the buffer
                                    for (var i = br; i < buff.Length; i++)
                                        buff[i] = 0;

                                // await vnc.SendAsync(buff, 20); // we're sending 20ms of data
                                discord.Voice.VoiceStream.Send(buff, 20);
                            }
                        }
                    }
                    finally {
                        discord.Voice.SendIsSpeaking(false);
                    }
                    
                }
            };
            
            discord.Connect();
        }
    }
}