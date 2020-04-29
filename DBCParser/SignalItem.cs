using System;
using System.Collections.Generic;

namespace DBCParser
{
    public class SignalItem
    {
        public string Name { get; set; }
        public uint Length { get; set; }
        public enum ByteOrderEnum
        {
            Intel,
            Motorola,
        }
        public enum ValueTypeEnum
        {
            Unsigned,
            Signed,
        }
        public ByteOrderEnum ByteOrder { get; set; }
        public ValueTypeEnum ValueType { get; set; }
        public float InitialValue { get; set; }
        public float Factor { get; set; }
        public float Offset { get; set; }
        public float Max { get; set; }
        public float Min { get; set; }
        public string Unit { get; set; }
        public Dictionary<float, string> ValueTable { get; set; }
        public string Comment { get; set; }
        public Dictionary<string, string> AppendAttribute { get; set; }
    }
}