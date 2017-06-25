namespace ZurvanBot.Discord.Gateway.Events
{
    public class TypingStartEventArgs: GatewayEventArgs
    {
        public ulong? ChannelId;
        public ulong? UserId;
        public int? Timestamp;
    }
}