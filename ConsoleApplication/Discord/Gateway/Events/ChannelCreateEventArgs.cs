using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Gateway.Events
{
    public class ChannelCreateEventArgs: GatewayEventArgs
    {
        public enum ChannelType {DM, Guild}
        
        public DMChannelObject DMChannel;
        public GuildChannelObject GuildChannel;
    }
}