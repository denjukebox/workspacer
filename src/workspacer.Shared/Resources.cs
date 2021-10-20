using System.Collections.Generic;

namespace workspacer
{
    public static class WorkspacerResources
    {
        public static Dictionary<string, Color> GlobalColors { get; set; } = new Dictionary<string, Color>();

        public static string ForegroundColor = "workspacer.ForegroundColor";
        public static string BackgroundColor = "workspacer.BackgroundColor";

        internal static readonly Color BackgroundColorDefault = Color.White;
        internal static readonly Color ForegroundColorDefault = Color.Black;

        public static Color GetForgroundColor(this IDictionary<string, Color> stack, Color defaultColor = null)
        {
            return GetColorByKey(stack, ForegroundColor, defaultColor ?? ForegroundColorDefault);
        }

        public static Color GetBackgroundColor(this IDictionary<string, Color> stack, Color defaultColor = null)
        {
            return GetColorByKey(stack, BackgroundColor, defaultColor ?? BackgroundColorDefault);
        }

        public static Color GetColorByKey(this IDictionary<string, Color> stack, string key, Color defaulColor = null)
        {
            return ResourceExtensions.GetResourceByKey(stack, key, defaulColor);
        }
    }

    public static class ResourceExtensions
    {
        public static void MergeResourceDictionaries<T>(this IDictionary<string, T> source, IDictionary<string, T> destinaton)
        {
            foreach (var configColor in source)
            {
                if (!destinaton.ContainsKey(configColor.Key))
                {
                    destinaton.Add(configColor.Key, configColor.Value);
                }
            }
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
    }
}
