using System.Threading;
using ZurvanBot.Discord.Gateway.Payloads;

namespace ZurvanBot.Discord.Gateway
{
    public partial class GatewayListener
    {
        public void JoinVoiceServer(ulong guildId, ulong? channelId=null)
        {
            if (!_isRunning || !_websocket.IsAlive) return;
            
            Monitor.Enter(_readyWait);
            try { while (!_isReady) Monitor.Wait(_readyWait); }
            finally { Monitor.Exit(_readyWait); }

            var payload = new VoiceStateUpdatePayload
            {
                d =
                {
                    guild_id = guildId,
                    channel_id = channelId
                }
            };

            var serialized = payload.Serialized(false);
            _websocket.Send(serialized);
        }
    }
}