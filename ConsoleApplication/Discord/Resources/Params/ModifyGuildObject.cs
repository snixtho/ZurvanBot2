namespace ZurvanBot.Discord.Resources.Params
{
    public class ModifyGuildObject
    {
        public string name;
        public string region;
        public int? verification_level;
        public int? default_message_notifications;
        public ulong? afk_channel_id;
        public int? afk_timeout;
        public string icon;
        public ulong? owner_id;
        public string splash;
    }
}