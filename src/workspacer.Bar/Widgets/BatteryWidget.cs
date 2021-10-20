using System.Windows.Forms;

namespace workspacer.Bar.Widgets
{
    public class BatteryWidgetResources
    {
        public const string LowChargeColor = "workspacer.Bar.Widgets.BatteryWidget.LowChargeColor";
        public const string MedChargeColor = "workspacer.Bar.Widgets.BatteryWidget.MedChargeColor";
        public const string HighChargeColor = "workspacer.Bar.Widgets.BatteryWidget.HighChargeColor";

        internal static readonly Color LowChargeColorDefault = Color.Red;
        internal static readonly Color MedChargeColorDefault = Color.Yellow;
        internal static readonly Color HighChargeColorDefault = Color.Green;
    }

    public class BatteryWidget : BarWidgetBase
    {
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
                    return Parts(Part(currentBatteryCharge.ToString("#0%"), Colors.GetColorByKey(BatteryWidgetResources.LowChargeColor, BatteryWidgetResources.LowChargeColorDefault), fontname: FontName));
                }
                else if (currentBatteryCharge <= MedChargeThreshold)
                {
                    return Parts(Part(currentBatteryCharge.ToString("#0%"), Colors.GetColorByKey(BatteryWidgetResources.MedChargeColor, BatteryWidgetResources.MedChargeColorDefault), fontname: FontName));
                }
                else
                {
                    return Parts(Part(currentBatteryCharge.ToString("#0%"), Colors.GetColorByKey(BatteryWidgetResources.HighChargeColor, BatteryWidgetResources.HighChargeColorDefault), fontname: FontName));
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
