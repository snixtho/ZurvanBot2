using System;
using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Resources.Params
{
    public class CreateMessageParams
    {
        public string content;
        public UInt64 ?nonce;
        public bool ?tts;
        public string file;
        public EmbedObject embed;
    }
}