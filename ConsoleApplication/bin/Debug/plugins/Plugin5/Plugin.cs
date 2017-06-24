namespace ZurvanBot.Plugins
{
    public class CustomPlugin : Plugin
    {
        public CustomPlugin(PluginSystem pluginSystem) : base(pluginSystem)
        {
            
        }

        public override string Name()
        {
            StuffWriter stuffwriter = new StuffWriter();
            stuffwriter.WriteStuff();
            
            return "Plugin5";
        }
        
        public override string Author() {
            return "snixtho";
        }
        
        public override string Version() {
            return "1.0.0";
        }
        
        public override string Website() {
            return "https://snixtho.zurvan-labs.net";
        }
    }
} 
