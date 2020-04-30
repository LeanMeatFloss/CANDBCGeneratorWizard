using System.Collections.Generic;
using System.IO;
using ArxmlFormater.ElementHelper;
using DBCParser;
using NetCoreSystemEnvHelper;
using Newtonsoft.Json.Linq;
namespace ArxmlTemplateDealer.ComStack
{
    public class ComStackParametersDealer
    {
        public static void ConfigureArxmlUsingDBCFiles (NodeItem DBCNode, IList<ElementBase> elementList)
        {
            string filePath = FileSysHelper.GetCurrentAppLocationPath () + "\\Resources\\ArxmlTemplateDealerResources\\Template.json";
            //Seraching to locate
            var canStackConfigureTemplateList = JObject.Parse (File.ReadAllText (filePath)).Value<JObject> ("ComStack").Value<JArray> ("Template");
            foreach (JObject template in canStackConfigureTemplateList)
            {
                IEnumerable<ElementBase> fliteredElements = ArxmlHelper.SearchingElementsByConfigure (template, elementList);
                //Configure the parameters By ParametersTemplate
                JObject canStackConfigureParametersTemplate = template.Value<JObject> ("ParametersTemplate");
            }

            //  var list = ArxmlTemplateDealer.ArxmlHelper.SearchingElementsByConfigure (canStackConfigureParametersTemplate, elementList);
        }

    }
}