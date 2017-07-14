using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Resources.Params {
    public class CreateGuildParams {
        public string name;
        public string region;
        public string icon;
        public int verification_level;
        public int default_message_notifications;
        public RoleObject[] roles;
        public GuildChannelObject[] channels;
    }
}