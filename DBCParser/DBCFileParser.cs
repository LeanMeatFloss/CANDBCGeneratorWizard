using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
namespace DBCParser
{
    public class DBCFileParser
    {
        public static IList<NodeItem> ParserNodes (string dbcText)
        {
            var nodeList = GetNodes (dbcText);
            var messageList = ParserMessageBlocks (dbcText);
            return nodeList.Select (node => new NodeItem () { NodeName = node }).Select (node =>
            {
                node.TransimitMessages = messageList.Where (message => message.Transmitter.Equals (node.NodeName)).ToList ();
                node.ReceiveMessages = messageList.Where (message => message.SignalList.Where (signal => signal.Receiver.Contains (node.NodeName)).Count () != 0).ToList ();
                return node;
            }).ToList ();
        }
        internal static IList<MessageItem> ParserMessageBlocks (string dbcText)
        {
            return GetMessageBlockText (dbcText).Select (ele => MessageItemParser (ele)).ToList ();
        }

        internal static IList<string> GetNodes (string dbcText)
        {
            string nodePattern = @"^[\s]*BU_[\s]*:[\s]*([\s\S]*?)$";
            var matchRes = Regex.Match (dbcText, nodePattern, RegexOptions.Multiline);
            var res = matchRes.Groups;
            return res[1].Value.Split (new string[] { " ", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList ();
        }
        internal static IList<string> GetSignalLineText (string text)
        {
            string signalPattern = @"^[\s]*(SG_[\s]+[\s\S]+?)$";
            var res = Regex.Matches (text, signalPattern, RegexOptions.Multiline);
            return res.Select (ele => ele.Groups[1].Value).ToList ();
        }
        internal static MessageItem MessageItemParser (string messageBlock)
        {
            string messageBlockParserPattern = @"^[\s]*BO_[\s]*([0-9]+)[\s]*([\S]+)[\s]*:[\s]*([0-9]+)[\s]+([\S]+)[\s]*$";
            var matchRes = Regex.Match (messageBlock, messageBlockParserPattern, RegexOptions.Multiline);
            var res = matchRes.Groups;
            var messageItem = new MessageItem ();
            for (int i = 0; i < res.Count; i++)
            {
                switch (i)
                {
                    case 1:
                        messageItem.MessageId = uint.Parse (res[i].Value);
                        break;
                    case 2:
                        messageItem.MessageName = res[i].Value;
                        break;
                    case 3:
                        messageItem.MessageSize = uint.Parse (res[i].Value);
                        break;
                    case 4:
                        messageItem.Transmitter = res[i].Value;
                        break;
                }
            }
            messageItem.SignalList = GetSignalLineText (messageBlock).Select (ele => ParserSignalLineText (ele)).ToList ();
            return messageItem;
        }
        internal static SignalItem ParserSignalLineText (string signalLine)
        {
            string signalParserPattern = @"^[\s]*SG_[\s]+([\S]+?)[\s]*:[\s]*([0-9]+)\|([0-9]+)@([0-9])[\s]*([\+\-])[\s]*\(([\-\+0-9\.]+),([\-\+0-9\.]+)\)[\s]*\[([\-\+0-9\.]+)\|([\-\+0-9\.]+)\][\s]*" +
                "\"([\\S\\s]*)\"" +
                @"[\s]*([\S]*)[\s]*$";
            var res = Regex.Match (signalLine, signalParserPattern, RegexOptions.Multiline).Groups;
            var signalItem = new SignalItem ();
            for (int i = 0; i < res.Count; i++)
            {
                switch (i)
                {
                    case 1:
                        signalItem.SignalName = res[i].Value;
                        break;
                    case 2:
                        signalItem.StartBit = uint.Parse (res[i].Value);
                        break;
                    case 3:
                        signalItem.SignalSize = uint.Parse (res[i].Value);
                        break;
                    case 4:
                        signalItem.ByteOrder = uint.Parse (res[i].Value) == 0 ? SignalItem.ByteOrderEnum.Motorola : SignalItem.ByteOrderEnum.Intel;
                        break;
                    case 5:
                        signalItem.ValueType = res[i].Value.Contains ("+") ? SignalItem.ValueTypeEnum.Unsigned : SignalItem.ValueTypeEnum.Signed;
                        break;
                    case 6:
                        signalItem.Factor = float.Parse (res[i].Value);
                        break;
                    case 7:
                        signalItem.Offset = float.Parse (res[i].Value);
                        break;
                    case 8:
                        signalItem.Min = float.Parse (res[i].Value);
                        break;
                    case 9:
                        signalItem.Max = float.Parse (res[i].Value);
                        break;
                    case 10:
                        signalItem.Unit = res[i].Value;
                        break;
                    case 11:
                        signalItem.Receiver = res[i].Value.Split (new string[] { ",", " ", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList ();
                        break;
                }
            }
            return signalItem;
        }
        /// <summary>
        /// Get all message blocks
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        internal static IList<string> GetMessageBlockText (string text)
        {
            string MessageBlockMatchPattern = @"^(BO_[\s\S]*?)(?=BO_|BS_|BU_|NS_|VERSION|CM_|BA_)";
            var res = Regex.Matches (text, MessageBlockMatchPattern, RegexOptions.Multiline);
            return res.Select (ele => ele.Groups[1].Value).ToList ();
        }

    }
}