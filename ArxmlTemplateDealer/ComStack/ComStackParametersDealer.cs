using System.Collections.Generic;
using System.IO;
using ArxmlFormater.ElementHelper;
using DBCParser;
using NetCoreSystemEnvHelper;
using Newtonsoft.Json.Linq;
namespace ArxmlTemplateDealer.Com.ComConfig.ComSignals
{
    public class ComStackParametersDealer
    {
        public static void ConfigureArxmlUsingDBCFiles (NodeItem DBCNode, IList<ElementBase> elementList)
        {
            string filePath = FileSysHelper.GetCurrentAppLocationPath () + "\\Resources\\ArxmlTemplateDealerResources\\Template.json";
            //Seraching to locate
            JObject canStackConfigureTemplate = JObject.Parse (File.ReadAllText (filePath)).Value<JObject> ("ComStack");
            IEnumerable<ElementBase> fliteredElements = ArxmlHelper.SearchingElementsByConfigure (canStackConfigureTemplate, elementList);
            //Configure the parameters By ParametersTemplate
            JObject canStackConfigureParametersTemplate = canStackConfigureTemplate.Value<JObject> ("ParametersTemplate");

        }

    }
}