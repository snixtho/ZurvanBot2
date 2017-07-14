namespace ZurvanBot.Discord.Resources.Objects {
    public class GuildObject : ResObject {
        public string name;
        public string icon;
        public string splash;
        public ulong? owner_id;
        public string region;
        public ulong? afk_channel_id;
        public int? afk_timout;
        public bool? embed_enabled;
        public ulong? embed_channel_id;
        public int? verification_level;
        public int? default_message_notifications;
        public RoleObject[] roles;
        public EmojiObject[] emojis;
        public string[] features;
        public int? mfa_level;
        public string joined_at;
        public bool? large;
        public bool? unavailable;
        public int? member_count;
        public VoiceStateObject[] voice_states;
        public GuildMemberObject[] members;
        public GuildChannelObject[] channels;
        public PresenceObject[] presences;
    }
}