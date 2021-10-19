using System.Collections.Generic;

namespace workspacer.Bar
{
    public interface IBarWidget
    {
        void Initialize(IBarWidgetContext context);
        IBarWidgetPart[] GetParts();
        public Dictionary<string, Color> Colors { get; set; }
    }
}
