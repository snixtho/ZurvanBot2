namespace ZurvanBot.Discord.Gateway.Events {
    public class GuildDeleteEventArgs : GatewayEventArgs {
        public ulong Id;
        public bool? Unavailable;
    }
}