using System.Collections.Generic;
namespace DBCParser
{
    public class NodeItem : ISupportAttributesDBCItem
    {
        public string NodeName { get; set; }
        public IList<MessageItem> TransimitMessages { get; set; }
        public IList<MessageItem> ReceiveMessages { get; set; }
        public Dictionary<string, string> AttributesDict { get; set; } = new Dictionary<string, string> ();
        public const string TypeHead = "BU_";
        public string Comment { get; set; }
    }
}