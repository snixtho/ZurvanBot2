using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Gateway.Events {
    public class GuildCreateEventArgs : GatewayEventArgs {
        public GuildObject Guild;
    }
}