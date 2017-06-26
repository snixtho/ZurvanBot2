using System;

namespace ZurvanBot.Discord.Resources.Params
{
    public class GetChannelMessagesParams
    {
        public UInt64? around;
        public UInt64? before;
        public UInt64? after;
        public UInt64? limit;
    }
}