using Newtonsoft.Json;

namespace ZurvanBot.Discord.Voice.Payloads {
    public class PayloadBase {
        public int op;
        public object d;
        public int s;
        public string t;

        public string Serialized(bool ignoreNull = true) {
            var serialized = JsonConvert.SerializeObject(this, new JsonSerializerSettings {
                NullValueHandling = ignoreNull ? NullValueHandling.Ignore : NullValueHandling.Include
            });
            return serialized;
        }
    }
}