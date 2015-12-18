using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PS.Terminal.Link.Bridge.RemoteSettleTerminalService;

namespace SamplePointOfSaleClient.Implementations
{
    public static class GlobalProperties
    {
        private static Dictionary<String, String> globalDictionary = new Dictionary<string, string>();

        public static String getValueByKey(String key)
        {
            String value;
            try
            {
                globalDictionary.TryGetValue(key, out value);
            }
            catch
            {
                value = "";
            }

            return value;
        }

        public static void setKeyAndValue(String key, String value)
        {
            globalDictionary.Add(key, value);
        }

        public static bool containsKey(String key)
        {
            return globalDictionary.ContainsKey(key);
        }

        public static int decimalToInt(decimal d) {

            return Decimal.ToInt32(Decimal.Round(Decimal.Multiply(d, 100), 0, MidpointRounding.AwayFromZero));
        }
        public static void main(String[] args)
        {

            Console.WriteLine(decimalToInt(1.22m));
        }

        public static String colsToRow(String firstString, String secondString, String thirdString, int indexOfSecondCol, int maxLenOfSecondCol, int lastIndexOfThirdCol)
        {
            String row = "";

            row += fillWithSpaces(firstString, indexOfSecondCol - 1 + maxLenOfSecondCol - secondString.Length);
            row += fillWithSpaces(secondString, lastIndexOfThirdCol - indexOfSecondCol - maxLenOfSecondCol + 1 - thirdString.Length + secondString.Length);
            row += thirdString;

            return row;
        }

        public static String fillWithSpaces(String str, int totalLen)
        {
            while (str.Length < totalLen)
            {
                str += " ";
            }

            return str;
        }

        public static String[] additionalTopInfoOfReceipt;
        public static String[] additionalBottomInfoOfReceipt;
        public static List<TCPPortConfiguration> terminalsList;
    }
}
