using System.Collections.Generic;

namespace ZurvanBot.Discord.Resources.Params
{
    public class CreateGroupDMParams
    {
        public string[] access_tokens;
        public Dictionary<ulong, string> nicks;
    }
}