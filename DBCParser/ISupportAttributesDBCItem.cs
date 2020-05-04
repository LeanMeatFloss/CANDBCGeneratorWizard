using System.Collections.Generic;
namespace DBCParser
{
    public interface ISupportAttributesDBCItem
    {
        Dictionary<string, string> AttributesDict { get; set; }

    }
}