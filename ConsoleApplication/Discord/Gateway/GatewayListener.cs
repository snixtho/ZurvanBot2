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
    public class GatewayListener
    {
        public delegate void ReadyEvent(ReadyEventArgs e);
        public delegate void ChannelCreateEvent(ChannelCreateEventArgs e);
        public delegate void ChannelDeleteEvent(ChannelDeleteEventArgs e);
        public delegate void ChannelUpdateEvent(ChannelUpdateEventArgs e);
        public delegate void GuildBanAddEvent(GuildBanAddEventArgs e);
        public delegate void GuildBanRemoveEvent(GuildBanRemoveEventArgs e);
        public delegate void GuildCreateEvent(GuildCreateEventArgs e);
        public delegate void GuildDeleteEvent(GuildDeleteEventArgs e);
        public delegate void GuildEmojisUpdateEvent(GuildEmojisUpdateEventArgs e);
        public delegate void GuildIntegrationsUpdateEvent(GuildIntegrationsUpdateEventArgs e);
        public delegate void GuildMemberAddEvent(GuildMemberAddEventArgs e);
        public delegate void GuildMemberRemoveEvent(GuildMemberRemoveEventArgs e);
        public delegate void GuildMembersChunkEvent(GuildMembersChunkEventArgs e);
        public delegate void GuildMemberUpdateEvent(GuildMemberUpdateEventArgs e);
        public delegate void GuildRoleCreateEvent(GuildRoleCreateEventArgs e);
        public delegate void GuildRoleDeleteEvent(GuildRoleDeleteEventArgs e);
        public delegate void GuildRoleUpdateEvent(GuildRoleUpdateEventArgs e);
        public delegate void GuildUpdateEvent(GuildUpdateEventArgs e);
        public delegate void MessageCreateEvent(MessageCreateEventArgs e);
        public delegate void MessageDeleteBulkEvent(MessageDeleteBulkEventArgs e);
        public delegate void MessageDeleteEvent(MessageDeleteEventArgs e);
        public delegate void MessageReactionAddEvent(MessageReactionAddEventArgs e);
        public delegate void MessageReactionRemoveAllEvent(MessageReactionRemoveAllEventArgs e);
        public delegate void MessageReactionRemoveEvent(MessageReactionRemoveEventArgs e);
        public delegate void PresenceUpdateEvent(PresenceUpdateEventArgs e);
        public delegate void ResumedEvent(ResumedEventArgs e);
        public delegate void TypingStartEvent(TypingStartEventArgs e);
        public delegate void UserUpdateEvent(UserUpdateEventArgs e);
        public delegate void VoiceServerUpdateEvent(VoiceServerUpdateEventArgs e);
        public delegate void VoiceStateUpdateEvent(VoiceStateUpdateEventArgs e);
        
        public event ReadyEvent OnReadyEvent;
        public event ChannelCreateEvent OnChannelCreateEvent;
        public event ChannelDeleteEvent OnChannelDeleteEvent;
        public event ChannelUpdateEvent OnChannelUpdateEvent;
        public event GuildBanAddEvent OnGuildBanAddEvent;
        public event GuildBanRemoveEvent OnGuildBanRemoveEvent;
        public event GuildCreateEvent OnGuildCreateEvent;
        public event GuildDeleteEvent OnGuildDeleteEvent;
        public event GuildEmojisUpdateEvent OnGuildEmojisUpdateEvent;
        public event GuildIntegrationsUpdateEvent OnGuildIntegrationsUpdateEvent;
        public event GuildMemberAddEvent OnGuildMemberAddEvent;
        public event GuildMemberRemoveEvent OnGuildMemberRemoveEvent;
        public event GuildMembersChunkEvent OnGuildMembersChunkEvent;
        public event GuildMemberUpdateEvent OnGuildMemberUpdateEvent;
        public event GuildRoleCreateEvent OnGuildRoleCreateEvent;
        public event GuildRoleDeleteEvent OnGuildRoleDeleteEvent;
        public event GuildRoleUpdateEvent OnGuildRoleUpdateEvent;
        public event GuildUpdateEvent OnGuildUpdateEvent;
        public event MessageCreateEvent OnMessageCreateEvent;
        public event MessageDeleteBulkEvent OnMessageDeleteBulkEvent;
        public event MessageDeleteEvent OnMessageDeleteEvent;
        public event MessageReactionAddEvent OnMessageReactionAddEvent;
        public event MessageReactionRemoveAllEvent OnMessageReactionRemoveAllEvent;
        public event MessageReactionRemoveEvent OnMessageReactionRemoveEvent;
        public event PresenceUpdateEvent OnPresenceUpdateEvent;
        public event ResumedEvent OnResumedEvent;
        public event TypingStartEvent OnTypingStartEvent;
        public event UserUpdateEvent OnUserUpdateEvent;
        public event VoiceServerUpdateEvent OnVoiceServerUpdateEvent;
        public event VoiceStateUpdateEvent OnVoiceStateUpdateEvent;
        
        private readonly Uri _gatewayAddress;
        private WebSocket _websocket;
        private bool _isRunning = false;
        private readonly string _authToken;
        private HeartBeater _heart;
        private object _mainWait = new object();
        
        public int ProtocolVersion { get; private set; }
        
        public GatewayListener(string gatewayUrl, string authToken)
        {
            ProtocolVersion = 5;
            _gatewayAddress = new Uri(gatewayUrl);
            _authToken = authToken;
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
            identifyPayload.d.compress = false;
            identifyPayload.d.token = _authToken;
            _websocket.Send(identifyPayload.Serialized());
        }

        private void WebsocketOnOnClose(object sender, CloseEventArgs closeEventArgs)
        {
            Log.Info("Connection closed: (" + closeEventArgs.Code + ") " + closeEventArgs.Reason);
            
            if ((ErrorCode)closeEventArgs.Code == ErrorCode.NotAuthenticated)
                Log.Error("Authentication failed for gateway.");
            
            try
            {
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
            Log.Info("Message recieved from websocket.");
            Log.Debug("Message: " + messageEventArgs.Data);
            var jo = JObject.Parse(messageEventArgs.Data);
            var op = (OpCode) (int) jo["op"];

            switch (op)
            {
                case OpCode.Dispatch:
                    Log.Debug("Event: Dispatch", "websocket.onmessage");
                    var t = (string)jo["t"];
                    t = t.ToUpper();
                    Log.Info("Event: " + t);
                    
                    if (t.Equals("READY"))
                    {
                        if (t.ToUpper().Equals("READY")) {
                            _heart.StartAsync();
                        }
                        
                        var args = new ReadyEventArgs();
                        args.V = (int) jo["d"]["v"];
                        args.User = jo["d"]["user"].ToObject<UserObject>();
                        args.PrivateChannels = jo["d"]["private_channels"].ToObject<DMChannelObject[]>();
                        args.Guilds = jo["d"]["guilds"].ToObject<UnavailableGuildObject[]>();
                        args.SessionID = (string) jo["d"]["session_id"];
                        args._trace = jo["d"]["_trace"].ToObject<string[]>();
                        
                        OnReadyEvent?.Invoke(args);
                    }
                    else if (t.Equals("RESUMED")) {}
                    else if (t.Equals("CHANNEL_CREATE")) {}
                    else if (t.Equals("CHANNEL_UPDATE")) {}
                    else if (t.Equals("CHANNEL_DELETE")) {}
                    else if (t.Equals("GUILD_CREATE")) {}
                    else if (t.Equals("GUILD_UPDATE")) {}
                    else if (t.Equals("GUILD_DELETE")) {}
                    else if (t.Equals("GUILD_BAN_ADD")) {}
                    else if (t.Equals("GUILD_BAN_REMOVE")) {}
                    else if (t.Equals("GUILD_EMOJIS_UPDATE")) {}
                    else if (t.Equals("GUILD_INTEGRATIONS_UPDATE")) {}
                    else if (t.Equals("GUILD_MEMBER_ADD")) {}
                    else if (t.Equals("GUILD_MEMBER_REMOVE")) {}
                    else if (t.Equals("GUILD_MEMBER_UPDATE")) {}
                    else if (t.Equals("GUILD_MEMBERS_CHUNK")) {}
                    else if (t.Equals("GUILD_ROLE_CREATE")) {}
                    else if (t.Equals("GUILD_ROLE_UPDATE")) {}
                    else if (t.Equals("GUILD_ROLE_DELETE")) {}
                    else if (t.Equals("MESSAGE_CREATE")) {}
                    else if (t.Equals("MESSAGE_UPDATE")) {}
                    else if (t.Equals("MESSAGE_DELETE")) {}
                    else if (t.Equals("MESSAGE_DELETE_BULK")) {}
                    else if (t.Equals("MESSAGE_REACTION_ADD")) {}
                    else if (t.Equals("MESSAGE_REACTION_REMOVE")) {}
                    else if (t.Equals("MESSAGE_REACTION_REMOVE_ALL")) {}
                    else if (t.Equals("PRESENCE_UPDATE")) {}
                    else if (t.Equals("TYPING_START")) {}
                    else if (t.Equals("USER_UPDATE")) {}
                    else if (t.Equals("VOICE_STATE_UPDATE")) {}
                    else if (t.Equals("VOICE_SERVER_UPDATE")) {}
                    
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
            var t = new Task(() =>
            {
                SetupNewSocket();
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
    }
}
