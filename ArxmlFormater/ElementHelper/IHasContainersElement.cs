using System.Collections.Generic;
namespace ArxmlFormater.ElementHelper
{
    public interface IHasContainersElement
    {
        IEnumerable<ElementBase> Containers { get; }
        void AddContainers (params ElementBase[] Containers);
    }
}