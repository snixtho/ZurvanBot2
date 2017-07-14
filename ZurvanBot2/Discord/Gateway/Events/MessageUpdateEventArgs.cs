using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Gateway.Events {
    public class MessageUpdateEventArgs : GatewayEventArgs {
        public MessageObject Message;
    }
}