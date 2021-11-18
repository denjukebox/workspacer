using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace workspacer.Bar
{
    public interface IBarWidgetPartDesign
    {
        public Color ForegroundColor { get; set; }
        public Color BackgroundColor { get; set; }
        public string FontName { get; set; }
        public int FontSize { get; set; }
    }

    public interface IBarWidgetPart
    {
        Control CreateControl();
        void UpdateControl(Control control);
    }

    public interface IBarWidgetPartClickAction
    {
        public Action PartClicked { get; set; }
    }
}
