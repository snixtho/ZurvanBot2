using Newtonsoft.Json;
using ZurvanBot.Discord.Resources.Objects;

namespace ZurvanBot.Discord.Resources {
    public class Voice : Resource {
        public VoiceRegionObject[] ListVoiceRegions() {
            var re = _request.GetRequestAsync("/voice/regions").Result;
            if (re.Code != 200)
                return null; // handle these errors ?
            var r = JsonConvert.DeserializeObject<VoiceRegionObject[]>(re.Contents);
            return r;
        }

        public Voice(ResourceRequest request) : base(request) {
        }
    }
}