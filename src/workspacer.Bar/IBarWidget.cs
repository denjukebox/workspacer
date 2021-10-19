using System.Collections.Generic;

namespace workspacer.Bar
{
    public interface IBarWidget
    {
        void Initialize(IBarWidgetContext context);
        IBarWidgetPart[] GetParts();
        public IDictionary<string, Color> Colors { get; set; }
    }
}
