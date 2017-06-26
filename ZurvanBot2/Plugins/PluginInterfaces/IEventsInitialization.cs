namespace ZurvanBot.Plugins.PluginInterfaces
{
    public interface IEventsInitialization
    {
        void OnLoaded(PluginSystem pluginSystem);
        void OnAllPluginsLoaded(PluginSystem pluginSystem);
    }
}
