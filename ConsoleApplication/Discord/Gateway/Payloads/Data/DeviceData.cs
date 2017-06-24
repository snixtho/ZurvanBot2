using System.ComponentModel;
using Newtonsoft.Json;
using ZurvanBot.Util;

namespace ZurvanBot.Discord.Gateway.Payloads
{
    public class DeviceData
    {
        public DeviceData()
        {
            OS = PlatformDetect.GetBroadPlatformName();
            Browser = "ZurvanBot";
            Device = "ZurvanBot";
            Referer = "";
            ReferringDomain = "";
        }

        [JsonProperty(PropertyName = "$os")] 
        public string OS;

        [JsonProperty(PropertyName = "$browser")]
        public string Browser;

        [JsonProperty(PropertyName = "$device")]
        public string Device;
        
        [JsonProperty(PropertyName = "$referrer")]
        public string Referer;
        
        [JsonProperty(PropertyName = "$referring_domain")]
        public string ReferringDomain;
    }
}