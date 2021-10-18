using System;

namespace workspacer.Background
{
    public class BackgroundItem
    {
        public BackgroundContentType Type { get { return _type; } }
        public string Content { get { return _content; } }
        private BackgroundContentType _type { get; set; }
        private string _content { get; set; }

        public BackgroundItem(BackgroundContentType type, string path)
        {
            _type = type;
            _content = path;
        }

        public BackgroundItem(BackgroundContentType type, Color content)
        {
            _type = type;
            _content = $"{content.R};{content.G};{content.B}";
        }
    }

    public enum BackgroundContentType
    {
        Image,
        Folder,
        Color
    }
}
