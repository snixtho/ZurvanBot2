using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Gateway.Events {
    public class UserUpdateEventArgs : GatewayEventArgs {
        public UserObject User;
    }
}