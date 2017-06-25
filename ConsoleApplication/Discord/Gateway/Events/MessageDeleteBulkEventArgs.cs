namespace ZurvanBot.Discord.Gateway.Events
{
    public class MessageDeleteBulkEventArgs: GatewayEventArgs
    {
        public ulong?[] Ids;
        public ulong? ChannelId;
    }
}