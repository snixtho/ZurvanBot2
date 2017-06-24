﻿using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using ZurvanBot.Discord.Gateway.Payloads;

namespace ZurvanBot.Discord.Gateway
{
    public class GatewayListener
    {
        private Uri _gatewayAddress;
        private WebSocket _websocket;
        private bool isRunning = false;
        
        public int GatewayProtocol { get; private set; }
        
        public GatewayListener(string gatewayUrl)
        {
            GatewayProtocol = 5;
            _gatewayAddress = new Uri(gatewayUrl);
        }

        /// <summary>
        /// Gets the connection url.
        /// </summary>
        /// <returns>The connection url.</returns>
        private string buildConnectUrl()
        {
            return "wss://"
                   + _gatewayAddress.Host
                   + "/"
                   + "?v=" + GatewayProtocol
                   + "&encoding=json";
        }

        private void SetupNewSocket()
        {
            if (_websocket != null && _websocket.IsAlive)
                _websocket.Close();

            var connUrl = buildConnectUrl();
            _websocket = new WebSocket(connUrl);
            
            _websocket.OnOpen += WebsocketOnOnOpen;
            _websocket.OnMessage += WebsocketOnOnMessage;
            _websocket.OnError += WebsocketOnOnError;
            _websocket.OnClose += WebsocketOnOnClose;
        }

        private void SendAuthentication()
        {
            var identifyPayload = new IdentifyPayload();
            identifyPayload.compress = false;
            _websocket.Send(identifyPayload.Serialized());
        }

        private void WebsocketOnOnClose(object sender, CloseEventArgs closeEventArgs)
        {
            
        }

        private void WebsocketOnOnError(object sender, ErrorEventArgs errorEventArgs)
        {
            
        }

        private void WebsocketOnOnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            var jo = JObject.Parse(messageEventArgs.Data);
            var op = (OpCode) (int) jo["op"];

            switch (op)
            {
                case OpCode.Dispatch:
                    break;
                case OpCode.Heartbeat:
                    break;
                case OpCode.HeartbeatACK:
                    break;
                case OpCode.Hello:
                    break;
                case OpCode.Identify:
                    break;
                case OpCode.InvalidSession:
                    break;
                case OpCode.Reconnect:
                    break;
                case OpCode.RequestGuildMembers:
                    break;
                case OpCode.Resume:
                    break;
                case OpCode.StatusUpdate:
                    break;
                case OpCode.VoiceServeRPing:
                    break;
                case OpCode.VoiceStateUpdate:
                    break;
            }
        }

        private void WebsocketOnOnOpen(object sender, EventArgs eventArgs)
        {
            SendAuthentication();
        }

        /// <summary>
        /// Starts to listen to events to the gateway server.
        /// </summary>
        /// <returns>The main thread which can be joined.</returns>
        public Thread StarListening()
        {
            var t = new Thread(() =>
            {
                SetupNewSocket();
                
                // todo: implement proper wait
                while (isRunning) ;
            });

            return t;
        }
    }
}