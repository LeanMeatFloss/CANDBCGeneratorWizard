namespace ArxmlFormater.ElementHelper
{
    public interface ISupportDefinitionRefElement : ISupportElement
    {
        string DefinitionRef { get; set; }
        string DefinitionType { get; set; }
    }
}