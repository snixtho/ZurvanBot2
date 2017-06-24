using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Gateway.Events
{
    public class GuildMemberUpdateEventArgs: GatewayEventArgs
    {
        public ulong GuildId;
        public RoleObject[] Roles;
        public UserObject User;
        public string Nick;
    }
}