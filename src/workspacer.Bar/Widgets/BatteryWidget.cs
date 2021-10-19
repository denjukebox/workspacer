using System.Windows.Forms;

namespace workspacer.Bar.Widgets
{
    public class BatteryWidgetDesign
    {
        public const string LOWCHARGECOLOR_KEY = "workspacer.Bar.Widgets.BatteryWidget.LowChargeColor";
        public const string MEDCHARGECOLOR_KEY = "workspacer.Bar.Widgets.BatteryWidget.MedChargeColor";
        public const string HIGHCHARGECOLOR_KEY = "workspacer.Bar.Widgets.BatteryWidget.HighChargeColor";
    }

    public class BatteryWidget : BarWidgetBase
    {
        private Color _lowChargeColorDefault = Color.Red;
        private Color _medChargeColor = Color.Yellow;
        private Color _highChargeColor = Color.Green;

        public bool HasBatteryWarning { get; set; } = true;
        public double LowChargeThreshold { get; set; } = 0.10;
        public double MedChargeThreshold { get; set; } = 0.50;
        public int Interval { get; set; } = 5000;
        

        private System.Timers.Timer _timer;

        public override IBarWidgetPart[] GetParts()
        {
            PowerStatus pwr = SystemInformation.PowerStatus;
            float currentBatteryCharge = pwr.BatteryLifePercent;

            if (HasBatteryWarning)
            {
                if (currentBatteryCharge <= LowChargeThreshold)
                {
                    return Parts(Part(currentBatteryCharge.ToString("#0%"), Colors.GetColorFromDesign(BatteryWidgetDesign.LOWCHARGECOLOR_KEY, _lowChargeColorDefault), fontname: FontName));
                }
                else if (currentBatteryCharge <= MedChargeThreshold)
                {
                    return Parts(Part(currentBatteryCharge.ToString("#0%"), Colors.GetColorFromDesign(BatteryWidgetDesign.MEDCHARGECOLOR_KEY, _medChargeColor), fontname: FontName));
                }
                else
                {
                    return Parts(Part(currentBatteryCharge.ToString("#0%"), Colors.GetColorFromDesign(BatteryWidgetDesign.HIGHCHARGECOLOR_KEY, _highChargeColor), fontname: FontName));
                }
            }
            else
            {
                return Parts(Part(currentBatteryCharge.ToString("#0%"), fontname: FontName));
            }
        }

        public override void Initialize()
        {
            _timer = new System.Timers.Timer(Interval);
            _timer.Elapsed += (s, e) => Context.MarkDirty();
            _timer.Enabled = true;
        }
    }
}
