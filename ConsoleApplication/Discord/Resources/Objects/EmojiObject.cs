namespace ZurvanBot.Discord.Resources.Objects
{
    public class EmojiObject: ResObject
    {
        public string name;
        public RoleObject[] roles;
        public bool? require_colons;
        public bool? managed;
    }
}