namespace ZurvanBot.Discord.Resources.Objects {
    public class InviteWithMetadataObject : InviteMetadataObject {
        public string code;
        public InviteGuildObject guild;
        public InviteChannelObject channel;
    }
}