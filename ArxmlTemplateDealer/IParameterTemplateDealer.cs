using ArxmlFormater.ElementHelper;
using Newtonsoft.Json.Linq;
namespace ArxmlTemplateDealer
{
    public delegate void IParameterTemplateDealer (ElementBase parametersContainer, ISupportParameterElement parameter, JToken template);
}