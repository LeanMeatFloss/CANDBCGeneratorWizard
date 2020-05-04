using System.Collections.Generic;
using System.Xml.Linq;
namespace ArxmlFormater.ElementHelper
{
    public interface IHasParametersDefinitions : ISupportElement
    {
        IEnumerable<ElementBase> Parameters { get; }
        //void AddParameters (params ISupportParameterElement[] Parameters);
    }
}