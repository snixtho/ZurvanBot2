using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using ZurvanBot.Discord.Gateway.Events;
using ZurvanBot.Discord.Gateway.Payloads;
using ZurvanBot.Discord.Resources.Objects;
using ZurvanBot.Util;

namespace ZurvanBot.Discord.Gateway
{
    public partial class GatewayListener
    {
        private readonly Uri _gatewayAddress;
        private WebSocket _websocket;
        private bool _isRunning = false;
        private readonly string _authToken;
        private HeartBeater _heart;
        private object _mainWait = new object();
        
        /// <summary>
        /// The gateway procotol version to use.
        /// </summary>
        public int ProtocolVersion { get; private set; }
        /// <summary>
        /// Set to true to call events asynchronously, false to wait for completion.
        /// </summary>
        public bool AsyncEvents { get; set; }
        
        public GatewayListener(string gatewayUrl, string authToken)
        {
            ProtocolVersion = 5;
            _gatewayAddress = new Uri(gatewayUrl);
            _authToken = authToken;
            AsyncEvents = true;
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
                   + "?v=" + ProtocolVersion
                   + "&encoding=json";
        }

        private void SetupNewSocket()
        {
            Log.Debug("Setting up websocket ...");
            if (_websocket != null && _websocket.IsAlive)
            {
                Log.Debug("_websocket != null && _websocket.IsAlive: True, closing websocket ...");
                _websocket.Close();
            }

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
            identifyPayload.d.compress = false;
            identifyPayload.d.token = _authToken;
            Log.Debug("Attempt: Send authentication ...");
            _websocket.Send(identifyPayload.Serialized());
        }

        private void WebsocketOnOnClose(object sender, CloseEventArgs closeEventArgs)
        {
            Log.Info("Connection closed: (" + closeEventArgs.Code + ") " + closeEventArgs.Reason);
            
            if ((ErrorCode)closeEventArgs.Code == ErrorCode.NotAuthenticated)
                Log.Error("Authentication failed for gateway.");
            
            try
            {
                Log.Debug("Attempt: set _isRunning to false.");
                Monitor.Enter(_mainWait);
                _isRunning = false;
                Monitor.PulseAll(_mainWait);
            }
            finally
            {
                Monitor.Exit(_mainWait);
            }
        }

        private void WebsocketOnOnError(object sender, ErrorEventArgs errorEventArgs)
        {
            Log.Error("Connection Errored: " + errorEventArgs.Message);
        }

        private void WebsocketOnOnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            Log.Debug("Message recieved from websocket.");
            Log.Verbose("Message: " + messageEventArgs.Data);
            var jo = JObject.Parse(messageEventArgs.Data);
            var op = (OpCode) (int) jo["op"];

            switch (op)
            {
                case OpCode.Dispatch:
                    Log.Debug("Event: Dispatch", "websocket.onmessage");
                    var t = (string)jo["t"];
                    t = t.ToUpper();
                    Log.Debug("Event: " + t);
                    DispatchEvent(t, jo);
                    
                    break;
                case OpCode.Heartbeat:
                    Log.Debug("Event: Heartbeat", "websocket.onmessage");
                    break;
                case OpCode.HeartbeatACK:
                    Log.Debug("Event: HeartbeatACK", "websocket.onmessage");
                    _heart.EchoHeartbeat();
                    break;
                case OpCode.Hello:
                    Log.Debug("Event: Hello", "websocket.onmessage");
                    SendAuthentication();
                    var heartInterval = (int) jo["d"]["heartbeat_interval"];
                    _heart = new HeartBeater(_websocket, heartInterval);
                    break;
                case OpCode.Identify:
                    Log.Debug("Event: Indentify", "websocket.onmessage");
                    break;
                case OpCode.InvalidSession:
                    Log.Debug("Event: InvalidSession", "websocket.onmessage");
                    break;
                case OpCode.Reconnect:
                    Log.Debug("Event: Reconnect", "websocket.onmessage");
                    break;
                case OpCode.RequestGuildMembers:
                    Log.Debug("Event: RequestGuildMembers", "websocket.onmessage");
                    break;
                case OpCode.Resume:
                    Log.Debug("Event: Resume", "websocket.onmessage");
                    break;
                case OpCode.StatusUpdate:
                    Log.Debug("Event: StatusUpdate", "websocket.onmessage");
                    break;
                case OpCode.VoiceServeRPing:
                    Log.Debug("Event: VoiceServerRPing", "websocket.onmessage");
                    break;
                case OpCode.VoiceStateUpdate:
                    Log.Debug("Event: VoiceStateUpdate", "websocket.onmessage");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void WebsocketOnOnOpen(object sender, EventArgs eventArgs)
        {
            Log.Debug("Websocket open.", "websocket.onopen");
        }

        /// <summary>
        /// Starts to listen to events to the gateway server.
        /// </summary>
        /// <returns>The main thread which can be joined.</returns>
        public Task StartListeningAsync()
        {
            Log.Debug("Attempt: start listning ...");
            if (_isRunning)
                throw new Exception("This gateway listener has already been started.");
            
            var t = new Task(() =>
            {
                SetupNewSocket();
                Log.Debug("Starting websocket ...");
                _websocket.Connect();
                
                // wait until _isRunning is false, which is usally when the connection closes.
                try
                {
                    Monitor.Enter(_mainWait);
                    while (_isRunning)
                        Monitor.Wait(_mainWait);
                }
                finally { Monitor.Exit(_mainWait); }
            });
            
            t.Start();
            _isRunning = true;

            return t;
        }

        /// <summary>
        /// Stop listening to the gateway.
        /// </summary>
        public void StopListening()
        {
            Log.Debug("Attempt: stop listening ...");
            
            try
            {
                Monitor.Enter(_mainWait);
                _isRunning = false;
                Monitor.PulseAll(_mainWait);
                _websocket.Close(1000);
            }
            finally
            {
                Monitor.Exit(_mainWait);
            }
        }
    }
}
