using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Gateway.Events {
    public class GuildRoleCreateEventArgs : GatewayEventArgs {
        public ulong? GuildId;
        public RoleObject Role;
    }
}