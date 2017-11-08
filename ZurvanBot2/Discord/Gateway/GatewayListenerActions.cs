using System.Threading;
using ZurvanBot.Discord.Gateway.Payloads;

namespace ZurvanBot.Discord.Gateway {
    public partial class GatewayListener {
        /// <summary>
        /// Joins/Moves/Leaves a voice channel.
        /// </summary>
        /// <param name="guildId">The guild the voice channel exists in.</param>
        /// <param name="channelId">If this is null, the client will leave, otherwise attempt to join/move to a channel.</param>
        private void _joinVoiceChannel(ulong guildId, ulong? channelId = null) {
            if (!_isRunning || !_websocket.IsAlive) return;

            Monitor.Enter(_readyWait);
            try {
                while (!_isReady) Monitor.Wait(_readyWait);
            }
            finally {
                Monitor.Exit(_readyWait);
            }

            var payload = new VoiceStateUpdatePayload {
                d = {
                    guild_id = guildId,
                    channel_id = channelId
                }
            };

            var serialized = payload.Serialized(false);
            _websocket.Send(serialized);
        }

        /// <summary>
        /// Join a voice channel, or move to another.
        /// </summary>
        /// <param name="guildId">The guild the voice channel exists in.</param>
        /// <param name="channelId">The channel to move/join.</param>
        public void JoinVoiceChannel(ulong guildId, ulong channelId) {
            _joinVoiceChannel(guildId, channelId);
        }

        /// <summary>
        /// Disconnect from the current voice channel.
        /// </summary>
        /// <param name="guildId">The guild the voice channel exists in.</param>
        public void DisconnectFromVoice(ulong guildId) {
            _joinVoiceChannel(guildId);
        }
    }
}