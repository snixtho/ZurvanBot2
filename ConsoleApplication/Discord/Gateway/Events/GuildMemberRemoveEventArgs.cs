using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Gateway.Events
{
    public class GuildMemberRemoveEventArgs: GatewayEventArgs
    {
        public ulong GuildId;
        public UserObject User;
    }
}