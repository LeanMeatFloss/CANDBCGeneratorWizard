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
                    else
                    {
                        return;
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
                returnValue = ArxmlHelper.DefaultParametersParser (parametersContainer, parameter, returnValue);
                foreach (Match match in Regex.Matches (returnValue, @"\{__DBC__\.([\S]*?)\.([\S]*?)\}"))
                {
                    string valueReplace = "";
                    switch (match.Groups[1].Value)
                    {
                        case "MESSAGE":
                            var selectMessage = DBCNode
                                .ReceiveMessages
                                .Concat (DBCNode.TransimitMessages)
                                .Where (message => message.MessageName.Equals (parametersContainer.ElementName) || message.SignalList.Select (signal => signal.SignalName).Contains (parametersContainer.ElementName))
                                .First ();
                            if (selectMessage.AttributesDict.ContainsKey (match.Groups[2].Value))
                            {
                                valueReplace = selectMessage.AttributesDict[match.Groups[2].Value];
                            }
                            else
                            {
                                var property = selectMessage.GetType ().GetProperty (match.Groups[2].Value);
                                valueReplace = property.GetValue (selectMessage).ToString ();
                            }

                            break;
                        case "SIGNAL":
                            var selectSignal = DBCNode
                                .ReceiveMessages
                                .Concat (DBCNode.TransimitMessages)
                                .SelectMany (message => message.SignalList)
                                .Where (signal => signal.SignalName.Equals (parametersContainer.ElementName))
                                .First ();
                            if (selectSignal.AttributesDict.ContainsKey (match.Groups[2].Value))
                            {
                                valueReplace = selectSignal.AttributesDict[match.Groups[2].Value];
                            }
                            else
                            {
                                var property = selectSignal.GetType ().GetProperty (match.Groups[2].Value);
                                valueReplace = property.GetValue (selectSignal).ToString ();
                            }
                            break;
                        case "NODE":
                            if (DBCNode.AttributesDict.ContainsKey (match.Groups[2].Value))
                            {
                                valueReplace = DBCNode.AttributesDict[match.Groups[2].Value];
                            }
                            else
                            {
                                var property = DBCNode.GetType ().GetProperty (match.Groups[2].Value);
                                valueReplace = property.GetValue (DBCNode).ToString ();
                            }
                            break;
                    }
                    returnValue = returnValue.Replace (match.Value, valueReplace);

                }

                parameter.Value = returnValue;
            }
            return canStackConfigure;
        }
    }
}