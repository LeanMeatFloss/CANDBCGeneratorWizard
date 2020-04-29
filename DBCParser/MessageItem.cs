using System.Collections.Generic;
namespace DBCParser
{
    public class MessageItem
    {
        public uint MessageId { get; set; }
        public string MessageName { get; set; }
        public uint DLC { get; set; }
        public IList<SignalItem> SignalList { get; set; }
    }
}