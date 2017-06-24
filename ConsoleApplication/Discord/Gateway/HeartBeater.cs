using System.Threading;
using WebSocketSharp;
using ZurvanBot.Discord.Gateway.Payloads;

namespace ZurvanBot.Discord.Gateway
{
    public class HeartBeater
    {
        private int _interval;
        private WebSocket _ws;
        
        public HeartBeater(WebSocket ws, int interval)
        {
            _interval = interval;
            _ws = ws;
        }

        /// <summary>
        /// Send a heartbeat to the websocket server.
        /// </summary>
        /// <returns>True on success, false if the send failed.</returns>
        private bool SendHeartbeat()
        {
            if (!_ws.IsAlive) return false;
            
            var heartbeat = new HeartbeatPayload();
            _ws.Send(heartbeat.Serialized());
            return true;
        }

        /// <summary>
        /// Starts the heartbeater.
        /// </summary>
        public void Start()
        {
            try
            {
                while (_ws.IsAlive)
                {
                    if (!SendHeartbeat())
                        break;
                    Thread.Sleep(_interval);
                }
            }
            catch (ThreadInterruptedException e) {}
        }
    }
}