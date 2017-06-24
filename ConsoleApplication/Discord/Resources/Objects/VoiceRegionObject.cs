namespace ZurvanBot.Discord.Resources.Objects
{
    public class VoiceRegionObject: ResObject
    {
        public new string id;
        public string name;
        public string sample_hostname;
        public int sample_port;
        public bool vip;
        public bool optimal;
        public bool deprecated;
        public bool custom;
    }
}