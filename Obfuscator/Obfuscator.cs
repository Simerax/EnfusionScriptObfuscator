using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Obfuscator
{
    public struct SymbolName
    {
        public string old;
        public string @new;
    }
    public class Obfuscator
    {
        public static readonly char[] allowed_chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_".ToCharArray();
        public static void Obfuscate(string[] paths)
        {
        }

        public static string GenerateRandomName(int length = 25)
        {
            var random_name = "";
            var rand = new Random();
            for (int i = 0; i < length; i++)
                random_name += allowed_chars[rand.Next(0, allowed_chars.Length - 1)];
            return random_name;
        }
    }
}
