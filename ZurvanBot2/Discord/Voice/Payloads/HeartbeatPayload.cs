namespace ZurvanBot.Discord.Voice.Payloads {
    public class HeartbeatPayload : Gateway.Payloads.PayloadBase {
        public HeartbeatPayload() {
            op = 3;
            d = 0;
        }

        public HeartbeatPayload(int resumeRef) {
            op = 3;
            d = resumeRef;
        }
    }
}