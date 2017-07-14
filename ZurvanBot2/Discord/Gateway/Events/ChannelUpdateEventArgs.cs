using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Gateway.Events {
    public class ChannelUpdateEventArgs : GatewayEventArgs {
        public GuildChannelObject Guild;
    }
}