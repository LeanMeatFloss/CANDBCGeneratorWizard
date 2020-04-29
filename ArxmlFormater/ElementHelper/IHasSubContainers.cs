using System.Collections.Generic;
namespace ArxmlFormater.ElementHelper
{
    public interface IHasSubContainers : ISupportElement
    {
        IEnumerable<ElementBase> SubContainers { get; }
        void AddSubContainers (params ElementBase[] Containers);
    }
}