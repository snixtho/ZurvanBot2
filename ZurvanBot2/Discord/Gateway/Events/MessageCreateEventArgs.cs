using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Gateway.Events {
    public class MessageCreateEventArgs : GatewayEventArgs {
        public MessageObject Message;
    }
}