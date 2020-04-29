using System.Xml.Linq;
namespace ArxmlFormater.ElementHelper
{
    public interface ISupportElement
    {
        XElement ArxmlElement { get; set; }
        string ElementType { get; set; }
        string ElementName { get; set; }
        XElement NewElement (string Name);

    }
}