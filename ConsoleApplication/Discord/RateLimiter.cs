using System;
using System.Net;
using System.Threading;
using ZurvanBot.Util;

namespace ZurvanBot.Discord
{
    public class RateLimiter
    {
        private object _ctrlObj = new object();
        private int _limit;
        private int _remaining;
        private uint _reset;
        
        public RateLimiter(int messagesPerSecond)
        {
            _limit = messagesPerSecond;
            _reset = (uint)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
        }

        /// <summary>
        /// Waits one second if limit has been reached, if the limit has not 
        /// been reached, it just returns immediately.
        /// </summary>
        public void WaitOrContinue()
        {
            lock (_ctrlObj)
            {
                var currTimestamp = (uint)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
                if (currTimestamp >= _reset+5)
                {
                    _remaining = 0;
                    _reset = currTimestamp + 1;
                }
            
                _remaining++;
                Log.Verbose("Rate count: " + _remaining);
                if (_remaining < _limit) return;
                Log.Info("Rate limit triggered. Waiting " +(_reset+5 - currTimestamp) + "s ...", "resources.ratelimit");
                Thread.Sleep((int)(_reset+5 - currTimestamp)*1000);
            }
        }
    }
}