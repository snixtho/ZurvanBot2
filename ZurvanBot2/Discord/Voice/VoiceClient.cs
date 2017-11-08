using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using ZurvanBot.Discord.Voice.Payloads;
using ZurvanBot.Discord.Voice.Payloads.Data;
using ZurvanBot.Util;

namespace ZurvanBot.Discord.Voice {
    public class VoiceClient {
        private DiscordClient _client;
        private VoiceConfig _config;
        private WebSocket _websocket;
        private Uri _gatewayAddress;
        private bool _isRunning = false;
        private bool _isReady = false;
        private HeartBeater _heart; // todo: make own heartbeater for the voice
        private object _mainWait = new object();
        private object _readyWait = new object();
        private bool _reconnect = true;
        private int _disconnectCode = -1;
        private string _disconnectReason = null;

        private string _vsAddress;
        private ushort _vsPort;
        
        
        public VoiceStream VoiceStream { get; private set; }
        
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
        
        public VoiceClient(VoiceConfig config) {
            // _client = client;
            _gatewayAddress = new Uri(config.Endpoint);
            _config = config;
            AsyncEvents = true;
        }
        
        /// <summary>
        /// Gets the connection url.
        /// </summary>
        /// <returns>The connection url.</returns>
        private string buildConnectUrl() {
            Log.Verbose("buildConnUrl: " + _gatewayAddress,"VoiceClient");
            return "ws://"
                   + _gatewayAddress
                   + "/"
                   + "";
        }
        
        private void SendAuthentication() {
            var identifyPayload = new IdentifyPayload();
            identifyPayload.d.server_id = _config.ServerId;
            identifyPayload.d.session_id = _config.SessionId;
            identifyPayload.d.token = _config.Auth.AuthString;
            identifyPayload.d.user_id = _config.UserId;
            Log.Debug("Attempt: Send authentication ...", "VoiceClient");
            _websocket.Send(identifyPayload.Serialized());
        }
        
        private void SetupNewSocket() {
            Log.Debug("Setting up websocket ...", "VoiceClient");
            if (_websocket != null && _websocket.IsAlive) {
                Log.Debug("_websocket != null && _websocket.IsAlive: True, closing websocket ...", "VoiceClient");
                _websocket.Close();
            }

            var connUrl = buildConnectUrl();
            Log.Verbose("Connecting ws to: " + connUrl, "VoiceClient");
            _websocket = new WebSocket(connUrl);

            _websocket.OnOpen += WebsocketOnOnOpen;
            _websocket.OnMessage += WebsocketOnOnMessage;
            _websocket.OnError += WebsocketOnOnError;
            _websocket.OnClose += WebsocketOnOnClose;
        }
        
        private void WebsocketOnOnClose(object sender, CloseEventArgs closeEventArgs) {
            Log.Info("Connection closed: (" + closeEventArgs.Code + ") " + closeEventArgs.Reason, "VoiceClient");

            _disconnectCode = closeEventArgs.WasClean ? 1000 : closeEventArgs.Code;
            _disconnectReason = closeEventArgs.Reason;

            try {
                Log.Debug("Attempt: set _isRunning to false.", "VoiceClient");
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
            Log.Debug("Message recieved from websocket.", "VoiceClient");
            Log.Verbose("Message: " + messageEventArgs.Data, "VoiceClient");
            var jo = JObject.Parse(messageEventArgs.Data);
            var op = (VoiceOpCode) (int) jo["op"];

            switch (op) {
                case VoiceOpCode.Identify:
                    Log.Debug("Event: Identify", "voiceclient.websocket.onmessage");
                    break;
                case VoiceOpCode.SelectProtocol:
                    Log.Debug("Event: SelectProtocol", "voiceclient.websocket.onmessage");
                    break;
                case VoiceOpCode.Ready://276
                    Log.Debug("Event: Ready", "voiceclient.websocket.onmessage");
                    _heart.StartAsync();

                    _vsAddress = (string)jo["d"]["ip"];
                    _vsPort = (ushort)jo["d"]["port"];
                    
                    VoiceStream = new VoiceStream(_vsAddress, _vsPort, this);
                    VoiceStream.Connect();
                    
                    Monitor.Enter(_readyWait);
                    try {
                        _isReady = true;
                        Monitor.PulseAll(_readyWait);
                    }
                    finally {
                        Monitor.Exit(_readyWait);
                    }
                    
                    break;
                case VoiceOpCode.Heartbeat:
                    Log.Debug("Event: Heartbeat", "voiceclient.websocket.onmessage");
                    _heart.EchoHeartbeat();
                    break;
                case VoiceOpCode.SessionDescription:
                    Log.Debug("Event: SessionDescription", "voiceclient.websocket.onmessage");
                    break;
                case VoiceOpCode.Speaking:
                    Log.Debug("Event: Speaking", "voiceclient.websocket.onmessage");
                    break;
                case VoiceOpCode.Hello:
                    Log.Debug("Event: Hello", "voiceclient.websocket.onmessage");
                    SendAuthentication();
                    _heart = new HeartBeater(_websocket, (int)jo["d"]["heartbeat_interval"]);
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
            Log.Debug("Attempt: start listning ...", "VoiceClient");
            if (_isRunning)
                throw new Exception("This gateway listener has already been started.");

            _disconnectCode = -1;
            _disconnectReason = null;
            
            var t = new Task(() => {
                while (_reconnect) {
                    _reconnect = false;
                    SetupNewSocket();
                    Log.Debug("Starting websocket ...", "VoiceClient");
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
            Log.Debug("Attempt: stop listening ...", "VoiceClient");

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
        
        /// <summary>
        /// Set the speaking status on discord.
        /// </summary>
        /// <param name="speaking">True to set as speaking, false for not speaking.</param>
        public void SendIsSpeaking(bool speaking = true) {
            var payload = new SpeakingPayload();
            payload.d = new VoiceSpeakingData(speaking);

            var serialized = payload.Serialized();
            Log.Info("Sending speaking: " + speaking, "VoiceClient");
            Log.Verbose("Speaking payload: " + serialized, "VoiceClient");
            _websocket.Send(serialized);
        }
    }
}