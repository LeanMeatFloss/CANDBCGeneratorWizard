using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ArxmlFormater;
namespace ConfigureTemplateEx
{
    class Program
    {
        static void ReadARPackages (ArxmlFormater.AR_PackageHelper.AR_PackageBase arPackage, List<ArxmlFormater.AR_PackageHelper.AR_PackageBase> packages, List<ArxmlFormater.ElementHelper.ElementBase> elements)
        {
            if (arPackage.Elements != null)
            {
                packages.Add (arPackage);
                elements.AddRange (arPackage.Elements);
            }
            if (arPackage.AR_Packages != null)
            {
                foreach (var package in arPackage.AR_Packages)
                {
                    ReadARPackages (package, packages, elements);
                }
            }
        }
        static void Main (string[] args)
        {
            Console.WriteLine ("Hello World!");
            string filePath = Console.ReadLine ();
            XElement element = XElement.Parse (File.ReadAllText (filePath));
            var ele1 = element.Elements ().Where (ele => ele.Name.LocalName.Equals ("AR-PACKAGES")).FirstOrDefault ().Elements ().FirstOrDefault ();
            var packageRoot = new ArxmlFormater.AR_PackageHelper.AR_PackageBase () { AR_PackageArxml = ele1 };
            List<ArxmlFormater.AR_PackageHelper.AR_PackageBase> packages = new List<ArxmlFormater.AR_PackageHelper.AR_PackageBase> ();
            List<ArxmlFormater.ElementHelper.ElementBase> elements = new List<ArxmlFormater.ElementHelper.ElementBase> ();
            ReadARPackages (packageRoot, packages, elements);
            var ComConfigure = elements.Where (ele => ele.ElementName.Equals ("Com")).FirstOrDefault ();
            var signalList = (ComConfigure as ArxmlFormater.ElementHelper.IHasContainersElement)
                .Containers.Where (ele => ele.ElementName.Equals ("ComConfig"))
                .Cast<ArxmlFormater.ElementHelper.IHasSubContainers> ().FirstOrDefault ().SubContainers
                .Where (ele => (ele as ArxmlFormater.ElementHelper.ISupportDefinitionRefElement).DefinitionRef.Contains ("ComSignal")).ToList ();
            foreach (ArxmlFormater.ElementHelper.IHasParameters signal in signalList)
            {
                //获取BitSize
                var bitSize = signal.Parameters.Where (ele => ele.DefinitionRef.Contains ("BitSize")).FirstOrDefault ().Value;
                var signalLength = signal.Parameters.Where (ele => ele.DefinitionRef.Contains ("ComSignalLength")).FirstOrDefault ();
                if (signalLength == null)
                {
                    signalLength = new ArxmlFormater.ElementHelper.ElementBase ()
                    {
                    ArxmlElement = ComConfigure.NewElement ("ECUC-NUMERICAL-PARAM-VALUE")
                    };
                    signalLength.DefinitionRef = "/AUTOSAR_Com/EcucModuleDefs/Com/ComConfig/ComSignal/ComSignalLength";
                    signalLength.DefinitionType = "ECUC-INTEGER-PARAM-DEF";
                    signal.AddParameters (signalLength);
                }
                //rewrite BitSize
                signalLength.Value = bitSize;
            }
            element.Save (filePath);
        }
    }
}