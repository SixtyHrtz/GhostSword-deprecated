using GhostSword.Interfaces;
using GhostSword.Types;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace GhostSwordOnline.Plugins
{
    public static class PluginManager
    {
        private static ContainerConfiguration configuration;

        public static void LoadPlugins()
        {
            var assemblyPath = Assembly.GetEntryAssembly().Location;
            var pluginsPath = Path.Combine(Path.GetDirectoryName(assemblyPath), "Plugins");
            Directory.CreateDirectory(pluginsPath);

            var assemblies = Directory
                .GetFiles(pluginsPath, "*.dll", SearchOption.AllDirectories)
                .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath)
                .ToList();

            configuration = new ContainerConfiguration().WithAssemblies(assemblies);
        }

        public static CompositionHost GetContainer() => configuration.CreateContainer();
    }

    public abstract class PluginManager<T>
    {
        [ImportMany]
        public List<T> Objects { get; private set; }

        public abstract string PluginType { get; }
        public string OfType { get { return $"{Resources.OfType} [{PluginType}]"; } }

        public Data<Message> LoadObjects(IServerCore serverCore)
        {
            try
            {
                List<T> objects;
                using (var container = PluginManager.GetContainer())
                    objects = container.GetExports<T>().ToList();

                if (objects.Count == 0)
                    return Data<Message>.CreateError($"{Resources.PluginsNotFound} {OfType}");

                var message = InitObjects(serverCore, objects);
                if (message != null)
                    if (!message.IsValid)
                        return message;

                Objects = objects;
                return Data<Message>.CreateValid(new Message($"{Resources.Plugin} {OfType} {Resources.SuccessfullyLoaded}"));
            }
            catch (Exception e)
            {
                return Data<Message>.CreateError($"{Resources.PluginLoadError} {OfType}\n{e.Message}");
            }
        }

        public virtual Data<Message> InitObjects(IServerCore serverCore, List<T> objects) => null;
    }
}