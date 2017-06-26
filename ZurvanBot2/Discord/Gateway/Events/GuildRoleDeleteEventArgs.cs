namespace ZurvanBot.Discord.Gateway.Events
{
    public class GuildRoleDeleteEventArgs: GatewayEventArgs
    {
        public ulong? GuildId;
        public ulong? RoleId;
    }
}