using System;
using System.IO;
using ZurvanBot.Plugins.Collections;
using ZurvanBot.Plugins.PluginInterfaces;

namespace ZurvanBot.Plugins {
    public class PluginSystem {
        public string PluginsDir { get; }
        public PluginsFileCollection PluginFiles { get; }
        public PluginsFileCollection ErroredPlugins { get; }
        public PluginCollection LoadedPlugins { get; }

        public PluginSystem(string pluginsDir) {
            if (!Directory.Exists(pluginsDir))
                throw new DirectoryNotFoundException("The plugin directory could not be found.");

            PluginsDir = pluginsDir;
            PluginFiles = new PluginsFileCollection();
            ErroredPlugins = new PluginsFileCollection();
            LoadedPlugins = new PluginCollection();
        }

        /// <summary>
        /// Call OnAllPluginsLoaded on all plugins that supports it
        /// </summary>
        private void CallEndInitialization() {
            foreach (var plugin in LoadedPlugins)
                if (plugin is IEventsInitialization)
                    ((IEventsInitialization) plugin).OnAllPluginsLoaded(this);
        }

        /// <summary>
        /// Load all plugins from the specified directory.
        /// This will basically take all files ending with .cs,
        /// compile them and try to add them as a plugin object
        /// if the plugin class exists.
        /// </summary>
        /// <returns>True if all plugins were loaded correctly, false if errors occured.</returns>
        public bool LoadPlugins() {
            var dirs = Directory.GetDirectories(PluginsDir);
            var errorsOccured = false;

            foreach (var fileName in dirs) {
                var dir = new DirectoryInfo(fileName);
                if (!File.Exists(dir.FullName + "/Plugin.cs")) continue;

                var pluginDir = new PluginDirectory(dir);
                PluginFiles.Add(pluginDir);

                if (!pluginDir.TryCompile()) {
                    errorsOccured = true;
                    ErroredPlugins.Add(pluginDir);
                    continue;
                }

                Plugin plugin = null;

                try {
                    plugin = pluginDir.CreateInstance(this);
                }
                catch (Exception e) {
                    //todo: log this error
                    Console.WriteLine("Failed to load the plugin '" + pluginDir + "': " + e.Message);
                }

                if (plugin == null) {
                    // if this occurs, something strange happened
                    //todo: log this weird error
                    Console.WriteLine("Failed to load the plugin '" + pluginDir + "'.");
                }

                LoadedPlugins.Add(plugin);

                // call the OnLoaded event if possible
                if (plugin is IEventsInitialization)
                    ((IEventsInitialization) plugin).OnLoaded(this);
            }

            CallEndInitialization();

            return !errorsOccured;
        }
    }
}