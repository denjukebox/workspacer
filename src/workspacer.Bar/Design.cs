using System.Collections.Generic;

namespace workspacer.Bar
{
    public static class BarDesign
    {
        public static string BACKGROUND_FILL_KEY = "workspacer.Bar.BackgroundFill";
    }

    public static class Design
    {
        public static string FOREGROUND_KEY = "workspacer.Bar.Foreground";
        public static string BACKGROUND_KEY = "workspacer.Bar.Background";
    }

    public static class DesignUtils
    {
        public static Color GetForgroundColorFromDesign(this Dictionary<string, Color> stack, Color defaultColor = null)
        {
            Color col = defaultColor;
            if (stack.ContainsKey(Design.FOREGROUND_KEY))
            {
                col = stack[Design.FOREGROUND_KEY];
            }

            return col;
        }

        public static Color GetBackgroundColorFromDesign(this Dictionary<string, Color> stack, Color defaultColor = null)
        {
            Color col = defaultColor;
            if (stack.ContainsKey(Design.BACKGROUND_KEY))
            {
                col = stack[Design.BACKGROUND_KEY];
            }

            return col;
        }

        public static Color GetColorFromDesign(this Dictionary<string, Color> stack, string key, Color defaulColor = null)
        {
            Color col = defaulColor ?? null;
            if (stack.ContainsKey(key))
            {
                col = stack[key];
            }

            return col;
        }

        public static System.Drawing.Color ColorToColor(Color color)
        {
            return System.Drawing.Color.FromArgb(color.R, color.G, color.B);
        }
    }
}
