using System.ComponentModel;
using Newtonsoft.Json;

namespace ZurvanBot.Discord.Gateway.Payloads
{
    public class DeviceInfo
    {
        [JsonProperty(PropertyName = "$os")]
        public string OS
        {
            get { return "linux"; }
        }

        [DefaultValue("ZurvanBot")]
        [JsonProperty(PropertyName = "$browser")]
        public string Browser;

        [DefaultValue("ZurvanBot")]
        [JsonProperty(PropertyName = "$device")]
        public string Device;
        
        [DefaultValue("")]
        [JsonProperty(PropertyName = "$referrer")]
        public string Referer;
        
        [DefaultValue("")]
        [JsonProperty(PropertyName = "$referring_domain")]
        public string ReferringDomain;
    }
}