using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Gateway.Events {
    public class GuildMemberUpdateEventArgs : GatewayEventArgs {
        public ulong? GuildId;
        public ulong[] Roles;
        public UserObject User;
        public string Nick;
    }
}