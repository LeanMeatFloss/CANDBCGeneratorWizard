using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
                ArxmlHelper.ConfigureParametersByTemplate (fliteredElements, elementList, canStackConfigureParametersTemplate, CanStackDealerFactory (DBCNode));
            }

            //  var list = ArxmlTemplateDealer.ArxmlHelper.SearchingElementsByConfigure (canStackConfigureParametersTemplate, elementList);
        }
        static IParameterTemplateDealer CanStackDealerFactory (NodeItem DBCNode)
        {
            void canStackConfigure (ElementBase parametersContainer, ISupportParameterElement parameter, JToken template)
            {
                //first using DBC formater to help parser
                string formaterString = null;
                var receiveNameList = DBCNode.ReceiveMessages.Select (message => message.MessageName).Concat (DBCNode.ReceiveMessages.SelectMany (message => message.SignalList).Select (signal => signal.SignalName));
                var transferNameList = DBCNode.TransimitMessages.Select (message => message.MessageName).Concat (DBCNode.TransimitMessages.SelectMany (message => message.SignalList).Select (signal => signal.SignalName));
                if (template is JObject configures)
                {
                    if (configures.ContainsKey ("__RX__") && receiveNameList.Contains (parametersContainer.ElementName))
                    {
                        formaterString = configures.Value<string> ("__RX__");
                    }
                    else if (configures.ContainsKey ("__TX__") && transferNameList.Contains (parametersContainer.ElementName))
                    {
                        formaterString = configures.Value<string> ("__TX__");
                    }

                }
                else if (template is JToken configure)
                {
                    formaterString = configure.ToString ();
                }
                else
                {
                    return;
                }
                //formatString using DBC
                if (string.IsNullOrEmpty (formaterString))
                {
                    parameter.Value = string.Empty;
                    return;
                }
                string returnValue = formaterString;
                foreach (Match match in Regex.Matches (returnValue, @"\{__DBC__\.([\S]*?)\}"))
                {

                }
                returnValue = ArxmlHelper.DefaultParametersParser (parametersContainer, parameter, returnValue);

            }
            return canStackConfigure;
        }
    }
}