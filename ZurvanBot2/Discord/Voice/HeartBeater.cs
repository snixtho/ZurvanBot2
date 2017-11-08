using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using ZurvanBot.Discord.Voice.Payloads;
using ZurvanBot.Util;

namespace ZurvanBot.Discord.Voice {
    public class HeartBeater {
        private int _interval;
        private WebSocket _ws;
        private bool _gotEcho = true;

        public HeartBeater(WebSocket ws, int interval) {
            _interval = interval;
            _ws = ws;
        }

        /// <summary>
        /// Send a heartbeat to the websocket server.
        /// </summary>
        /// <returns>True on success, false if the send failed.</returns>
        private bool SendHeartbeat() {
            if (!_ws.IsAlive) {
                Log.Debug("Did not send heartbeat: websocket not alive.");
                return false;
            }
            if (!_gotEcho) {
                Log.Error("Heartbeater got no echo, closing connection.", "heartbeater");
                _ws.Close(CloseStatusCode.Normal);
                return false;
            }

            var heartbeat = new HeartbeatPayload();
            Log.Debug("Sending heartbeat ...");
            _ws.Send(heartbeat.Serialized());
            return true;
        }

        /// <summary>
        /// Starts the heartbeater.
        /// </summary>
        public void Start() {
            try {
                while (_ws.IsAlive) {
                    if (!SendHeartbeat())
                        break;
                    Thread.Sleep(_interval);
                }
            }
            catch (ThreadInterruptedException e) {
            }
        }

        /// <summary>
        /// Starts the heartbeater asynchronous.
        /// </summary>
        public Task StartAsync() {
            var task = new Task(() => {
                try {
                    while (_ws.IsAlive) {
                        if (!SendHeartbeat())
                            break;
                        Thread.Sleep(_interval);
                    }
                }
                catch (ThreadInterruptedException e) {
                }
            });

            task.Start();
            return task;
        }

        /// <summary>
        /// This has to be called in between the heartbeats, otherwise
        /// the heartbeater will automatically close the websocket connection.
        /// </summary>
        public void EchoHeartbeat() {
            Log.Debug("Got heartbeat echo.");
            _gotEcho = true;
        }
    }
}