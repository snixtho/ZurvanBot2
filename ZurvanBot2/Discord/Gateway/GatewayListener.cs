﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using ZurvanBot.Discord.Gateway.Events;
using ZurvanBot.Discord.Gateway.Payloads;
using ZurvanBot.Discord.Gateway.StateTracking;
using ZurvanBot.Discord.Resources.Objects;
using ZurvanBot.Util;
using WebSocket = WebSocketSharp.WebSocket;

namespace ZurvanBot.Discord.Gateway {
    public partial class GatewayListener {
        private readonly Uri _gatewayAddress;
        private WebSocket _websocket;
        private bool _isRunning = false;
        private bool _isReady = false;
        private readonly string _authToken;
        private HeartBeater _heart;
        private object _mainWait = new object();
        private object _readyWait = new object();
        private bool _reconnect = true;
        private int _disconnectCode = -1;
        private string _disconnectReason = null;

        /// <summary>
        /// True when the gateway is connected, false if not.
        /// </summary>
        public bool Connected => _isRunning;

        /// <summary>
        /// The gateway procotol version to use.
        /// </summary>
        public int ProtocolVersion { get; private set; }

        /// <summary>
        /// Set to true to call events asynchronously, false to wait for completion.
        /// </summary>
        public bool AsyncEvents { get; set; }

        /// <summary>
        /// Get the code returned when the gateway was disconnected.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when trying to retrive before disconnection.</exception>
        public int DisconnectCode {
            get {
                if (_disconnectCode < 0)
                    throw new InvalidOperationException("Gateway not disconnected. There is no disconnect code.");
                return _disconnectCode;
            }
        }
        
        /// <summary>
        /// The reason as a string for why the gateway disconnected.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when trying to retrive before disconnection.</exception>
        public string DisconnectReason {
            get {
                if (_disconnectReason == null)
                    throw new InvalidOperationException("Gateway not disconnected. There is no disconnect reason.");
                return _disconnectReason;
            }
        }

        public StateTracker State { get; set; }

        public GatewayListener(string gatewayUrl, Authentication auth) {
            ProtocolVersion = 5;
            _gatewayAddress = new Uri(gatewayUrl);
            _authToken = auth.AuthString;
            AsyncEvents = true;
            State = new StateTracker(this);
        }

        /// <summary>
        /// Gets the connection url.
        /// </summary>
        /// <returns>The connection url.</returns>
        private string buildConnectUrl() {
            return "wss://"
                   + _gatewayAddress.Host
                   + "/"
                   + "?v=" + ProtocolVersion
                   + "&encoding=json";
        }

        private void SetupNewSocket() {
            Log.Debug("Setting up websocket ...");
            if (_websocket != null && _websocket.IsAlive) {
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

        private void SendAuthentication() {
            var identifyPayload = new IdentifyPayload();
            identifyPayload.d.compress = false;
            identifyPayload.d.token = _authToken;
            Log.Debug("Attempt: Send authentication ...");
            _websocket.Send(identifyPayload.Serialized());
        }

        private void WebsocketOnOnClose(object sender, CloseEventArgs closeEventArgs) {
            Log.Info("Connection closed: (" + closeEventArgs.Code + ") " + closeEventArgs.Reason);

            _disconnectCode = closeEventArgs.WasClean ? 1000 : closeEventArgs.Code;
            _disconnectReason = closeEventArgs.Reason;

            try {
                Log.Debug("Attempt: set _isRunning to false.");
                Monitor.Enter(_mainWait);
                _isRunning = false;
                Monitor.PulseAll(_mainWait);
            }
            finally {
                Monitor.Exit(_mainWait);
            }
        }

        private void WebsocketOnOnError(object sender, ErrorEventArgs errorEventArgs) {
            Log.Error("Connection Errored: " + errorEventArgs.Message);
        }

        private void WebsocketOnOnMessage(object sender, MessageEventArgs messageEventArgs) {
            Log.Debug("Message recieved from websocket.");
            Log.Verbose("Message: " + messageEventArgs.Data);
            var jo = JObject.Parse(messageEventArgs.Data);
            var op = (OpCode) (int) jo["op"];

            switch (op) {
                case OpCode.Dispatch:
                    Log.Debug("Event: Dispatch", "websocket.onmessage");
                    var t = (string) jo["t"];
                    t = t.ToUpper();
                    Log.Debug("Event: " + t);
                    ProcessEvent(t, jo);

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
                    _reconnect = true;
                    _websocket.Close(CloseStatusCode.Normal);
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
                    Log.Debug("Event: VoiceStateUpdatePayload", "websocket.onmessage");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void WebsocketOnOnOpen(object sender, EventArgs eventArgs) {
            Log.Debug("Websocket open.", "websocket.onopen");
        }

        /// <summary>
        /// Starts to listen to events to the gateway server.
        /// </summary>
        /// <returns>The main thread which can be joined.</returns>
        public Task StartListeningAsync() {
            Log.Debug("Attempt: start listning ...");
            if (_isRunning)
                throw new Exception("This gateway listener has already been started.");

            _disconnectCode = -1;
            _disconnectReason = null;
            
            var t = new Task(() => {
                while (_reconnect) {
                    _reconnect = false;
                    SetupNewSocket();
                    Log.Debug("Starting websocket ...");
                    _websocket.Connect();

                    // wait until _isRunning is false, which is usally when the connection closes.
                    try {
                        Monitor.Enter(_mainWait);
                        while (_isRunning)
                            Monitor.Wait(_mainWait);
                    }
                    finally {
                        Monitor.Exit(_mainWait);
                    }
                }
            });

            t.Start();
            _isRunning = true;

            return t;
        }

        /// <summary>
        /// Stop listening to the gateway.
        /// </summary>
        public void StopListening() {
            Log.Debug("Attempt: stop listening ...");

            try {
                Monitor.Enter(_mainWait);
                _isRunning = false;
                Monitor.PulseAll(_mainWait);
                _websocket.Close(1000);
            }
            finally {
                Monitor.Exit(_mainWait);
            }
        }
    }
}