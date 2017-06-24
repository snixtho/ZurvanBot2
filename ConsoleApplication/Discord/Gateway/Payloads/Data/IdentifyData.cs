namespace ZurvanBot.Discord.Gateway.Payloads
{
    public class IdentifyData
    {
        public string token;
        public DeviceData properties = new DeviceData();
        public bool compress;
        public int? large_threshold;
        public int?[] shard;
    }
}