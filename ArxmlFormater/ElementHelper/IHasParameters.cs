using System.Collections.Generic;
using System.Xml.Linq;
namespace ArxmlFormater.ElementHelper
{
    public interface IHasParameters : ISupportElement
    {
        IEnumerable<ISupportParameterElement> Parameters { get; }
        void AddParameters (params ISupportParameterElement[] Parameters);
    }
}