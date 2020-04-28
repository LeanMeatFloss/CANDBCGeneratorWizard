using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ArxmlFormater
{
    public class COMSignalDealer
    {
        public class
        public static class ComSignalDefinitionReferenceCollection
        {
            static string ComSignal = @"/AUTOSAR_Com/EcucModuleDefs/Com/ComConfig/ComSignal";
            static string Comsignal_ComBitPosition
        }
        public static IEnumerable<XElement> FliterComSignalCollection (XElement Containers, string ContainerName)
        {
            // Containers ()

            return Containers
                .Elements ()
                .Where (ele =>
                    ele.Name.Equals (ContainerName) &&
                    ele.Element ("DEFINITION-REF").Value.Equals (@"/AUTOSAR_Com/EcucModuleDefs/Com/ComConfig/ComSignal"));
        }

    }
}