namespace ZurvanBot.Discord.Resources.Objects {
    public class InviteMetadataObject : ResObject {
        public UserObject user;
        public int? uses;
        public int? max_uses;
        public int? max_age;
        public bool? temporary;
        public string created_at;
        public bool? revoked;
    }
}