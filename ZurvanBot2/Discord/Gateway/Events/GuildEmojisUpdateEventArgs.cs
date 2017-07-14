using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Gateway.Events {
    public class GuildEmojisUpdateEventArgs : GatewayEventArgs {
        public ulong? GuildId;
        public EmojiObject[] Emojis;
    }
}