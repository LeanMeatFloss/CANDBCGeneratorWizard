using System.Collections.Generic;
namespace DBCParser
{
    public class MessageItem
    {
        public uint MessageId { get; set; }
        public string MessageName { get; set; }
        public uint MessageSize { get; set; }
        public string Transmitter { get; set; }
        public IList<SignalItem> SignalList { get; set; }
    }
}