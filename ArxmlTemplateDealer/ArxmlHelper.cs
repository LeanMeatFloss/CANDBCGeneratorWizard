using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ArxmlFormater.ElementHelper;
using Newtonsoft.Json.Linq;
namespace ArxmlTemplateDealer
{
    public class ArxmlHelper
    {
        public static void ConfigureParametersByTemplate (IEnumerable<IHasParameters> elementsList, JObject templateConfigure, IParameterTemplateDealer parameterTemplateDealer)
        {
            foreach (var templateItem in templateConfigure)
            {
                foreach (var element in elementsList)
                {
                    var parameterItem = element.Parameters.Where (parameter => Regex.IsMatch (parameter.DefinitionRef, templateItem.Key)).FirstOrDefault ();
                    if (parameterItem == null)
                    {

                    }
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
                    result = result.SelectMany (ele => (ele as IHasContainersElement).Containers);
                    result = SeachingByConfigureDetail (configure.Value<JObject> ("SearchingContainer"), result);
                }
                if (configure.ContainsKey ("SearchingSubContainer"))
                {
                    result = result.SelectMany (ele => (ele as IHasSubContainers).SubContainers);
                    result = SeachingByConfigureDetail (configure.Value<JObject> ("SearchingSubContainer"), result);
                }
                return result.ToList ();
            }
            static IEnumerable<ElementBase> SeachingByConfigureDetail (JObject configureDetail, IEnumerable<ElementBase> elements)
            {
                if (configureDetail.ContainsKey ("SHORT-NAME"))
                {
                    elements = elements.Where (ele => Regex.IsMatch (ele.ElementName, configureDetail.Value<string> ("SHORT-NAME")));
                }
                if (configureDetail.ContainsKey ("DEFINITION-REF"))
                {
                    elements = elements.Where (ele => Regex.IsMatch ((ele as ISupportDefinitionRefElement).DefinitionRef, configureDetail.Value<string> ("DEFINITION-REF")));
                }
                return elements;
            }
        }
    }