namespace ZurvanBot.Discord.Gateway.Payloads
{
    public class IdentifyPayload: PayloadBase
    {
        public string token;
        public DeviceInfo properties;
        public bool compress;
        public int? large_threshold;
        public int?[] shard;
    }
}