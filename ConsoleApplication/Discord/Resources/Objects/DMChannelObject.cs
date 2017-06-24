using System;

namespace ZurvanBot.Discord.Resources.Objects
{
    public class DMChannelObject: ResObject
    {
        public bool is_private;
        public UserObject recipient;
        public UInt64 last_message_id;
    }
}