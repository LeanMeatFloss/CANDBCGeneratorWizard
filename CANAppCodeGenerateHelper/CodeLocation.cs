using System.Text;
namespace CANAppCodeGenerateHelper
{
    public static class CodeLocation
    {
        public static long IndexOf (this StringBuilder stringBuilder, string value)
        {
            stringBuilder.ToString ().IndexOf (value);
        }
        /// <summary>
        /// Searching the location of function definitions according to the function name
        /// </summary>
        /// <param name="codeContent"></param>
        /// <param name="FunctionName"></param>
        public void SearchForFunctionDefinition (StringBuilder codeContent, string functionName)
        {
            string codeContentString = codeContent.ToString ();
            //the following chars can be follow with the function name,
            //e.g. void functionName(void) is avaliable
            //e.g. void functionName(input); is not avaliable
            char[] avaliableUnASCIICharArray = new char[]
            {
                '(',
                '_',
                ')',
                '.',
                '{'
            };
            int functionLocation = 0;
            int function
            for (int i = 0;; i++)
            {
                codeContentString
            }
        }
    }
}