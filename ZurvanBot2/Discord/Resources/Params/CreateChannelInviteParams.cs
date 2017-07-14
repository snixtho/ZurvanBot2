using System.ComponentModel;
using Newtonsoft.Json;

namespace ZurvanBot.Discord.Resources.Params {
    public class CreateChannelInviteParams {
        [DefaultValue(86400)] [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)] public int max_age;
        [DefaultValue(0)] [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)] public int max_uses;
        public bool? temporary;
        public bool? unique;
    }
}