namespace ZurvanBot.Discord.Resources.Objects
{
    public class GuildMemberObject: ResObject
    {
        public UserObject user;
        public string nick;
        public string[] roles;
        public string joined_at;
        public bool? deaf;
        public bool? mute;
    }
}