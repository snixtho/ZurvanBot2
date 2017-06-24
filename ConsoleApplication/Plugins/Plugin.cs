namespace ZurvanBot.Plugins
{
    public abstract class Plugin
    {
        protected PluginSystem pluginSystem;
        
        protected Plugin(PluginSystem pluginSystem)
        {
            this.pluginSystem = pluginSystem;
        }

        public abstract string Name();
        public abstract string Author();
        public abstract string Version();
        public abstract string Website();
    }
}
