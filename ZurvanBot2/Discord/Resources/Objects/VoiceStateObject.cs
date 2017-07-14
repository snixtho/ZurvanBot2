namespace ZurvanBot.Discord.Resources.Objects {
    public class VoiceStateObject : ResObject {
        public ulong? guild_id;
        public ulong? channel_id;
        public ulong? user_id;
        public string session_id;
        public bool? deaf;
        public bool? mute;
        public bool? self_deaf;
        public bool? self_mute;
        public bool? suppress;
    }
}