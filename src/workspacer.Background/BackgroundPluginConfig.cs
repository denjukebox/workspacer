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
        public bool RandomOrder { get; set; }

        private int _currentIndex = 0;
        private Random _random = new Random();

        public MultiBackgroundPluginConfig(IEnumerable<BackgroundItem> backgrounds, IMonitor monitor, TimeSpan interval, bool randomOrder) : base(monitor)
        {
            Interval = interval;
            Backgrounds = backgrounds.ToList();
            RandomOrder = randomOrder;
        }

        public BackgroundItem GetNext()
        {
            if (RandomOrder)
            {
                return Backgrounds.ElementAt(_random.Next(0, Backgrounds.Count() - 1));
            }
            else
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
}
