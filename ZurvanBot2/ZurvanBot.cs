using ZurvanBot.Plugins;

namespace ZurvanBot {
    public sealed class ZurvanBot {
        public static string Version = "1.0.0";

        private PluginSystem _pluginsSystem = new PluginSystem("plugins");
        
        public ZurvanBot() {
            
        }
    }
}
