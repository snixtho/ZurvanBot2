using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Gateway.Events {
    public class ReadyEventArgs : GatewayEventArgs {
        public int? V;
        public UserObject User;
        public DMChannelObject[] PrivateChannels;
        public UnavailableGuildObject[] Guilds;
        public string SessionID;
        public string[] _trace;
    }
}