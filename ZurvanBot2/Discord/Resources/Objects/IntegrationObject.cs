namespace ZurvanBot.Discord.Resources.Objects {
    public class IntegrationObject : ResObject {
        public string name;
        public string type;
        public bool? enabled;
        public bool? syncing;
        public ulong role_id;
        public int? expire_behavior;
        public int? expire_grace_period;
        public UserObject user;
        public AccountObject account;
        public string synced_at;
    }
}