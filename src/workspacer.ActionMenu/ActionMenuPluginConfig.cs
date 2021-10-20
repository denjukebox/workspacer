using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer.ActionMenu
{
    public class ActionMenuPluginConfig
    {
        public bool RegisterKeybind { get; set; } = true;
        public KeyModifiers KeybindMod { get; set; } = KeyModifiers.LAlt;
        public Keys KeybindKey { get; set; } = Keys.P;

        public string MenuTitle { get; set; } = "workspacer.ActionMenu";
        public int MenuHeight { get; set; } = 50;
        public int MenuWidth { get; set; } = 500;
        public string FontName { get; set; } = "Consolas";
        public int FontSize { get; set; } = 16;

        private Dictionary<string, Color> _colors;
        public Dictionary<string, Color> Colors 
        { 
            get 
            { 
                if(_colors == null)
                {
                    return WorkspacerResources.GlobalColors;
                }

                return _colors;
            }
            set 
            {
                _colors = value;
                WorkspacerResources.GlobalColors.MergeResourceDictionaries(_colors);
            } 
        }

        public IMatcher Matcher { get; set; } = new OrMatcher(new PrefixMatcher(), new ContiguousMatcher());
    }
}
