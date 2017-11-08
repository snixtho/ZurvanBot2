using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Gateway.Events {
    public class ChannelCreateEventArgs : GatewayEventArgs {
        public DMChannelObject DMChannel;
        public GuildChannelObject GuildChannel;
        public ChannelType Type;
    }
}