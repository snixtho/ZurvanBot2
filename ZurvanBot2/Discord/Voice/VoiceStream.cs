using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ZurvanBot.Discord.Voice.Codecs;
using ZurvanBot.Util;
using ZurvanBot.Util.Net;

namespace ZurvanBot.Discord.Voice {
    public class VoiceStream {
        public delegate void OnDataReceievedEvent(byte[] data);
        public event OnDataReceievedEvent OnDataReceived;
        
        private UdpClientEx _socket;
        private VoiceClient _voiceClient;
        private string _host;
        private int _port;

        private OpusCodec _opusCodec;
        private RtpCodec _rtpCodec;
        private SodiumCodec _sodiumCodec;

        private ushort _seq;
        private uint _timestamp;
        private uint _ssrc;
        private byte[] _key;

        private Stopwatch _syncer;

        private bool _speaking;

        private TaskCompletionSource<bool> _playingWait;
        private SemaphoreSlim _playbackSemaphore;
        
        public VoiceStream(string host, int port, VoiceClient voiceClient) {
            _host = host;
            _port = port;
            _voiceClient = voiceClient;
            _syncer = new Stopwatch();
            _speaking = false;
            _playingWait = null;
            _playbackSemaphore = new SemaphoreSlim(1, 1);
        }

        /// <summary>
        /// Connect the audiostream.
        /// </summary>
        /// <returns>Task of the audio stream.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the stream is already connected.</exception>
        public Task Connect() {
            if (_socket != null && _socket.Active)
                throw new InvalidOperationException("Stream already active.");

            _socket = new UdpClientEx(_port);
            
            var t = new Task(() => {
                _socket.Connect(_host, _port);
                var anyEndpoint = new IPEndPoint(IPAddress.Any, 0);

                if (!_socket.Active) {
                    Log.Error("Failed to connect to the voice stream server.", "VoiceStream");
                    return;
                }
                
                Log.Info("VoiceStream connected.", "VoiceStream");

                try {
                    while (_socket.Active) {
                        var buf = _socket.Receive(ref anyEndpoint);
                        Log.Info("Voice data recieved", "VoiceStream");
                        OnDataReceived?.Invoke(buf);
                    }
                }
                catch (Exception e) {
                    Log.Error("VoiceStream disconnected by error: " + e.Message, "VoiceStream");
                }
            });
            
            t.Start();
            return t;
        }

        /// <summary>
        /// Send some data to the audio stream.
        /// </summary>
        /// <param name="pcm"></param>
        /// <param name="blockSize"></param>
        /// <param name="bitrate"></param>
        public void Send(byte[] pcm, int blockSize, int bitrate = 16) {
            _playbackSemaphore.WaitAsync().Wait();
            
            var rtp = _rtpCodec.Encode(_seq, _timestamp, _ssrc);
            var data = _opusCodec.Encode(pcm, 0, pcm.Length, bitrate);
            data = _sodiumCodec.Encode(data, _rtpCodec.MakeNonce(rtp), _key);
            data = _rtpCodec.Encode(rtp, data);

            if (!_speaking)
            {
                _syncer.Reset();
                _playingWait?.SetResult(true);
            }
            else
            {
                if (_playingWait == null || _playingWait.Task.IsCompleted)
                    _playingWait = new TaskCompletionSource<bool>();
            }
            
            _voiceClient.SendIsSpeaking(true);
            
            Log.Info("Sending audio " + pcm.Length + " bytes", "VoiceStream");
            
            _socket.SendAsync(data, data.Length).Wait();
            _seq++;
            _timestamp += 48 * (uint)blockSize;
            
            _syncer.Stop();
            var ts = TimeSpan.FromMilliseconds(blockSize) - _syncer.Elapsed - TimeSpan.FromMilliseconds(0.1);
            if (ts.Ticks < 0)
                ts = TimeSpan.FromTicks(1);
            var dt = DateTime.Now;
            while (DateTime.Now - dt < ts) ; // todo: can make this better?
            _syncer.Restart();

            _playbackSemaphore.Release();
        }
    }
}
