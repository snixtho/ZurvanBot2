using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Gateway.Events
{
    public class GuildMemberAddEventArgs: GatewayEventArgs
    {
        public GuildMemberObject GuildMember;
        public ulong? GuildId;
    }
}