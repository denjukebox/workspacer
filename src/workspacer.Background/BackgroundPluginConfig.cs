using System;
using System.Linq;
using System.Collections.Generic;

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
        public BackgroundItem Background { get; set; }

        public SingleBackgroundPluginConfig(BackgroundItem background, IMonitor monitor = null) : base(monitor)
        {
            Background = background;
        }
    }

    public class MultiBackgroundPluginConfig : BackgroundPluginConfig
    {
        public List<BackgroundItem> Backgrounds { get; set; }
        public TimeSpan Interval { get; set; }

        private int _currentIndex = 0;

        public MultiBackgroundPluginConfig(IEnumerable<BackgroundItem> backgrounds, TimeSpan interval, IMonitor monitor = null) : base(monitor)
        {
            Interval = interval;
            Backgrounds = backgrounds.ToList();
        }

        public BackgroundItem GetNext()
        {
            if (_currentIndex == Backgrounds.Count() - 1)
            {
                _currentIndex = 0;
            }
            else
            {
                _currentIndex++;
            }

            return Backgrounds.ElementAt(_currentIndex);
        }
    }
}
