using System;
using System.Globalization;
using System.Text;
namespace sven.common.appenders
{
    public sealed class ColoredItem
    {
        readonly string _rtf;

        public ColoredItem(int colorIndex, string text)
        {
            
            _rtf = "\\cf" + colorIndex + "\r\n" + text.Replace("\r\n", "\r\n\\line ");
        }

        public static String getRTFString(String plainStr)
        {

            StringBuilder builder = new StringBuilder();
            foreach (Char c in plainStr)
            {
                int code = Convert.ToInt32(c);
                if (Char.IsLetter(Convert.ToString(c), 0) && c < 0x80)
                {

                    builder.Append(c);
                } else {

                    builder.AppendFormat(CultureInfo.InvariantCulture, "\\u{0}{1}", code, removeDiacritics(c));
                }
                
            }

            return builder.ToString();

        }

        public static String removeDiacritics(Char c) 
        {


            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                return Convert.ToString(c);
            }
            else
            {
                return "";
            }
        }


        public override string ToString()
        {
            return _rtf;
        }
    }
}