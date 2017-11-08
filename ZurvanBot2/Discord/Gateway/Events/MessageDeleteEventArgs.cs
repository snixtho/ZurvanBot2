namespace ZurvanBot.Discord.Gateway.Events {
    public class MessageDeleteEventArgs : GatewayEventArgs {
        public ulong? Id;
        public ulong? ChannelId;
    }
}