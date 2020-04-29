using System.Collections.Generic;
namespace DBCParser
{
    public class NodeItem
    {
        public string NodeName { get; set; }
        public IList<MessageItem> TransimitMessages { get; set; }
        public IList<MessageItem> ReceiveMessages { get; set; }
    }
}