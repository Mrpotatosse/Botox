using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Botox.Extension
{
    public static class ByteArrayExtension
    {
        public static string ToHexString(this byte[] byteArray)
        {
            return BitConverter.ToString(byteArray).Replace("-", " ");
        }        

        public static bool Same(this byte[] array, byte[] array2, out int first_diff, bool same_len = false)
        {            
            if (array.Length > array2.Length) return Same(array2, array, out first_diff, same_len);

            first_diff = -1;
            if (array.Length != array2.Length && same_len) return false;
            
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != array2[i])
                {
                    first_diff = i;
                    return false;
                }
            }
            return true;
        }

        public static byte[] Hash(this byte[] array)
        {
            using(SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(array);
            }
        }
    }
}
