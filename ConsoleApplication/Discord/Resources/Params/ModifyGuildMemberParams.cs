namespace ZurvanBot.Discord.Resources.Params
{
    public class ModifyGuildMemberParams
    {
        public string nick;
        public ulong?[] roles;
        public bool? mute;
        public bool? deaf;
        public ulong? channel_id;
    }
}
