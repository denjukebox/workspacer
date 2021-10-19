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
        public Dictionary<string, Color> Colors { get; set; }

        public Func<IBarWidget[]> LeftWidgets { get; set; } = () => 
            new IBarWidget[] { new WorkspaceWidget(), new TextWidget(": "), new TitleWidget() };
        public Func<IBarWidget[]> RightWidgets { get; set; } = () => 
            new IBarWidget[] { new TimeWidget(), new ActiveLayoutWidget() };
    }
}
