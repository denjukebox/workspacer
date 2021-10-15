using System;

namespace workspacer.Background
{
    public class BackgroundItem
    {
        public BackgroundContentType Type { get { return _type; } }
        public Uri Content { get { return _content; } }
        private BackgroundContentType _type { get; set; }
        private Uri _content { get; set; }

        public BackgroundItem(BackgroundContentType type, Uri content)
        {
            _type = type;
            _content = content;
        }
    }

    public enum BackgroundContentType
    {
        Image,
        Video
    }
}
