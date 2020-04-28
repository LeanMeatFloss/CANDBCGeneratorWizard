namespace ArxmlFormater.ComSignal
{
    public interface IParameter
    {
        ECUc.ECUcParamTypeEnum ParameterType { get; set; }
        ECUc.ECUcParamDestinationDefEnum DestinationDef { get; set; }
        string Definition { get; set; }
        string Value { get; set; }
    }
}