using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                .Select (path => new { ELE = XElement.Parse (File.ReadAllText (path)), FilePath = path })
                .SelectMany (ele => AR_PackageBase.ReadArxmlFileRoot (ele.ELE, ele.FilePath))
                //.Where (ele => ele != null)
                .ToList ();
            //watching the change of xml
            StringBuilder changList = new StringBuilder ();
            changList.AppendJoin (',', "Package File", "Operation Target", "Operation Type", "Parameter Type", "Parameter Def", "Parameter Value", "\n");
            foreach (var packageRoot in list)
            {
                packageRoot.AR_PackageArxml.Changed += (sender, e) =>
                {
                    packageRoot.IsChanged = true;
                    var element = sender as XElement;
                    //search element until find ECUC container
                    var parent = element;
                    var parameter = element;
                    while (parent != null && parent.Name.LocalName != "ECUC-CONTAINER-VALUE")
                    {
                        parent = parent.Parent;
                    }
                    while (parameter != null && !parameter.Name.LocalName.Contains ("PARAM-VALUE"))
                    {
                        if (parameter.Parent != null)
                        {
                            parameter = parameter.Parent;
                        }
                        else
                        {
                            parameter = null;
                            break;
                        }
                    }
                    changList.AppendJoin (',',
                        packageRoot.FilePath.Split ("\\").Last (),
                        parent?.Element (XName.Get ("SHORT-NAME", parent.Name.Namespace.NamespaceName))?.Value,
                        e.ObjectChange.ToString (),
                        parameter?.Name.LocalName,
                        parent == null? "" : parameter?.Element (XName.Get ("DEFINITION-REF", parameter.Name.NamespaceName))?.Value,
                        parameter?.Element (XName.Get ("VALUE", parameter.Name.NamespaceName))?.Value,
                        "\n");
                };
                AR_PackageBase.GetAllARPackagesAndElements (packageRoot, packageList, elementList);
            }

            ComStackParametersDealer.ConfigureArxmlUsingDBCFiles (DBCFileParser.ParserNodes (DBCText).Where (elementList => elementList.NodeName.Equals ("ED")).First (), elementList);
            //try to save and replace and backup files
            foreach (var packageRoot in list.Where (package => package.IsChanged))
            {
                File.Copy (packageRoot.FilePath, packageRoot.FilePath + "." + DateTime.Now.ToString ("yyyy-MM-dd-HH-mm-ss") + "-BAK");
                var xml = packageRoot.AR_PackageArxml;
                while (xml.Parent != null)
                {
                    xml = xml.Parent;
                }
                xml.Save (packageRoot.FilePath);
            }

            //ComStackParametersDealer.ConfigureArxmlUsingDBCFiles()
            // Assert.Equal (10, pathAll.Length);
            // string logPath = FileSysHelper.GetCurrentAppLocationPath () + "\\ChangLists\\"; //
            // if (!Directory.Exists (logPath))
            // {
            //     Directory.CreateDirectory (logPath);
            // }
            // using (StreamWriter writer = new StreamWriter (logPath + DateTime.Now.ToString ("yyyy-MM-dd") + ".csv"))
            // {
            //     writer.Write (changList.ToString ());
            // }

        }
    }
}