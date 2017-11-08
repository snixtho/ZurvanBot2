namespace ZurvanBot.Discord.Gateway.Payloads {
    public class VoiceStateUpdatePayload : PayloadBase {
        public new VoiceStateUpdateData d = new VoiceStateUpdateData();
        public new int op = 4;
    }
}