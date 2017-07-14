using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Resources.Params {
    public class CreateGuildChannelParams {
        public string name;
        public string type;
        public int? bitrate;
        public int? user_limit;
        public OverwriteObject[] permission_overwrites;
    }
}