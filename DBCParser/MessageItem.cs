using System.Collections.Generic;
namespace DBCParser
{
    public class MessageItem : ISupportAttributesDBCItem
    {
        public uint MessageId { get; set; }
        public string MessageName { get; set; }
        public uint MessageSize { get; set; }
        public string Transmitter { get; set; }
        public IList<SignalItem> SignalList { get; set; }
        public Dictionary<string, string> AttributesDict { get; set; } = new Dictionary<string, string> ();
        public const string TypeHead = "BO_";
        public string Comment { get; set; }
    }
}