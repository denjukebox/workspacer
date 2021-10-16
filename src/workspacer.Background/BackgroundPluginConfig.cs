namespace workspacer.Background
{
    public interface IBackgroundPluginConfig
    {
        IMonitor AssignedMonitor { get; }
    }

    public class BackgroundPluginConfig : IBackgroundPluginConfig
    {
        private IMonitor _assignedMonitor;
        public IMonitor AssignedMonitor { get { return _assignedMonitor; } }

        public BackgroundPluginConfig(IMonitor monitor)
        {
            _assignedMonitor = monitor;
        }        
    }

    public class SingleBackgroundPluginConfig : BackgroundPluginConfig
    {
        private BackgroundItem _background;
        public BackgroundItem Background { get { return _background; } }

        public SingleBackgroundPluginConfig(BackgroundItem background, IMonitor monitor = null) : base(monitor)
        {
            _background = background;
        }
    }

    public class MultiBackgroundPluginConfig : BackgroundPluginConfig
    {
        private BackgroundItem[] _backgrounds;
        private int _interval = 0;
        public int Interval { get { return _interval; } }

        private int _currentIndex = 0;

        public MultiBackgroundPluginConfig(BackgroundItem[] backgrounds, int interval, IMonitor monitor = null) : base(monitor)
        {
            _interval = interval;
            _backgrounds = backgrounds;
        }

        public BackgroundItem GetNext()
        {
            if (_currentIndex == _backgrounds.Length - 1)
            {
                _currentIndex = 0;
            }
            else
            {
                _currentIndex++;
            }

            return _backgrounds[_currentIndex];
        }
    }
}
