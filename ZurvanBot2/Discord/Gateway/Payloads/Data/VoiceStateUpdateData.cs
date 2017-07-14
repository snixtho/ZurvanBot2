namespace ZurvanBot.Discord.Gateway.Payloads {
    public class VoiceStateUpdateData {
        public ulong guild_id;
        public ulong? channel_id;
        public bool self_mute = false;
        public bool self_deaf = false;
    }
}