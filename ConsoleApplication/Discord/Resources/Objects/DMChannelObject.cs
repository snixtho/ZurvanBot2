using System;

namespace ZurvanBot.Discord.Resources.Objects
{
    public class DMChannelObject: ResObject
    {
        public bool? is_private;
        public UserObject recipient;
        public ulong? last_message_id;
    }
}