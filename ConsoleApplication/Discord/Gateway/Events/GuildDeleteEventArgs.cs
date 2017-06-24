namespace ZurvanBot.Discord.Gateway.Events
{
    public class GuildDeleteEventArgs: GatewayEventArgs
    {
        public int Id;
        public bool Unavailable;
    }
}