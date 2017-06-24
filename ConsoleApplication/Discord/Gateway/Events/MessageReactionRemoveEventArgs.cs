using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Gateway.Events
{
    public class MessageReactionRemoveEventArgs: GatewayEventArgs
    {
        public ulong UserId;
        public ulong ChannelId;
        public ulong MessageId;
        public EmojiObject Emoji;
    }
}