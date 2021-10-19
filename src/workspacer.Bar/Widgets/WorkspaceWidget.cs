using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Timers;

namespace workspacer.Bar.Widgets
{
    public static class WorkspaceWidgetResources
    {
        public const string WorkspaceHasFocusColorKey = "workspacer.Bar.Widgets.WorkspaceWidget.WorkspaceHasFocusColor";
        public const string WorkspaceEmptyColorKey = "workspacer.Bar.Widgets.WorkspaceWidget.WorkspaceEmptyColor";
        public const string WorkspaceFilledColorKey = "workspacer.Bar.Widgets.WorkspaceWidget.WorkspaceFilledColor";

        public const string WorkspaceIndicatingBackColorKey = "workspacer.Bar.Widgets.WorkspaceWidget.WorkspaceIndicatingBackColor";
        public const string WorkspaceBackColorKey = "workspacer.Bar.Widgets.WorkspaceWidget.WorkspaceBackColor";

        internal static readonly Color WorkspaceHasFocusColorDefault = Color.Red;
        internal static readonly Color WorkspaceEmptyColorDefault = Color.Gray;
        internal static readonly Color WorkspaceFilledColorDefault = Color.White;
        internal static readonly Color WorkspaceIndicatingBackColorDefault = Color.Teal;
    }

    public class WorkspaceWidget : BarWidgetBase
    {
        public int BlinkPeriod { get; set; } = 1000;

        private Timer _blinkTimer;
        private ConcurrentDictionary<IWorkspace, bool> _blinkingWorkspaces;

        public override void Initialize()
        {
            Context.Workspaces.WorkspaceUpdated += () => UpdateWorkspaces();
            Context.Workspaces.WindowMoved += (w, o, n) => UpdateWorkspaces();

            _blinkingWorkspaces = new ConcurrentDictionary<IWorkspace, bool>();

            _blinkTimer = new Timer(BlinkPeriod);
            _blinkTimer.Elapsed += (s, e) => BlinkIndicatingWorkspaces();
            _blinkTimer.Enabled = true;
        }

        public override IBarWidgetPart[] GetParts()
        {
            var parts = new List<IBarWidgetPart>();
            var workspaces = Context.WorkspaceContainer.GetWorkspaces(Context.Monitor);
            int index = 0;
            foreach (var workspace in workspaces)
            {
                parts.Add(CreatePart(workspace, index));
                index++;
            }
            return parts.ToArray();
        }

        private bool WorkspaceIsIndicating(IWorkspace workspace)
        {
            if (workspace.IsIndicating)
            {
                if (_blinkingWorkspaces.ContainsKey(workspace))
                {
                    _blinkingWorkspaces.TryGetValue(workspace, out bool value);
                    return value;
                } else
                {
                    _blinkingWorkspaces.TryAdd(workspace, true);
                    return true;
                }
            }
            else if (_blinkingWorkspaces.ContainsKey(workspace))
            {
                _blinkingWorkspaces.TryRemove(workspace, out bool _);
            }
            return false;
        }

        private IBarWidgetPart CreatePart(IWorkspace workspace, int index)
        {
            var backColor = WorkspaceIsIndicating(workspace) ? 
                Colors.GetColorByKey(WorkspaceWidgetResources.WorkspaceIndicatingBackColorKey, WorkspaceWidgetResources.WorkspaceIndicatingBackColorDefault) : null;

            return Part(GetDisplayName(workspace, index), GetDisplayColor(workspace, index), backColor, () =>
            {
                Context.Workspaces.SwitchMonitorToWorkspace(Context.Monitor.Index, index);
            },
            FontName);
        }

        private void UpdateWorkspaces()
        {
            Context.MarkDirty();
        }

        protected virtual string GetDisplayName(IWorkspace workspace, int index)
        {
            var monitor = Context.WorkspaceContainer.GetCurrentMonitorForWorkspace(workspace);
            var visible = Context.Monitor == monitor;

            return visible ? LeftPadding + workspace.Name + RightPadding : workspace.Name;
        }

        protected virtual Color GetDisplayColor(IWorkspace workspace, int index)
        {
            var monitor = Context.WorkspaceContainer.GetCurrentMonitorForWorkspace(workspace);
            if (Context.Monitor == monitor)
            {
                return Colors.GetColorByKey(WorkspaceWidgetResources.WorkspaceHasFocusColorKey, WorkspaceWidgetResources.WorkspaceHasFocusColorDefault) ;
            }

            var hasWindows = workspace.ManagedWindows.Count != 0;
            return hasWindows ?
                Colors.GetColorByKey(WorkspaceWidgetResources.WorkspaceFilledColorKey, WorkspaceWidgetResources.WorkspaceFilledColorDefault) :
                Colors.GetColorByKey(WorkspaceWidgetResources.WorkspaceEmptyColorKey, WorkspaceWidgetResources.WorkspaceEmptyColorDefault);
        }

        private void BlinkIndicatingWorkspaces()
        {
            var workspaces = _blinkingWorkspaces.Keys;

            var didFlip = false;
            foreach (var workspace in workspaces)
            {
                if (_blinkingWorkspaces.TryGetValue(workspace, out bool value))
                {
                    _blinkingWorkspaces.TryUpdate(workspace, !value, value);
                    didFlip = true;
                }
            }

            if (didFlip)
            {
                Context.MarkDirty();
            }
        }
    }
}
