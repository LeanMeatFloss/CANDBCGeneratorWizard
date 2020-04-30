using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
namespace ArxmlFormater.ElementHelper
{
    public class ElementBase : ISupportElement, ISupportDefinitionRefElement, ISupportParameterElement, IHasContainersElement, IHasParameters, IHasSubContainers
    {
        public XElement ArxmlElement { get; set; }
        public string FilePath { get; set; }
        public XElement NewElement (string Name)
        {
            return new XElement (XName.Get (Name, ArxmlElement.Name.NamespaceName));
        }

        public string ElementType
        {
            get => ArxmlElement.Name.LocalName;
            set => ArxmlElement.Name = value;
        }
        public string ElementName
        {
            get => ArxmlElement.Elements ().Where (ele => ele.Name.LocalName.Equals ("SHORT-NAME")).FirstOrDefault ()?.Value;
            set
            {
                if (ArxmlElement.Elements ().Where (ele => ele.Name.LocalName.Equals ("SHORT-NAME")).FirstOrDefault () == null)
                    ArxmlElement.Add (new XElement (XName.Get ("SHORT-NAME", ArxmlElement.Name.NamespaceName)) { Value = value });
                else
                    ArxmlElement.Elements ().Where (ele => ele.Name.LocalName.Equals ("SHORT-NAME")).FirstOrDefault ().Value = value;
            }
        }
        string ISupportDefinitionRefElement.DefinitionRef
        {
            get => ArxmlElement.Elements ().Where (ele => ele.Name.LocalName.Equals ("DEFINITION-REF")).FirstOrDefault ()?.Value;
            set
            {
                if (ArxmlElement.Elements ().Where (ele => ele.Name.LocalName.Equals ("DEFINITION-REF")).FirstOrDefault () == null)
                    ArxmlElement.Add (new XElement (XName.Get ("DEFINITION-REF", ArxmlElement.Name.NamespaceName)) { Value = value });
                else
                    ArxmlElement.Elements ().Where (ele => ele.Name.LocalName.Equals ("DEFINITION-REF")).FirstOrDefault ().Value = value;
            }
        }
        string ISupportDefinitionRefElement.DefinitionType
        {
            get => ArxmlElement.Elements ().Where (ele => ele.Name.LocalName.Equals ("DEFINITION-REF")).FirstOrDefault ()?.Attribute ("DEST").Value;
            set
            {
                if (ArxmlElement.Elements ().Where (ele => ele.Name.LocalName.Equals ("DEFINITION-REF")).FirstOrDefault () == null)
                    ArxmlElement.Add (new XElement (XName.Get ("DEFINITION-REF", ArxmlElement.Name.NamespaceName)));

                ArxmlElement.Elements ().Where (ele => ele.Name.LocalName.Equals ("DEFINITION-REF")).FirstOrDefault ().SetAttributeValue ("DEST", value);
            }
        }
        string ISupportParameterElement.Value
        {
            get => ArxmlElement
                .Elements ()
                .Where (ele => ele.Name.LocalName.Equals ("VALUE"))
                .FirstOrDefault () ?
                .Value;
            set
            {
                if (ArxmlElement.Elements ().Where (ele => ele.Name.LocalName.Equals ("VALUE")).FirstOrDefault () == null)
                    ArxmlElement.Add (new XElement (XName.Get ("VALUE", ArxmlElement.Name.NamespaceName)) { Value = value });
                else
                    ArxmlElement.Elements ().Where (ele => ele.Name.LocalName.Equals ("VALUE")).FirstOrDefault ().Value = value;
            }
        }
        IEnumerable<ElementBase> IHasContainersElement.Containers
        {
            get => ArxmlElement.Elements ().Where (ele => ele.Name.LocalName.Equals ("CONTAINERS")).FirstOrDefault ()?.Elements ().Select (ele => new ElementBase () { ArxmlElement = ele, FilePath = FilePath });

        }
        void IHasContainersElement.AddContainers (params ElementBase[] Containers)
        {
            if (ArxmlElement.Elements ().Where (ele => ele.Name.LocalName.Equals ("CONTAINERS")).FirstOrDefault () == null)
            {
                ArxmlElement.Add (new XElement (XName.Get ("CONTAINERS", ArxmlElement.Name.NamespaceName)));
            }
            ArxmlElement.Elements ().Where (ele => ele.Name.LocalName.Equals ("CONTAINERS")).FirstOrDefault ().Add (Containers.Select (ele => ele.ArxmlElement));
        }
        IEnumerable<ElementBase> IHasSubContainers.SubContainers
        {
            get => ArxmlElement.Elements ().Where (ele => ele.Name.LocalName.Equals ("SUB-CONTAINERS")).FirstOrDefault ()?.Elements ().Select (ele => new ElementBase () { ArxmlElement = ele, FilePath = FilePath });

        }
        void IHasSubContainers.AddSubContainers (params ElementBase[] Containers)
        {
            if (ArxmlElement.Elements ().Where (ele => ele.Name.LocalName.Equals ("SUB-CONTAINERS")).FirstOrDefault () == null)
            {
                ArxmlElement.Add (new XElement (XName.Get ("SUB-CONTAINERS", ArxmlElement.Name.NamespaceName)));
            }
            ArxmlElement.Elements ().Where (ele => ele.Name.LocalName.Equals ("SUB-CONTAINERS")).FirstOrDefault ().Add (Containers.Select (ele => ele.ArxmlElement));
        }
        IEnumerable<ISupportParameterElement> IHasParameters.Parameters
        {
            get => ArxmlElement.Elements ().Where (ele => ele.Name.LocalName.Equals ("PARAMETER-VALUES")).FirstOrDefault ()?.Elements ().Select (ele => new ElementBase () { ArxmlElement = ele, FilePath = FilePath });

        }
        void IHasParameters.AddParameters (params ISupportParameterElement[] Parameters)
        {
            if (ArxmlElement.Elements ().Where (ele => ele.Name.LocalName.Equals ("PARAMETER-VALUES")).FirstOrDefault () == null)
            {
                ArxmlElement.Add (new XElement (XName.Get ("PARAMETER-VALUES", ArxmlElement.Name.NamespaceName)));
            }
            ArxmlElement.Elements ().Where (ele => ele.Name.LocalName.Equals ("PARAMETER-VALUES")).FirstOrDefault ().Add (Parameters.Select (ele => ele.ArxmlElement));
        }

    }
}