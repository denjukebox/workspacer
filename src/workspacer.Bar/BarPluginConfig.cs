using System;
using System.Collections.Generic;
using workspacer.Bar.Widgets;

namespace workspacer.Bar
{
    public class BarPluginConfig
    {
        public string BarTitle { get; set; } = "workspacer.Bar";
        public int BarHeight { get; set; } = 30;
        public string FontName { get; set; } = "Consolas";
        public int FontSize { get; set; } = 16; 
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

        public Func<IBarWidget[]> LeftWidgets { get; set; } = () => 
            new IBarWidget[] { new WorkspaceWidget(), new TextWidget(": "), new TitleWidget() };
        public Func<IBarWidget[]> RightWidgets { get; set; } = () => 
            new IBarWidget[] { new TimeWidget(), new ActiveLayoutWidget() };
    }
}
