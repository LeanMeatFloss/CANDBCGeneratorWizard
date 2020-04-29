using System.IO;
using NetCoreSystemEnvHelper;
using Newtonsoft.Json.Linq;
namespace ArxmlFormater.ECUc
{
    public static class ECUcConfigureDict
    {
        static JObject Configure { get; set; }
        public static void LoadConfigure ()
        {
            string configureTest = File.ReadAllText (FileSysHelper.GetCurrentAppLocationPath () + @"\Resources\ArxmlFormaterResources\ECUcDefaultConfigure.json");
            Configure = JObject.Parse (configureTest);
        }
    }
}