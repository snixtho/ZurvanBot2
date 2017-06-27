namespace ZurvanBot.Discord.Gateway.Payloads
{
    public class HeartbeatPayload : PayloadBase
    {
        public HeartbeatPayload()
        {
            op = 1;
            d = 0;
        }

        public HeartbeatPayload(int resumeRef)
        {
            op = 1;
            d = resumeRef;
        }
    }
}