using System.Threading;
using WebSocketSharp;
using ZurvanBot.Discord.Gateway.Payloads;

namespace ZurvanBot.Discord.Gateway
{
    public class HeartBeater
    {
        private int _interval;
        private WebSocket _ws;
        private bool _gotEcho = true;
        
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
            if (!_gotEcho) {
                _ws.Close(CloseStatusCode.Normal);
                return false;
            }
            
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

        /// <summary>
        /// This has to be called in between the heartbeats, otherwise
        /// the heartbeater will automatically close the websocket connection.
        /// </summary>
        public void EchoHeartbeat()
        {
            _gotEcho = true;
        }
    }
}