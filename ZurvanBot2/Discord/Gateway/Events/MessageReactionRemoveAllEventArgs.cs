namespace ZurvanBot.Discord.Gateway.Events {
    public class MessageReactionRemoveAllEventArgs : GatewayEventArgs {
        public ulong? ChannelId;
        public ulong? MessageId;
    }
}