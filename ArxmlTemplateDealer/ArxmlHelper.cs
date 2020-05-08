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

        public static IList<ElementBase> SearchingElementsDefinitionByPath (IEnumerable<ElementBase> elementsList, string path)
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
        public delegate void AddElementDelegate (params ElementBase[] elements);
        static JObject ElementTypeJDict = JObject.Parse (File.ReadAllText ((FileSysHelper.GetCurrentAppLocationPath () + @"\Resources\ArxmlTemplateDealerResources\ElementType.json")));
        public static void IHasContainersTemplateDealer (ElementBase container, IEnumerable<ElementBase> containerElements, IEnumerable<ElementBase> elementsCollection, JObject template, AddElementDelegate addElementFunc, Func<ElementBase, string, IEnumerable<string>> nameListFunc)
        {
            //Searching elements that fixed the template
            List<string> nameList = nameListFunc (container, template.Value<string> ("ElementName")).ToList ();
            //Searching elements that contains the definitions
            var elementsDefinitionCollection = SearchingElementsDefinitionByPath (elementsCollection, (container as ISupportDefinitionRefElement).DefinitionRef);

            ElementBase elementDefinition = elementsDefinitionCollection.Where (ele => Regex.IsMatch (ele.Path + "\\" + ele.ElementName, template.Value<string> ("DEFINITION"), RegexOptions.Multiline)).FirstOrDefault ();
            string elementType = elementDefinition.ElementType;
            foreach (var subJToken in ElementTypeJDict)
            {
                if ((subJToken.Value as JArray).Contains (elementType))
                {
                    elementType = subJToken.Key;
                    break;
                }
            }
            foreach (var name in nameList)
            {
                var containerElement = containerElements.Where (ele => ele.ElementName.Equals (name)).FirstOrDefault ();
                if (containerElement == null)
                {
                    //try to add an element

                    var ele = container.NewElement (elementType);
                    var elementBase = new ElementBase () { ArxmlElement = ele };
                    (elementBase as ISupportDefinitionRefElement).DefinitionRef = elementDefinition.Path + "\\" + elementDefinition.ElementName;
                    (elementBase as ISupportDefinitionRefElement).DefinitionType = elementDefinition.ElementType;
                    addElementFunc (elementBase);
                    containerElement = containerElements.Where (ele => ele.ElementName.Equals (name)).FirstOrDefault ();
                }
                else
                {
                    (containerElement as ISupportDefinitionRefElement).DefinitionRef = elementDefinition.Path + "\\" + elementDefinition.ElementName;
                    (containerElement as ISupportDefinitionRefElement).DefinitionType = elementDefinition.ElementType;
                }
            }

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
                        var parametersDefinitionCollection = SearchingElementsDefinitionByPath (elementsCollection, path);
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
                                definitionType = Regex.Replace (definitionType, "(-ENUMERATION-PARAM-|-FUNCTION-NAME-)", "-TEXTUAL-PARAM-");
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
                    if (string.IsNullOrEmpty (parameterItem.Value)) { element.RemoveParameters (parameterItem); }

                }
            }
        }
        static JObject ReflectJObject = JObject.Parse (File.ReadAllText ((FileSysHelper.GetCurrentAppLocationPath () + @"\Resources\ArxmlTemplateDealerResources\Reflect.json")));
        public static string DefaultParametersParser (ElementBase parameterContainer, ISupportParameterElement parameter, string template)
        {
            string matchPattern = @"\{__ARXML__\.([\S]*?)\}";
            foreach (Match match in Regex.Matches (template, matchPattern))
            {
                string refTarget = match.Groups[1].Value;
                string value = parameterContainer.GetType ().GetProperty (ReflectJObject.Value<string> (refTarget)).GetValue (parameterContainer) as string;
                template = template.Replace (match.Value, value);
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
            if (configure.ContainsKey ("SearchingSubContainerRoute"))
            {
                foreach (var subContainerSearchTemplate in configure.Value<JArray> ("SearchingSubContainerRoute"))
                {
                    result = result.Where (ele => (ele as IHasSubContainers).SubContainers != null).SelectMany (ele => (ele as IHasSubContainers).SubContainers);
                    result = SeachingByConfigureDetail (subContainerSearchTemplate as JObject, result);
                }
            }
            return result.ToList ();
        }
        static IEnumerable<ElementBase> SeachingByConfigureDetail (JObject configureDetail, IEnumerable<ElementBase> elements)
        {
            if (configureDetail.ContainsKey ("SHORT-NAME"))
            {
                elements = elements.Where (ele => Regex.IsMatch (ele.ElementName?? "", configureDetail.Value<string> ("SHORT-NAME"), RegexOptions.Multiline));
            }
            //searching in two ways, definition and definition ref
            if (configureDetail.ContainsKey ("DEFINITION"))
            {
                //var definitions = (elements.Where (ele => Regex.IsMatch (ele.Path, configureDetail.Value<string> ("DEFINITION"), RegexOptions.Multiline)));
                elements = elements.Where (ele => Regex.IsMatch ((ele as ISupportDefinitionRefElement).DefinitionRef?? "", configureDetail.Value<string> ("DEFINITION"), RegexOptions.Multiline));
            }
            return elements;
        }
    }
}