namespace ZurvanBot.Discord.Resources.Objects
{
    public class InviteObject: ResObject
    {
        public string code;
        public InviteGuildObject guild;
        public InviteChannelObject channel;
    }
}