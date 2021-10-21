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
    public class TitleWidgetTitlePart : BarWidgetLabelPart, IBarWidgetPart
    {
        public int Margin { get; set; }
        public string ProcessPath { get; set; }
        public override Control CreateControl()
        {
            var panel = new FlowLayoutPanel
            {
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom,
                Location = new Point(0, 0),
                Margin = new Padding(Margin, 0, Margin, 0),
                Size = new Size(50, 50),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                ForeColor = ColorToColor(ForegroundColor),
                BackColor = ColorToColor(BackgroundColor)
            };

            var pictureBox = new PictureBox
            {
                Padding = new Padding(0),
                Margin = new Padding(0),
                Image = GetIcon(),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            panel.Controls.Add(pictureBox);

            var label = base.CreateControl();
            panel.Controls.Add(label);

            return panel;
        }

        public override void UpdateControl(Control control)
        {
            var panel = (FlowLayoutPanel)control;

            var label = panel.Controls.OfType<Label>().ElementAt(0);
            base.UpdateControl(label);

            var size = label.ClientSize.Height;

            var pictureBox = panel.Controls.OfType<PictureBox>().ElementAt(0);
            pictureBox.Image = GetIcon();
            pictureBox.Height = size;
            pictureBox.Width = size;
        }

        private System.Drawing.Color ColorToColor(Color color)
        {
            return System.Drawing.Color.FromArgb(color.R, color.G, color.B);
        }

        private Bitmap GetIcon()
        {
            if (!string.IsNullOrEmpty(ProcessPath))
            {
                var result = Icon.ExtractAssociatedIcon(ProcessPath);
                return Bitmap.FromHicon(result.Handle);
            }

            return null;
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
                if (!IsShortTitle)
                {
                    return Parts(Part(window, window.Title, color, fontname: FontName));
                }
                else
                {
                    var shortTitle = GetShortTitle(window.Title);
                    return Parts(Part(window, shortTitle, color, fontname: FontName));
                }
            }
            else
            {
                return Parts(Part(null, NoWindowMessage, color, fontname: FontName));
            }
        }

        protected IBarWidgetPart Part(IWindow window, string windowTitle, Color fore = null, Color back = null, Action partClicked = null, string fontname = null)
        {
            var processPath = string.Empty;
            if(window != null)
            {
                var process = Process.GetProcessById(window.ProcessId);
                processPath = process?.MainModule?.FileName ?? string.Empty;
            }

            return new TitleWidgetTitlePart()
            {
                Text = windowTitle,
                ProcessPath = processPath,
                ForegroundColor = fore,
                BackgroundColor = back,
                FontName = fontname,
                PartClicked = partClicked,
                FontSize = 10,
                Margin = 5
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
