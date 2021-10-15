using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace workspacer.Background
{
    public partial class BackgroundForm : Form
    {
        private static Logger Logger = Logger.Create();

        private System.Threading.Timer _intervalTimer;

        private IBackgroundPluginConfig _config;

        public BackgroundForm(IBackgroundPluginConfig config)
        {
            _config = config;

            InitializeComponent();
            ConfigureControl(_config.AssignedMonitor);

            // force handle get so that the window handle is created
            var handle = Handle;
            Logger.Debug("Background[{0}] - handle created", handle);
        }

        private void ConfigureControl(IMonitor monitor)
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = System.Drawing.Color.Transparent;

            FormBorderStyle = FormBorderStyle.None;
            ControlBox = false;

            Location = new Point(0, 0);
            Width = monitor.Width;
            Height = monitor.Height;
        }

        private void InitTimer(int interval)
        {
            _intervalTimer = new System.Threading.Timer(new TimerCallback(IntervalTriggerd), null, 0, interval);
        }

        private void IntervalTriggerd(object state)
        {
            this.Invoke((MethodInvoker)(() =>
            {
                Refresh();
            }));
        }

        private void PaintContent(PaintEventArgs e)
        {
            if (_config is SolidColorBackgroundPluginConfig)
            {
                var colorConfig = _config as SolidColorBackgroundPluginConfig;
                e.Graphics.Clear(System.Drawing.Color.FromArgb(1, colorConfig.Background.R, colorConfig.Background.G, colorConfig.Background.B));
            }
            if (_config is SingleBackgroundPluginConfig)
            {
                var bgConfig = _config as SingleBackgroundPluginConfig;
                if (bgConfig.Background.Type == BackgroundContentType.Image)
                {
                    e.Graphics.DrawImage(Image.FromFile(bgConfig.Background.Content.AbsolutePath), 0, 0, _config.AssignedMonitor.Width, _config.AssignedMonitor.Height);
                }
            }
            if (_config is MultiBackgroundPluginConfig)
            {
                var bgImageConfig = _config as MultiBackgroundPluginConfig;
                e.Graphics.DrawImage(Image.FromFile(bgImageConfig.GetNext().Content.AbsolutePath), 0, 0, _config.AssignedMonitor.Width, _config.AssignedMonitor.Height);
            }
        }

        public void ShowBackground()
        {
            this.Invoke((MethodInvoker)(() =>
            {
                this.Show();
                this.SendToBack();
                if (_config is MultiBackgroundPluginConfig)
                {
                    InitTimer(((MultiBackgroundPluginConfig)_config).Interval);
                }
            }));
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
            PaintContent(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e) { /* Ignore */ }
    }
}
