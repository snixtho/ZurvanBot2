using Newtonsoft.Json;

namespace ZurvanBot.Discord.Resources.Params
{
    public class CreateGuildBanParams
    {
        [JsonProperty(PropertyName = "delete-message-days")]
        public int delete_message_days;
    }
}