namespace ZurvanBot.Discord.Resources.Objects
{
    public class ConnectionObject: ResObject
    {
        public new string id;
        public string name;
        public string type;
        public bool revoked;
        public IntegrationObject[] integrations;
    }
}
