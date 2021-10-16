namespace workspacer.Background
{
    public class BackgroundPlugin : IPlugin
    {
        private IConfigContext _context;
        private IBackgroundPluginConfig _config;

        private BackgroundForm _form;

        public BackgroundPlugin() : this(new SingleBackgroundPluginConfig(new BackgroundItem(BackgroundContentType.Color, Color.Gray.ToString()))) { }

        public BackgroundPlugin(IBackgroundPluginConfig config)
        {
            _config = config;
            _form = new BackgroundForm(config);
        }

        public void AfterConfig(IConfigContext context)
        {
            _context = context;
            _form.ShowBackground();
        }
    }
}
