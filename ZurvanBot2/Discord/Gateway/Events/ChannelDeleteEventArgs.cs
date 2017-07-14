using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Gateway.Events {
    public class ChannelDeleteEventArgs : GatewayEventArgs {
        public DMChannelObject DMChannel;
        public GuildChannelObject GuildChannel;
        public ChannelType Type;
    }
}