using System;

namespace ZurvanBot.Discord.Resources.Objects
{
    public class GuildChannelObject : ResObject
    {
        public ulong? guild_id;
        public string name;
        public string type;
        public int? position;
        public bool? is_private;
        public OverwriteObject[] permission_overwrites;
        public string topic;
        public int? bitrate;
        public int? user_limit;
    }
}