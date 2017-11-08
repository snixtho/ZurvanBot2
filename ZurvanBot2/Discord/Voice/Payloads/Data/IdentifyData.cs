using ZurvanBot.Discord.Gateway.Payloads;

namespace ZurvanBot.Discord.Voice.Payloads.Data {
    public class IdentifyData {
        public ulong server_id;
        public ulong user_id;
        public string session_id;
        public string token;
    }
}