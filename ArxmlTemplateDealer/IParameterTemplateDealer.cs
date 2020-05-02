using ArxmlFormater.ElementHelper;
using Newtonsoft.Json.Linq;
namespace ArxmlTemplateDealer
{
    public delegate string IParameterTemplateDealer (ElementBase parametersContainer, ISupportParameterElement parameter, JToken template);
}