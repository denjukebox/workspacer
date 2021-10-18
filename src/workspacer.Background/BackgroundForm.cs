using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace workspacer.Background
{
    public partial class BackgroundForm : Form
    {
        private static Logger Logger = Logger.Create();

        private System.Threading.Timer _intervalTimer;
        private IBackgroundPluginConfig _config;    
        
        private readonly string[] _extensions = { ".jpg", ".png" };

        public BackgroundForm(IBackgroundPluginConfig config)
        {
            _config = config;

            InitializeComponent();
            ConfigureControl(_config.AssignedMonitor);

            // force handle get so that the window handle is created
            var handle = Handle;
            Logger.Debug("Background[{0}] - handle created", handle);
        }

        public void ShowBackground()
        {
            Invoke((MethodInvoker)(() =>
            {
                Show();
                SendToBack();

                if (_config is MultiBackgroundPluginConfig)
                {
                    var multiConfig = _config as MultiBackgroundPluginConfig;
                    PreProcessBackgroundFolders(multiConfig);
                    InitTimer(multiConfig.Interval);
                    return;
                }

                UpdateTrigger(null);
            }));
        }

        private void ConfigureControl(IMonitor monitor)
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = System.Drawing.Color.Transparent;

            FormBorderStyle = FormBorderStyle.None;
            ControlBox = false;

            Location = new Point(monitor.X, monitor.Y);
            Width = monitor.Width;
            Height = monitor.Height;
        }

        private void InitTimer(TimeSpan interval)
        {
            _intervalTimer = new System.Threading.Timer(new TimerCallback(UpdateTrigger), null, 0, (long)interval.TotalMilliseconds);
        }

        private void PreProcessBackgroundFolders(MultiBackgroundPluginConfig config)
        {
            var folderItems = config.Backgrounds.Where(x => x.Type == BackgroundContentType.Folder).ToList();
            foreach (var folderItem in folderItems)
            {
                var images = Directory.GetFiles(folderItem.Content, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(f => _extensions.Contains(Path.GetExtension(f).ToLower()))
                    .Select(f => new BackgroundItem(BackgroundContentType.Image, f)).ToList();

                var originalIndex = config.Backgrounds.IndexOf(folderItem);
                config.Backgrounds.RemoveAt(originalIndex);
                config.Backgrounds.InsertRange(originalIndex, images);
            }
        }

        private void UpdateTrigger(object state)
        {
            Invoke((MethodInvoker)(() =>
            {
                Refresh();
            }));
        }

        private void ProcessContent(PaintEventArgs e)
        {
            BackgroundItem item = null;
            if (_config is SingleBackgroundPluginConfig)
            {
                var bgConfig = _config as SingleBackgroundPluginConfig;
                item = bgConfig.Background;
            }
            if (_config is MultiBackgroundPluginConfig)
            {
                var bgImageConfig = _config as MultiBackgroundPluginConfig;
                item = bgImageConfig.GetNext();
            }

            PaintContent(e, item, _config.AssignedMonitor);
        }

        private void PaintContent(PaintEventArgs e, BackgroundItem item, IMonitor monitor)
        {
            if(item == null)
            {
                return;
            }

            if(item.Type == BackgroundContentType.Color)
            {
                var parts = item.Content.Split(';').Select(x => int.Parse(x));
                var color = System.Drawing.Color.FromArgb(1, parts.ElementAt(0), parts.ElementAt(1), parts.ElementAt(2));
                e.Graphics.FillRectangle(new SolidBrush(color), 0, 0, monitor.Width, monitor.Height);
            }
            if(item.Type == BackgroundContentType.Image)
            {
                e.Graphics.DrawImage(Image.FromFile(item.Content), 0, 0, monitor.Width, monitor.Height);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // Set the form click-through
                //cp.ExStyle |= 0x80000 /* WS_EX_LAYERED */ | 0x20 /* WS_EX_TRANSPARENT */;

                // don't steal focus
                //cp.Style |= 0x00040000 /*WS_THICKFRAME */;
                cp.ExStyle |= 0x08000000 /* WS_EX_NOACTIVATE */ | 0x00000080 /* WS_EX_TOOLWINDOW */;
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ProcessContent(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e) { /* Ignore */ }
    }
}
