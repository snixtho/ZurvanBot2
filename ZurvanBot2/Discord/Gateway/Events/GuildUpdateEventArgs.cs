using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Gateway.Events {
    public class GuildUpdateEventArgs : GatewayEventArgs {
        public GuildObject Guild;
    }
}