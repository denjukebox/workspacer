using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace workspacer.Bar
{
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
