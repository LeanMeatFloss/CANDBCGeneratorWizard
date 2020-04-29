using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace DBCParser
{
    public class DBCFileParser
    {
        public static IList<MessageItem> ParserMessage (string text)
        {
            //First Read All Block of Messages
            string MessageBlockMatchPattern = @"^(BO_[\s\S]*?)(BO_|BS_|BU_|NS_|VERSION|CM_|BA_)";
            var res = Regex.Match (text, MessageBlockMatchPattern, RegexOptions.Multiline);
            return null;
        }

    }
}