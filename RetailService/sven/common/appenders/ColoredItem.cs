namespace implementations.common.appenders
{
    public sealed class ColoredItem
    {
        readonly string _rtf;

        public ColoredItem(int colorIndex, string text)
        {
            _rtf = "\\cf" + colorIndex + "\r\n" + text.Replace("\r\n", "\r\n\\line ");
        }

        public override string ToString()
        {
            return _rtf;
        }
    }
}