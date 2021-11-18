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
            for (var wIndex = 0; wIndex < widgets.Length; wIndex++)
            {
                if (!widgets[wIndex].IsDirty())
                {
                    continue;
                }

                var widgetPanel = (FlowLayoutPanel)_panel.Controls[wIndex];
                var parts = widgets[wIndex].GetParts();

                EqualizeControls(widgetPanel, parts);

                for (var pIndex = 0; pIndex < parts.Length; pIndex++)
                {
                    var part = parts[pIndex];
                    var control = widgetPanel.Controls[pIndex];
                    if (part is IBarWidgetPartDesign)
                    {
                        UpdatePart(part as IBarWidgetPartDesign);
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

        private void UpdatePart(IBarWidgetPartDesign part)
        {
            if (part.BackgroundColor == null)
            {
                part.BackgroundColor = _defaultBack;
            }

            if (part.ForegroundColor == null)
            {
                part.ForegroundColor = _defaultFore;
            }

            if (string.IsNullOrEmpty(part.FontName))
            {
                part.FontName = _fontName;
            }

            if (part.FontSize == 0)
            {
                part.FontSize = _fontSize;
            }
        }

        private void EqualizeControls(FlowLayoutPanel pannel, IBarWidgetPart[] parts)
        {
            var partCount = parts.Count();
            var rootPart = parts.FirstOrDefault();
            if (pannel.Controls.Count != partCount)
            {
                while (pannel.Controls.Count < partCount)
                {
                    var control = rootPart?.CreateControl();
                    pannel.Controls.Add(control);
                    if (rootPart is IBarWidgetPartClickAction && control != null)
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
                    pannel.Controls.RemoveAt(pannel.Controls.Count - 1);
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
