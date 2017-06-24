namespace ZurvanBot.Discord.Gateway.Events
{
    public class VoiceServerUpdateEventArgs: GatewayEventArgs
    {
        public string Token;
        public ulong GuildId;
        public string Endpoint;
    }
}