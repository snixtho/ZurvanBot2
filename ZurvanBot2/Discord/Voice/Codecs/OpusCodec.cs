using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace ZurvanBot.Discord.Voice.Codecs {
    // taken from: https://github.com/NaamloosDT/DSharpPlus
    public sealed class OpusCodec : IDisposable
    {
        [DllImport("libopus", CallingConvention = CallingConvention.Cdecl, EntryPoint = "opus_encoder_create")]
        private static extern IntPtr CreateEncoder(int samplerate, int channels, int application, out OpusError error);

        [DllImport("libopus", CallingConvention = CallingConvention.Cdecl, EntryPoint = "opus_encoder_destroy")]
        private static extern void DestroyEncoder(IntPtr encoder);

        [DllImport("libopus", CallingConvention = CallingConvention.Cdecl, EntryPoint = "opus_encode")]
        private static extern int Encode(IntPtr encoder, byte[] pcm, int frame_size, IntPtr data, int max_data_bytes);

#if !NETSTANDARD1_1
        [DllImport("libopus", CallingConvention = CallingConvention.Cdecl, EntryPoint = "opus_decoder_create")]
        private static extern IntPtr CreateDecoder(int samplerate, int channels, out OpusError error);

        [DllImport("libopus", CallingConvention = CallingConvention.Cdecl, EntryPoint = "opus_decoder_destroy")]
        private static extern void DestroyDecoder(IntPtr decoder);

        [DllImport("libopus", CallingConvention = CallingConvention.Cdecl, EntryPoint = "opus_decode")]
        private static extern int Decode(IntPtr decoder, byte[] opus, int frame_size, IntPtr data, int max_data_bytes, int decode_fec);

        
        public const int PCM_SAMPLE_SIZE = 3840;
#endif

        private IntPtr Encoder { get; set; }
#if !NETSTANDARD1_1
        private IntPtr Decoder { get; set; }
#endif
        private OpusError Errors { get; }
        private bool IsDisposed { get; set; }

        private int SampleRate { get; }
        private int Channels { get; }
        private VoiceApplication Application { get; }

        private static int[] AllowedSampleRates { get; }
        private static int[] AllowedChannelCounts { get; }

        public OpusCodec(int samplerate, int channels, VoiceApplication application)
        {
            if (!AllowedSampleRates.Contains(samplerate))
                throw new ArgumentOutOfRangeException(nameof(samplerate), string.Concat("Sample rate must be one of ", string.Join(", ", AllowedSampleRates)));

            if (!AllowedChannelCounts.Contains(channels))
                throw new ArgumentOutOfRangeException(nameof(channels), string.Concat("Channel count must be one of ", string.Join(", ", AllowedChannelCounts)));

            SampleRate = samplerate;
            Channels = channels;
            Application = application;

            var err = OpusError.OpusOk;
            Encoder = CreateEncoder(SampleRate, Channels, (int)Application, out err);
            Errors = err;
            if (Errors != OpusError.OpusOk)
                throw new Exception(Errors.ToString());

#if !NETSTANDARD1_1
            Decoder = CreateDecoder(SampleRate, Channels, out err);
            Errors = err;
            if (Errors != OpusError.OpusOk)
                throw new Exception(Errors.ToString());
#endif
        }

        ~OpusCodec()
        {
            Dispose();
        }

        static OpusCodec()
        {
            AllowedSampleRates = new[] { 8000, 12000, 16000, 24000, 48000 };
            AllowedChannelCounts = new[] { 1, 2 };
        }

        public unsafe byte[] Encode(byte[] pcm_input, int offset, int count, int bitrate = 16)
        {
            if (this.IsDisposed)
                throw new ObjectDisposedException(nameof(Encoder), "Encoder is disposed");

            var frame = new byte[count];
            Array.Copy(pcm_input, offset, frame, 0, frame.Length);

            var frame_size = FrameCount(frame.Length, bitrate);
            var encdata = IntPtr.Zero;
            var enc = new byte[frame.Length];
            int len = 0;

            fixed (byte* encptr = enc)
            {
                encdata = new IntPtr(encptr);
                len = Encode(Encoder, frame, frame_size, encdata, enc.Length);
            }

            if (len < 0)
                throw new Exception(string.Concat("OPUS encoding failed (", (OpusError)len, ")"));

            Array.Resize(ref enc, len);
            return enc;
        }

#if !NETSTANDARD1_1
        public unsafe byte[] Decode(byte[] opus_input, int offset, int count, int bitrate = 16)
        {
            if (this.IsDisposed)
                throw new ObjectDisposedException(nameof(Decoder), "Decoder is disposed");

            var frame = new byte[PCM_SAMPLE_SIZE];
            
            var frame_size = FrameCount(frame.Length, bitrate);
            var decdata = IntPtr.Zero;
            var len = 0;

            fixed (byte* decptr = frame)
            {
                decdata = new IntPtr(decptr);
                len = Decode(Decoder, opus_input, frame_size, decdata, frame.Length, 0);
            }

            if (len < 0)
                throw new Exception(string.Concat("OPUS decoding failed (", (OpusError)len, ")"));

            Array.Resize(ref frame, len * Channels * 2);
            return frame;
        }
#endif

        public void Dispose()
        {
            if (IsDisposed)
                return;
            
            if (Encoder != IntPtr.Zero)
                DestroyEncoder(Encoder);
            Encoder = IntPtr.Zero;

#if !NETSTANDARD1_1
            if (Decoder != IntPtr.Zero)
                DestroyDecoder(Decoder);
            Decoder = IntPtr.Zero;
#endif

            IsDisposed = true;
        }

        private int FrameCount(int length, int bitrate)
        {
            int bps = (bitrate >> 3) << 1; // (bitrate / 8) * 2;
            return length / bps;
        }
    }

    [Flags]
    internal enum OpusError
    {
        OpusOk = 0,
        OpusBadArg,
        OpusBufferToSmall,
        OpusInternalError,
        OpusInvalidPacket,
        OpusUnimplemented,
        OpusInvalidState,
        OpusAllocFail
    }

    public enum VoiceApplication : int
    {
        Voice = 2048,
        Music = 2049,
        LowLatency = 2051
    }
}