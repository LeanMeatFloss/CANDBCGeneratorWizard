using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ArxmlFormater.AR_PackageHelper;
using ArxmlFormater.ElementHelper;
using ArxmlTemplateDealer.ComStack;
using DBCParser;
using NetCoreSystemEnvHelper;
using ProjectHelper;
using Xunit;
namespace ArxmlTemplateDealer.Tests
{
    public class ArxmlTemplateDealerShould
    {
        string DBCText = File.ReadAllText (FileSysHelper.GetCurrentAppLocationPath () + "\\Resources\\ArxmlTemplateDealer.TestsResources\\dbcForTest.dbc");
        [Fact]
        public void ConfigureParametersByTemplateTest ()
        {
            string projectDirPath = @"C:\Users\ITQ2CS\rtc_sandbox\rbd_briBk10_gly_sw_Development_RWS_Hansong_Dev\rbd";
            var pathAll = ProjectFileFilter.SearchForDirectory (projectDirPath, false,
                new List<string> () { @"\\[\S]+\.arxml$" },
                new List<string> ()
                {
                    @"\\generated\\",
                    @"\\pf\\",
                });
            List<AR_PackageBase> packageList = new List<AR_PackageBase> ();
            List<ElementBase> elementList = new List<ElementBase> ();
            // LoadElements
            var list = pathAll
                .Select (path => new { ELE = XElement.Parse (File.ReadAllText (path)), Path = path })
                .SelectMany (ele => AR_PackageBase.ReadArxmlFileRoot (ele.ELE, ele.Path))
                //.Where (ele => ele != null)
                .ToList ();
            foreach (var item in list)
            {
                AR_PackageBase.GetAllARPackagesAndElements (item, packageList, elementList);
            }

            ComStackParametersDealer.ConfigureArxmlUsingDBCFiles (DBCFileParser.ParserNodes (DBCText).Where (elementList => elementList.NodeName.Equals ("ED")).First (), elementList);
            //ComStackParametersDealer.ConfigureArxmlUsingDBCFiles()
            Assert.Equal (10, pathAll.Length);
        }
    }
}