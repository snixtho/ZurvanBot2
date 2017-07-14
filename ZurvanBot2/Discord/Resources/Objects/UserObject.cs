namespace ZurvanBot.Discord.Resources.Objects {
    public class UserObject : ResObject {
        public string username;
        public string discriminator;
        public string avatar;
        public bool? bot;
        public bool? mfa_enabled;
        public bool? verified;
        public string email;
    }
}