﻿using System;
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
        private bool _dirty;
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
            _dirty = true;
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
            if (_dirty)
            {
                var widgets = _reverse ? _widgets.Reverse().ToArray() : _widgets;
                for (var wIndex = 0; wIndex < widgets.Length; wIndex++)
                {
                    var widgetPanel = (FlowLayoutPanel)_panel.Controls[wIndex];
                    var parts = widgets[wIndex].GetParts();

                    EqualizeControls(widgetPanel, parts);

                    for (var pIndex = 0; pIndex < parts.Length; pIndex++)
                    {
                        var part = parts[pIndex];
                        var control = widgetPanel.Controls[pIndex];
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

                    _dirty = false;
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
                    pannel.Controls.RemoveAt(0);
                }
            }
        }

        //private void SetAsControl(FlowLayoutPanel panel, IBarWidgetControlPart part)
        //{
        //    var control = part.GetControl();
        //    panel.Controls.Add(control);
        //}

        //private void SetAsLabel(FlowLayoutPanel panel, IBarWidgetLabelPart part)
        //{
        //    var label = new Label
        //    {
        //        Font = CreateFont(_fontName, _fontSize),
        //        Padding = new Padding(0),
        //        Margin = new Padding(0),
        //        AutoSize = true
        //    };

        //    label.Click += (s, e) =>
        //    {
        //        if (_clickedHandlers.ContainsKey(label))
        //        {
        //            _clickedHandlers[label]();
        //        }
        //    };

        //    label.Text = part.Text;

        //    var foregroundColor = ColorToColor(part.ForegroundColor ?? _defaultFore);
        //    if (label.ForeColor != foregroundColor)
        //    {
        //        label.ForeColor = foregroundColor;
        //    }

        //    var backgroundColor = ColorToColor(part.BackgroundColor ?? _defaultBack);
        //    if (label.BackColor != backgroundColor)
        //    {
        //        label.BackColor = backgroundColor;
        //    }

        //    label.Font = CreateFont(string.IsNullOrEmpty(part.FontName) ? _fontName : part.FontName, _fontSize);

        //    //if (part.PartClicked != null)
        //    //{
        //    //    _clickedHandlers[label] = part.PartClicked;
        //    //}
        //    //else
        //    //{
        //    //    _clickedHandlers.Remove(label);
        //    //}

        //    panel.Controls.Add(label);
        //}

        public void MarkDirty()
        {
            _dirty = true;
        }

        private System.Drawing.Color ColorToColor(Color color)
        {
            return System.Drawing.Color.FromArgb(color.R, color.G, color.B);
        }

        private Font CreateFont(string name, float size)
        {
            return new Font(name, size, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
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
