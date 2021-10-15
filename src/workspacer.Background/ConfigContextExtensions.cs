using System.Linq;

namespace workspacer.Background
{
    public static class ConfigContextExtensions
    {
        public static void AddBackground(this IConfigContext context, IBackgroundPluginConfig config)
        {
            var baseMonitor = context.MonitorContainer.GetAllMonitors().FirstOrDefault();
            context.Plugins.RegisterPlugin(new BackgroundPlugin(config ?? new BackgroundPluginConfig(baseMonitor)));
        }
    }
}