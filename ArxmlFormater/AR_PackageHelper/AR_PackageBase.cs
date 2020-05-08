using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ArxmlFormater.ElementHelper;
namespace ArxmlFormater.AR_PackageHelper
{
    public class AR_PackageBase
    {
        public static IEnumerable<AR_PackageBase> ReadArxmlFileRoot (XElement root, string filePath)
        {

            var res = root.Elements ().Where (ele => ele.Name.LocalName.Equals ("AR-PACKAGES")).FirstOrDefault ()?.Elements ().Select (ele => new ArxmlFormater.AR_PackageHelper.AR_PackageBase () { AR_PackageArxml = ele, FilePath = filePath });
            if (res == null)
            {
                return new List<AR_PackageBase> ();
            }
            else
            {
                return res;
            }
        }
        public bool IsChanged { get; set; }

        public static void GetAllARPackagesAndElements (AR_PackageBase packageSet, IList<AR_PackageBase> packageList, IList<ElementBase> elementList)
        {
            if (packageSet.Elements != null)
            {
                if (packageList != null)
                {
                    packageList.Add (packageSet);
                }
                if (elementList != null)
                {
                    foreach (var element in packageSet.Elements)
                    {
                        elementList.Add (element);
                    }
                }

            }
            if (packageSet.AR_Packages != null)
            {
                foreach (var package in packageSet.AR_Packages)
                {
                    GetAllARPackagesAndElements (package, packageList, elementList);
                }
            }
        }
        public string FilePath { get; set; }
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
        public IEnumerable<ElementBase> Elements => AR_PackageArxml.Elements ().Where (ele => ele.Name.LocalName.Equals ("ELEMENTS")).FirstOrDefault ()?.Elements ().Select (ele => new ElementBase () { ArxmlElement = ele, FilePath = FilePath, Path = Path + @"/" + PackageName });
        public IEnumerable<AR_PackageBase> AR_Packages => AR_PackageArxml.Elements ().Where (ele => ele.Name.LocalName.Equals ("AR-PACKAGES")).FirstOrDefault ()?.Elements ().Select (ele => new AR_PackageBase () { AR_PackageArxml = ele, FilePath = FilePath, Path = Path + @"/" + PackageName });
    }
}