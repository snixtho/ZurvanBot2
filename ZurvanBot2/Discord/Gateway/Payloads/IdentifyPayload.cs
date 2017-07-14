namespace ZurvanBot.Discord.Gateway.Payloads {
    public class IdentifyPayload : PayloadBase {
        public new IdentifyData d = new IdentifyData();
        public new int op = 2;
    }
}