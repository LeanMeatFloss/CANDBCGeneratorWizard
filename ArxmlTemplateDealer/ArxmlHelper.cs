using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ArxmlFormater.AR_PackageHelper;
using ArxmlFormater.ElementHelper;
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

                        return;
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
                            if ((container as IHasParameters).Parameters == null)
                            {
                                continue;
                            }
                            foreach (var parameter in (container as IHasParameters).Parameters)
                            {
                                //parameter checking,
                                if (Regex.IsMatch (parameter.Path, templateItem.Key, RegexOptions.Multiline))
                                {
                                    //find it, try to add it!
                                    //first get type name
                                    string typeName = parameter.ElementType;
                                    var ele = element.NewElement (typeName);
                                    string definitionType = Regex.Replace (typeName, "(-INTEGER-|-FLOAT-)", "-NUMERICAL-");
                                    definitionType = Regex.Replace (definitionType, "(-ENUMERATION-|-FUNCTION-)", "-TEXTUAL-");
                                    definitionType = Regex.Replace (definitionType, "-DEF$", "-VALUE", RegexOptions.Multiline);
                                    (ele as ISupportDefinitionRefElement).ElementType = definitionType;
                                    (ele as ISupportDefinitionRefElement).DefinitionType = typeName;
                                    (ele as ISupportDefinitionRefElement).DefinitionRef = parameter.Path + @"\" + parameter.ElementName;
                                    element.AddParameters (ele as ISupportParameterElement);
                                    parameterItem = element.Parameters.Where (parameter => Regex.IsMatch (parameter.DefinitionRef, templateItem.Key, RegexOptions.Multiline)).FirstOrDefault ();
                                    break;
                                }

                            }
                        }
                    }
                    parameterTemplateDealer (element as ElementBase, parameterItem, templateItem.Value);
                }

            }
        }
        static JObject ReflectJObject = JObject.Parse (File.ReadAllText ())
        public static string DefaultParametersParser (ElementBase parameterContainer, ISupportParameterElement parameter, string template)
        {
            string matchPattern = @"\{___ARXML__\.([\S]*?)\}";
            foreach (Match match in Regex.Matches (template, matchPattern))
            {
                string refTarget = match.Groups[1].Value;
                parameterContainer.GetType ().GetProperty (refTarget)
            }
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