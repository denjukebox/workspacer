using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace workspacer.Bar
{
    public class BarSection
    {
        private FlowLayoutPanel _panel;
        private IBarWidget[] _widgets;
        private IMonitor _monitor;
        private IConfigContext _configContext;
        private string _fontName;
        private int _fontSize;

        private Color _defaultFore;
        private Color _defaultBack;

        private bool _reverse;
        private IBarWidgetContext _context;

        private IDictionary<Control, Action> _clickedHandlers;

        public BarSection(bool reverse, FlowLayoutPanel panel, IBarWidget[] widgets, IMonitor monitor, IConfigContext context,
            Color defaultFore, Color defaultBack, string fontName, int fontSize)
        {
            _panel = panel;
            _widgets = widgets;
            _monitor = monitor;
            _configContext = context;
            _fontName = fontName;
            _fontSize = fontSize;
            _reverse = reverse;
            _defaultFore = defaultFore;
            _defaultBack = defaultBack;

            _clickedHandlers = new Dictionary<Control, Action>();

            _context = new BarWidgetContext(this, _monitor, _configContext);
            while (_panel.Controls.Count != _widgets.Count())
            {
                _panel.Controls.Add(CreateWidgetPannel());
            }

            InitializeWidgets(widgets, _context);
        }

        private FlowLayoutPanel CreateWidgetPannel()
        {
            return new FlowLayoutPanel
            {
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom,
                BackColor = ColorToColor(_defaultBack),
                Location = new Point(0, 0),
                Margin = new Padding(0),
                Size = new Size(50, 50),
                WrapContents = false
            };
        }

        public void Draw()
        {
            if (!_widgets.Any(w => w.IsDirty()))
            {
                return;
            }

            var widgets = _reverse ? _widgets.Reverse().ToArray() : _widgets;
            for (int wIndex = 0; wIndex < widgets.Length; wIndex++)
            {
                if (!widgets[wIndex].IsDirty())
                {
                    continue;
                }

                var widgetPanel = _panel.Controls[wIndex];
                var parts = widgets[wIndex].GetParts();

                EqualizeControls((FlowLayoutPanel)widgetPanel, parts.Count());
                foreach (var part in parts)
                {
                    if (part is IBarWidgetPartWithDesign)
                    {
                        UpdatePart(part as IBarWidgetPartWithDesign);
                    }

                    part.UpdateControl(control);

                    if (part is IBarWidgetPartClickAction)
                    {
                        if (((IBarWidgetPartClickAction)part).PartClicked != null)
                        {
                            _clickedHandlers[control] = ((IBarWidgetPartClickAction)part).PartClicked;
                        }
                        else
                        {
                            _clickedHandlers.Remove(control);
                        }
                    }
                }

                widgets[wIndex].MarkClean();
            }
        }


            }
        }

        private void EqualizeControls(FlowLayoutPanel pannel, IBarWidgetPart[] parts)
        {
            var partCount = parts.Count();
            if (pannel.Controls.Count != partCount)
            {
                while (pannel.Controls.Count < partCount)
                {
                    UpdatePart(parts[0] as IBarWidgetPartWithDesign);
                    var control = parts[0].CreateControl();
                    pannel.Controls.Add(control);
                    if (parts[0] is IBarWidgetPartClickAction)
                    {
                        control.Click += (s, e) =>
                        {
                            if (_clickedHandlers.ContainsKey(control))
                            {
                                _clickedHandlers[control]();
                            }
                        };
                    }
                }

                while (pannel.Controls.Count > partCount)
                {
                    panel.Controls.RemoveAt(panel.Controls.Count - 1);
                }
            }
        }

        private System.Drawing.Color ColorToColor(Color color)
        {
            return System.Drawing.Color.FromArgb(color.R, color.G, color.B);
        }

        private void InitializeWidgets(IEnumerable<IBarWidget> widgets, IBarWidgetContext context)
        {
            foreach (var w in widgets)
            {
                w.Initialize(context);
            }
        }
    }
}
