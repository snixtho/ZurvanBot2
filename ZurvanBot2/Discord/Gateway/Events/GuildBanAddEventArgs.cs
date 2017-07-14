using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Gateway.Events {
    public class GuildBanAddEventArgs : GatewayEventArgs {
        public UserObject User;
        public ulong? GuildId;
    }
}