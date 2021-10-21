using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace workspacer.Bar.Widgets
{
    public class TitleWidgetTitlePart : IBarWidgetPart
    {
        public string Text { get; set; }
        public string ProcessPath { get; set; }
        public Control CreateControl()
        {
            var panel = new FlowLayoutPanel
            {
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom,
                Location = new Point(0, 0),
                Margin = new Padding(0),
                Size = new Size(50, 50),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };

            var icon = new PictureBox
            {
                Padding = new Padding(0),
                Margin = new Padding(0),
                AutoSize = true
            };

            panel.Controls.Add(icon);

            var label = new Label
            {
                Padding = new Padding(0),
                Margin = new Padding(0),
                AutoSize = true
            };

            panel.Controls.Add(label);

            return panel;
        }

        private System.Drawing.Color ColorToColor(Color color)
        {
            return System.Drawing.Color.FromArgb(color.R, color.G, color.B);
        }

        public void UpdateControl(Control control)
        {
            var panel = (FlowLayoutPanel)control;
            var label = panel.Controls.OfType<Label>().ElementAt(0);

            label.Text = Text;

            var foregroundColor = ColorToColor(Color.White);
            if (label.ForeColor != foregroundColor)
            {
                label.ForeColor = foregroundColor;
            }

            var backgroundColor = ColorToColor(Color.Black);
            if (label.BackColor != backgroundColor)
            {
                label.BackColor = backgroundColor;
            }

            var _pictureBox = panel.Controls.OfType<PictureBox>().ElementAt(0);
            if (!string.IsNullOrEmpty(ProcessPath))
            {
                var result = Icon.ExtractAssociatedIcon(ProcessPath);
                _pictureBox.Image = Bitmap.FromHicon(result.Handle);
            }
        }
    }

    public class TitleWidget : BarWidgetBase
    {
        public Color MonitorHasFocusColor { get; set; } = Color.Yellow;
        public bool IsShortTitle { get; set; } = false;
        public string NoWindowMessage { get; set; } = "No Windows";


        public override IBarWidgetPart[] GetParts()
        {
            var window = GetWindow();
            var isFocusedMonitor = Context.MonitorContainer.FocusedMonitor == Context.Monitor;
            var multipleMonitors = Context.MonitorContainer.NumMonitors > 1;
            var color = isFocusedMonitor && multipleMonitors ? MonitorHasFocusColor : null;
            if (window != null)
            {
                return Parts(Part(window));
                if (!IsShortTitle)
                {
                    return Parts(Part(window.Title, color, fontname: FontName));
                }
                else
                {
                    var shortTitle = GetShortTitle(window.Title);
                    return Parts(Part(shortTitle, color, fontname: FontName));
                }
            }
            else
            {
                return Parts(Part(NoWindowMessage, color, fontname: FontName));
            }
        }

        protected IBarWidgetPart Part(IWindow window)
        {
            var process = Process.GetProcessById(window.ProcessId);
            return new TitleWidgetTitlePart()
            {
                Text = window.Title,
                ProcessPath = process?.MainModule?.FileName ?? string.Empty
            };
        }

        public override void Initialize()
        {
            Context.Workspaces.WindowAdded += RefreshAddRemove;
            Context.Workspaces.WindowRemoved += RefreshAddRemove;
            Context.Workspaces.WindowUpdated += RefreshUpdated;
            Context.Workspaces.FocusedMonitorUpdated += RefreshFocusedMonitor;
        }

        private IWindow GetWindow()
        {
            var currentWorkspace = Context.WorkspaceContainer.GetWorkspaceForMonitor(Context.Monitor);
            return currentWorkspace.FocusedWindow ??
                   currentWorkspace.LastFocusedWindow ??
                   currentWorkspace.ManagedWindows.FirstOrDefault();
        }

        private void RefreshAddRemove(IWindow window, IWorkspace workspace)
        {
            var currentWorkspace = Context.WorkspaceContainer.GetWorkspaceForMonitor(Context.Monitor);
            if (workspace == currentWorkspace)
            {
                Context.MarkDirty();
            }
        }

        private void RefreshUpdated(IWindow window, IWorkspace workspace)
        {
            var currentWorkspace = Context.WorkspaceContainer.GetWorkspaceForMonitor(Context.Monitor);
            if (workspace == currentWorkspace && window == GetWindow())
            {
                Context.MarkDirty();
            }
        }

        private void RefreshFocusedMonitor()
        {
            Context.MarkDirty();
        }

        public static string GetShortTitle(string title)
        {
            var parts = title.Split(new char[] { '-', '—', '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                return title.Trim();
            }
            return parts.Last().Trim();
        }
    }
}
