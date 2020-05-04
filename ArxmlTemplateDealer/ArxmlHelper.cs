using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ArxmlFormater.AR_PackageHelper;
using ArxmlFormater.ElementHelper;
using NetCoreSystemEnvHelper;
using Newtonsoft.Json.Linq;
namespace ArxmlTemplateDealer
{
    public class ArxmlHelper
    {

        public static IList<ElementBase> SearchingParametersDefinitionByPath (IEnumerable<ElementBase> elementsList, string path)
        {
            void searchingParametersDefinitions (IEnumerable<ElementBase> elementsList, string path, List<ElementBase> filteredItem)
            {
                foreach (var element in elementsList.Where (ele => path.StartsWith (ele.Path)))
                {
                    if (element.Path.Equals (path))
                    {
                        if (element.ElementType.EndsWith ("-DEF"))
                        {
                            filteredItem.Add (element);
                        }

                    }
                    var containers = (element as IHasContainersElement).Containers;
                    if (containers != null)
                    {
                        searchingParametersDefinitions (containers, path, filteredItem);
                    }
                    var subContainers = (element as IHasSubContainers).SubContainers;
                    if (subContainers != null)
                    {
                        searchingParametersDefinitions (subContainers, path, filteredItem);
                    }
                    var parameters = (element as IHasParametersDefinitions).Parameters;
                    if (parameters != null)
                    {
                        searchingParametersDefinitions (parameters, path, filteredItem);
                    }

                }
            }
            List<ElementBase> filteredItem = new List<ElementBase> ();
            //elements=>
            searchingParametersDefinitions (elementsList, path, filteredItem);
            return filteredItem;
        }
        public static void ConfigureParametersByTemplate (IEnumerable<IHasParameters> elementsList, IEnumerable<ElementBase> elementsCollection, JObject templateConfigure, IParameterTemplateDealer parameterTemplateDealer)
        {
            foreach (var templateItem in templateConfigure)
            {
                foreach (var element in elementsList)
                {
                    var parameterItem = element.Parameters.Where (parameter => Regex.IsMatch (parameter.DefinitionRef, templateItem.Key, RegexOptions.Multiline)).FirstOrDefault ();
                    //if cannot searching the items ,try to search the elements to find parameters definitions.
                    if (parameterItem == null)
                    {
                        //searching element parameters definitions
                        string path = (element as ISupportDefinitionRefElement).DefinitionRef;
                        var parametersDefinitionCollection = SearchingParametersDefinitionByPath (elementsCollection, path);
                        //searching element parameter fixed with name
                        foreach (var container in parametersDefinitionCollection)
                        {

                            //parameter checking,
                            if (Regex.IsMatch (container.ElementName, templateItem.Key, RegexOptions.Multiline))
                            {
                                //find it, try to add it!
                                //first get type name
                                string typeName = container.ElementType;

                                string definitionType = Regex.Replace (typeName, "(-INTEGER-|-FLOAT-)", "-NUMERICAL-");
                                definitionType = Regex.Replace (definitionType, "(-ENUMERATION-|-FUNCTION-)", "-TEXTUAL-");
                                definitionType = Regex.Replace (definitionType, "-DEF$", "-VALUE", RegexOptions.Multiline);
                                var ele = element.NewElement (definitionType);
                                var elementBase = new ElementBase () { ArxmlElement = ele };
                                //(elementBase as ISupportDefinitionRefElement).ElementType = definitionType;
                                (elementBase as ISupportDefinitionRefElement).DefinitionType = typeName;
                                (elementBase as ISupportDefinitionRefElement).DefinitionRef = container.Path + @"/" + container.ElementName;
                                element.AddParameters (elementBase as ISupportParameterElement);
                                parameterItem = element.Parameters.Where (parameter => Regex.IsMatch (parameter.DefinitionRef, templateItem.Key, RegexOptions.Multiline)).FirstOrDefault ();
                                break;
                            }

                        }
                    }
                    parameterTemplateDealer (element as ElementBase, parameterItem, templateItem.Value);
                    if (parameterItem.ElementType.Contains ("NUMERICAL") && !parameterItem.DefinitionType.Contains ("BOOLEAN"))
                    {
                        parameterItem.Value = (new DataTable ()).Compute (parameterItem.Value, null).ToString ();
                    }
                }

            }
        }
        static JObject ReflectJObject = JObject.Parse (File.ReadAllText ((FileSysHelper.GetCurrentAppLocationPath () + @"\Resources\ArxmlFormaterResources\ECUcDefaultConfigure.json")));
        public static string DefaultParametersParser (ElementBase parameterContainer, ISupportParameterElement parameter, string template)
        {
            string matchPattern = @"\{___ARXML__\.([\S]*?)\}";
            foreach (Match match in Regex.Matches (template, matchPattern))
            {
                string refTarget = match.Groups[1].Value;
                string value = parameterContainer.GetType ().GetProperty (ReflectJObject.Value<string> ("refTarget")).GetValue (parameterContainer) as string;
                template.Replace (match.Value, value);
            }
            return template;
        }
        public static IList<ElementBase> SearchingElementsByConfigure (JObject configure, IList<ElementBase> elementList)
        {
            IEnumerable<ElementBase> result = elementList;
            if (configure.ContainsKey ("SearchingElement"))
            {
                result = SeachingByConfigureDetail (configure.Value<JObject> ("SearchingElement"), result);
            }
            if (configure.ContainsKey ("SearchingContainer"))
            {
                result = result.Where (ele => (ele as IHasContainersElement).Containers != null).SelectMany (ele => (ele as IHasContainersElement).Containers);
                result = SeachingByConfigureDetail (configure.Value<JObject> ("SearchingContainer"), result);
            }
            if (configure.ContainsKey ("SearchingSubContainer"))
            {
                result = result.Where (ele => (ele as IHasSubContainers).SubContainers != null).SelectMany (ele => (ele as IHasSubContainers).SubContainers);
                result = SeachingByConfigureDetail (configure.Value<JObject> ("SearchingSubContainer"), result);
            }
            return result.ToList ();
        }
        static IEnumerable<ElementBase> SeachingByConfigureDetail (JObject configureDetail, IEnumerable<ElementBase> elements)
        {
            if (configureDetail.ContainsKey ("SHORT-NAME"))
            {
                elements = elements.Where (ele => Regex.IsMatch (ele.ElementName?? "", configureDetail.Value<string> ("SHORT-NAME"), RegexOptions.Multiline));
            }
            if (configureDetail.ContainsKey ("DEFINITION-REF"))
            {
                elements = elements.Where (ele => Regex.IsMatch ((ele as ISupportDefinitionRefElement).DefinitionRef?? "", configureDetail.Value<string> ("DEFINITION-REF"), RegexOptions.Multiline));
            }
            return elements;
        }
    }
}