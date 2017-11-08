using ZurvanBot.Discord.Voice.Payloads.Data;

namespace ZurvanBot.Discord.Voice.Payloads {
    public class IdentifyPayload : PayloadBase {
        public new IdentifyData d = new IdentifyData();
        public new int op = 0;
    }
}