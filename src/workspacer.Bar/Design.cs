using System.Collections.Generic;

namespace workspacer.Bar
{
    public static class BarResources
    {
        public static string BackgroundFillColorKey = "workspacer.Bar.BackgroundFillColor";
        internal static readonly Color BackgroundFillColorDefault = Color.Black;
    }

    public static class DesignResources
    {
        public static string ForegroundColorKey = "workspacer.Bar.ForegroundColor";
        public static string BackgroundColorKey = "workspacer.Bar.BackgroundColor";

        internal static readonly Color BackgroundColorDefault = Color.White;
        internal static readonly Color ForegroundColorDefault = Color.Black;
    }

    public static class DesignUtils
    {
        public static Color GetForgroundColor(this IDictionary<string, Color> stack, Color defaultColor = null)
        {
            return GetColorByKey(stack, DesignResources.ForegroundColorKey, defaultColor ?? DesignResources.ForegroundColorDefault);
        }

        public static Color GetBackgroundColor(this IDictionary<string, Color> stack, Color defaultColor = null)
        {
            return GetColorByKey(stack, DesignResources.BackgroundColorKey, defaultColor ?? DesignResources.BackgroundColorDefault);
        }

        public static Color GetColorByKey(this IDictionary<string, Color> stack, string key, Color defaulColor = null)
        {
            return GetResourceByKey(stack, key, defaulColor);
        }

        public static T GetResourceByKey<T>(this IDictionary<string, T> stack, string key, T defaulValue = null) where T : class
        {
            var def = defaulValue ?? default(T);
            if (stack != null && stack.ContainsKey(key))
            {
                def = stack[key];
            }

            return def;
        }

        public static System.Drawing.Color ColorToColor(Color color)
        {
            return System.Drawing.Color.FromArgb(color.R, color.G, color.B);
        }
    }
}
