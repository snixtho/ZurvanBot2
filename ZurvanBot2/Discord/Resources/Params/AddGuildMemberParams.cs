namespace ZurvanBot.Discord.Resources.Params
{
    public class AddGuildMemberParams
    {
        public string access_token;
        public string nick;
        public ulong?[] roles;
        public bool? mute;
        public bool? deaf;
    }
}