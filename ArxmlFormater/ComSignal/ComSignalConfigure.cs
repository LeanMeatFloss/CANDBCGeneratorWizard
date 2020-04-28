using System.Xml.Linq;
namespace ArxmlFormater.ComSignal
{
    public class ComSignalConfigure
    {
        private XElement Container = null;
        public ComSignalConfigure SetContainer (XElement container)
        {
            this.Container = container;
        }

    }
}