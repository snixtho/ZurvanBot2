namespace ZurvanBot.Discord.Resources.Objects
{
    public class PresenceObject: ResObject
    {
        public UserObject user;
        public ulong[] roles;
        public GameObject game;
        public ulong guild_id;
        public string status;
    }
}