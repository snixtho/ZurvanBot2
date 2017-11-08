using System;
using System.Threading.Tasks;
using ZurvanBot.Discord.Gateway;
using ZurvanBot.Discord.Gateway.Events;
using ZurvanBot.Discord.Voice;
using ZurvanBot.Util;

namespace ZurvanBot.Discord {
    public sealed class DiscordClient {
        private GatewayListener _gateway;
        private VoiceClient _voiceClient;
        private object _vcsw = new object();
        private VoiceConfig _vconf;
        private int _vconfStage = 0;

        /// <summary>
        /// Get the voice client.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the voice client is not connected.</exception>
        public VoiceClient Voice {
            get {
                if (_voiceClient == null)
                    throw new InvalidOperationException("The voice client is not connected.");
                return _voiceClient;
            }
        }
        /// <summary>
        /// Get the REST API client of the current discord client.
        /// </summary>
        public API Api { get; }
        /// <summary>
        /// Get the gateway client which the discord client uses.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the gateway is not connected.</exception>
        public GatewayListener Gateway {
            get {
                if (_gateway == null)
                    throw new InvalidOperationException("Gateway is not connected.");
                return _gateway;
            }
        }

        public DiscordClient(Authentication auth) {
            _gateway = new GatewayListener("wss://gateway.discord.gg/", auth);
            _gateway.OnVoiceServerUpdate += GatewayOnOnVoiceServerUpdate;
            _gateway.OnVoiceStateUpdate += GatewayOnOnVoiceStateUpdate;
        }

        private void NextVoiceConnectionStage() {
            _vconfStage++;

            if (_vconfStage != 2) return;
            
            _voiceClient = new VoiceClient(_vconf);
            _voiceClient.StartListeningAsync();
        }

        private void GatewayOnOnVoiceStateUpdate(VoiceStateUpdateEventArgs args) {
            lock (_vcsw) {
                if (_vconf == null)
                    _vconf = new VoiceConfig();

                _vconf.SessionId = args.VoiceState.session_id;
                _vconf.UserId = args.VoiceState.user_id ?? 0;
                
                NextVoiceConnectionStage();
            }
        }

        private void GatewayOnOnVoiceServerUpdate(VoiceServerUpdateEventArgs args) {
            lock (_vcsw) {
                if (_vconf == null)
                    _vconf = new VoiceConfig();

                _vconf.ServerId = args.GuildId ?? 0;
                _vconf.Endpoint = args.Endpoint;
                _vconf.Auth = new Authentication(Authentication.AuthType.Bot, args.Token);
                
                NextVoiceConnectionStage();
            }
        }

        /// <summary>
        /// Connect to a voice channel.
        /// </summary>
        /// <param name="guildId">The id of the guild of where the voice channel is located in.</param>
        /// <param name="channelId">The id of the voice channel</param>
        /// <exception cref="InvalidOperationException">Thrown when the gateway is not connected.</exception>
        public void ConnectToVoice(ulong guildId, ulong channelId) {
            if (!_gateway.Connected)
                throw new InvalidOperationException("Gateway not connected.");
            _gateway.JoinVoiceChannel(guildId, channelId);
        }

        /// <summary>
        /// Disconnect to a voice channel.
        /// </summary>
        /// <param name="guildId">The id of the guild of where the voice channel is located in.</param>
        /// <exception cref="InvalidOperationException">Thrown when the gateway is not connected.</exception>
        public void DisconnectFromVoice(ulong guildId) {
            if (!_gateway.Connected)
                throw new InvalidOperationException("Gateway not connected.");
            _gateway.DisconnectFromVoice(guildId);
            _vconf = null;
            _vconfStage = 0;
        }

        /// <summary>
        /// Start the discord client.
        /// </summary>
        public void Connect() {
            _gateway.StartListeningAsync().Wait();

            if (_gateway.DisconnectCode == 1000) {
                Log.Info("Gateway disconnected successfully.");
                return; // disconnected without errors
            }
            
            Log.Error("Connection disconnected with error (" + _gateway.DisconnectCode + "): " + _gateway.DisconnectReason);
        }

        /// <summary>
        /// Disconnect the discord client.
        /// </summary>
        public void Disconnect() {
            _gateway.StopListening();
        }
    }
}
