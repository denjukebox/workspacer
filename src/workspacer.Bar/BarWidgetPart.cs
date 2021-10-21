using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace workspacer.Bar
{
    public class BarWidgetLabelPart : IBarWidgetPart, IBarWidgetPartClickAction
    {
        public string Text { get; set; }
        public string LeftPadding { get; set; }
        public string RightPadding { get; set; }
        public Color ForegroundColor { get; set; }
        public Color BackgroundColor { get; set; }
        public Action PartClicked { get; set; }
        public string FontName { get; set; }

        public Control CreateControl()
        {
            var label = new Label
            {
                Font = CreateFont(FontName, 15),
                Padding = new Padding(0),
                Margin = new Padding(0),
                AutoSize = true
            };

            return label;
        }

        private System.Drawing.Color ColorToColor(Color color)
        {
            return System.Drawing.Color.FromArgb(color.R, color.G, color.B);
        }

        private Font CreateFont(string name, float size)
        {
            return new Font(name, size, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
        }

        public void UpdateControl(Control control)
        {
            control.Text = Text;

            var foregroundColor = ColorToColor(ForegroundColor ?? Color.White);
            if (control.ForeColor != foregroundColor)
            {
                control.ForeColor = foregroundColor;
            }

            var backgroundColor = ColorToColor(BackgroundColor ?? Color.Black);
            if (control.BackColor != backgroundColor)
            {
                control.BackColor = backgroundColor;
            }

            control.Font = CreateFont(FontName, 15);
        }
    }
}
