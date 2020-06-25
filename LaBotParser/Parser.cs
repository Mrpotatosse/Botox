using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaBotParser
{
    public class Parser
    {
        private static readonly string LABO_WEB_LINK = @"https://louisabraham.github.io/LaBot/decoder.html?hex=";

        public static string FromWeb(byte[] data)
        {
            string link = LABO_WEB_LINK + ToHexString(data);

            ProcessStartInfo startInfo = new ProcessStartInfo(link);
            Process.Start(startInfo);

            return "";
        }

        private static string ToHexString(byte[] byteArray)
        {
            return BitConverter.ToString(byteArray).Replace("-", "");
        }
    }
}
