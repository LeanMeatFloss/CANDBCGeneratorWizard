using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ArxmlFormater.ElementHelper;
namespace ArxmlFormater.AR_PackageHelper
{
    public class AR_PackageBase
    {
        public XElement AR_PackageArxml { get; set; }
        public string Path { get; set; } = "";
        public string PackageName
        {
            get => AR_PackageArxml.Elements ().Where (ele => ele.Name.LocalName.Equals ("SHORT-NAME")).FirstOrDefault ()?.Value;
            set
            {
                if (AR_PackageArxml.Elements ().Where (ele => ele.Name.LocalName.Equals ("SHORT-NAME")).FirstOrDefault () == null)
                    AR_PackageArxml.Add (new XElement (XName.Get ("SHORT-NAME", AR_PackageArxml.Name.NamespaceName)) { Value = value });
                else
                    AR_PackageArxml.Elements ().Where (ele => ele.Name.LocalName.Equals ("SHORT-NAME")).FirstOrDefault ().Value = value;
            }
        }
        public IEnumerable<ElementBase> Elements => AR_PackageArxml.Elements ().Where (ele => ele.Name.LocalName.Equals ("ELEMENTS")).FirstOrDefault ()?.Elements ().Select (ele => new ElementBase () { ArxmlElement = ele });
        public IEnumerable<AR_PackageBase> AR_Packages => AR_PackageArxml.Elements ().Where (ele => ele.Name.LocalName.Equals ("AR-PACKAGES")).FirstOrDefault ()?.Elements ().Select (ele => new AR_PackageBase () { AR_PackageArxml = ele, Path = Path + "/" + PackageName });
    }
}