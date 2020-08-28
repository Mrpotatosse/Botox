using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botox.Extension
{
    public static class StringExtension
    {
        public static string RandomString(this string from, string random_char)
        {
            int len = from.Length;

            Random random = new Random();
            string result = "";
            for(int i = 0; i < len; i++)
            {
                result += random_char[random.Next(0, random_char.Length)];
            }

            return result;
        }
    }
}
