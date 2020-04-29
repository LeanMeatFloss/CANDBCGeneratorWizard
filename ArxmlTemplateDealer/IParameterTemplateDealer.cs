using ArxmlFormater.ElementHelper;
using Newtonsoft.Json.Linq;
namespace ArxmlTemplateDealer
{
    public delegate string IParameterTemplateDealer (ISupportParameterElement element, JObject template);

}