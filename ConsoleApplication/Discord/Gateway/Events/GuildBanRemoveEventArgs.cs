using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Gateway.Events
{
    public class GuildBanRemoveEventArgs: GatewayEventArgs
    {
        public UserObject User;
        public ulong? GuildId;
    }
}