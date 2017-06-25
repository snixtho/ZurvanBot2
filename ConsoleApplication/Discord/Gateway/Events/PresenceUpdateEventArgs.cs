using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Gateway.Events
{
    public class PresenceUpdateEventArgs: GatewayEventArgs
    {
        public UserObject User;
        public ulong[] Roles;
        public GameObject Game;
        public ulong? GuildId;
        public string Status;
    }
}