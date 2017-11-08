using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Gateway.Events {
    public class VoiceStateUpdateEventArgs : GatewayEventArgs {
        public VoiceStateObject VoiceState;
    }
}