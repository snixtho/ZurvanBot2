using ZurvanBot.Discord.Gateway;

namespace ZurvanBot.Discord.Resources.Objects
{
    public class GameObject: ResObject
    {
        public string name;
        public GameTypes? type;
        public string url;
    }
}