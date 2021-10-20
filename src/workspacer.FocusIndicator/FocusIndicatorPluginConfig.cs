using System.Collections.Generic;

namespace workspacer.FocusIndicator
{
    public class FocusIndicatorPluginConfig
    {
        public int BorderSize = 10; 
        public int TimeToShow = 200;
        private Dictionary<string, Color> _colors;
        public Dictionary<string, Color> Colors
        {
            get
            {
                if (_colors == null)
                {
                    return WorkspacerResources.GlobalColors;
                }

                return _colors;
            }
            set
            {
                _colors = value;
                WorkspacerResources.GlobalColors.MergeResourceDictionaries(_colors);
            }
        }
    }
}
