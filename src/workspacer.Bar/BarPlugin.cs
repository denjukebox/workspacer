using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace workspacer.Bar
{
    public static class BarPluginResources
    {
        public static string BackgroundFillColor = "workspacer.Bar.BackgroundFillColor";
        internal static readonly Color BackgroundFillColorDefault = Color.Black;
    }

    public class BarPlugin : IPlugin
    {
        private BarPluginConfig _config;

        public BarPlugin()
        {
            _config = new BarPluginConfig();
        }

        public BarPlugin(BarPluginConfig config)
        {
            _config = config;
        }

        private class MyAppContext : ApplicationContext
        {
            public MyAppContext(BarPluginConfig config, IConfigContext context)
            {
                var bars = new List<BarForm>();

                foreach (var m in context.MonitorContainer.GetAllMonitors())
                {
                    var bar = new BarForm(m, config);

                    var left = config.LeftWidgets();
                    var right = config.RightWidgets();

                    PassConfig(left, config);
                    PassConfig(right, config);

                    bar.Initialize(left, right, context);

                    bar.Show();
                    bars.Add(bar);
                }
            }
        }

        private static void PassConfig(IEnumerable<IBarWidget> widgets, BarPluginConfig config)
        {
            //Color Config
            foreach (var widget in widgets)
            {
                config.Colors.MergeResourceDictionaries(widget.Colors);
            }
        }

        public void AfterConfig(IConfigContext context)
        {
            var thread = new Thread(() =>
            {
                Application.EnableVisualStyles();
                Application.Run(new MyAppContext(_config, context));
            });
            thread.Name = "BarPlugin";
            thread.Start();
        }
    }
}
