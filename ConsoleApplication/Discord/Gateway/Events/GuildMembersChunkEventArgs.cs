using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Gateway.Events
{
    public class GuildMembersChunkEventArgs: GatewayEventArgs
    {
        public ulong GuildId;
        public GuildMemberObject[] Members;
    }
}