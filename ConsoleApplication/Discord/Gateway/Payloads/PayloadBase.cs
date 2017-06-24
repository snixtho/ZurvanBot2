using Newtonsoft.Json;

namespace ZurvanBot.Discord.Gateway.Payloads
{
    public class PayloadBase
    {
        public int op;
        public object d;
        public int s;
        public string t;

        public string Serialized()
        {
            var serialized = JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            return serialized;
        }
    }
}