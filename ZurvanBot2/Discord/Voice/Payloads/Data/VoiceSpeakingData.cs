namespace ZurvanBot.Discord.Voice.Payloads.Data {
    public class VoiceSpeakingData {
        public bool speaking;
        public int delay;
        
        public VoiceSpeakingData(bool speaking, int delay=0) {
            this.speaking = speaking;
            this.delay = delay;
        }
    }
}