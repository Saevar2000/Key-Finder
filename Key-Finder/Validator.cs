using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Key_Finder
{
    class Validator
    {
        public bool ValidatePrivateKey(string privateKey)
        {
            if (Regex.IsMatch(privateKey, "5[HJK][1-9A-HJ-NP-Za-km-z]{49}"))
            {
                Console.WriteLine("private key is uncompressed");
                return true;
            }
            else if (Regex.IsMatch(privateKey, "[KL][1-9A-HJ-NP-Za-km-z]{51}"))
            {
                Console.WriteLine("private key is compressed");
                return true;
            }
            else
            {
                Console.WriteLine("Invalid private key");
                return false;
            }
        }

        public bool ValidateBitcoinAddress(string address)
        {
            if (address.Length < 26 || address.Length > 35)
            {
                Console.WriteLine("wrong length");
                return false;
            }
            var decoded = DecodeBase58(address);
            var d1 = Hash(decoded.SubArray(0, 21));
            var d2 = Hash(d1);
            if (!decoded.SubArray(21, 4).SequenceEqual(d2.SubArray(0, 4)))
            {
                Console.WriteLine("bad digest");
                return false;
            }
            return true;
        }

        const string Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        const int Size = 25;

        private byte[] DecodeBase58(string input)
        {
            var output = new byte[Size];
            foreach (var t in input)
            {
                var p = Alphabet.IndexOf(t);
                if (p == -1) Console.WriteLine("invalid character found");
                var j = Size;
                while (--j > 0)
                {
                    p += 58 * output[j];
                    output[j] = (byte)(p % 256);
                    p /= 256;
                }
                if (p != 0) Console.WriteLine("address too long");
            }
            return output;
        }

        private byte[] Hash(byte[] bytes)
        {
            var hasher = new SHA256Managed();
            return hasher.ComputeHash(bytes);
        }
    }

    public static class ArrayExtensions
    {
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            var result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
