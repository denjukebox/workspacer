using System;

namespace workspacer.Background
{
    public class BackgroundItem
    {
        public BackgroundContentType Type { get { return _type; } }
        public string Content { get { return _content; } }
        private BackgroundContentType _type { get; set; }
        private string _content { get; set; }

        public BackgroundItem(BackgroundContentType type, string content)
        {
            _type = type;
            _content = content;
        }

        public BackgroundItem(BackgroundContentType type, Color content)
        {
            _type = type;
            _content = content.ToString();
        }
    }

    public enum BackgroundContentType
    {
        Image,
        Color
    }
}
