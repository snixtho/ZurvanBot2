using System;
using System.Collections;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ZurvanBot.Util;

namespace ZurvanBot.Discord.Gateway {
    class GatewayEvent {
        private string _eventName;
        private JObject _eventData;
        private Hashtable _handlers = new Hashtable();
            
        public GatewayEvent(string eventName, JObject eventData) {
            _eventName = eventName;
            _eventData = eventData;
        }

        /// <summary>
        /// Create a new event handler for a given event identifier.
        /// </summary>
        /// <param name="eventName">The event identifier.</param>
        /// <param name="cb">Callback function for the event handler.</param>
        /// <returns>Current GatewayEvent object for "one-liner" coding.</returns>
        public GatewayEvent OnEvent(string eventName, Action<JToken> cb) {
            if (!_handlers.ContainsKey(eventName))
                _handlers.Add(eventName, cb);
            return this;
        }

        /// <summary>
        /// Handle the passed event.
        /// </summary>
        public Task Handle() {
            if (!_handlers.ContainsKey(_eventName)) {
                Log.Warning("No handler for event: " + _eventName, "GatewayEvent");
                return;
            }

            var func = (Action<JToken>)_handlers[_eventName];
            try {
                var et = new Task(() => func(_eventData["d"]));
                et.Start();
                return et;
            } catch (Exception e) {
                Log.Error(e.Message);
                throw;
            }
        }
    }
}