using ZurvanBot.Discord.Voice.Payloads.Data;

namespace ZurvanBot.Discord.Voice.Payloads {
    public class SpeakingPayload : PayloadBase {
        public new int op = 5;
        public new VoiceSpeakingData d;
    }
}