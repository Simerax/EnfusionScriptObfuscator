using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Obfuscator
{
    public struct SymbolName
    {
        public string Old;
        public string New;
    }
    public class Obfuscator
    {
        public static readonly char[] allowed_chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_".ToCharArray();
        public static void Obfuscate(string[] paths)
        {
            /*
            List<SymbolName> custom_symbols = new List<SymbolName>();
            foreach(var path in paths)
            {
                
                string content = File.ReadAllText(path);
                var tokens = EnforceScript.Tokenizer.Tokenize(content);
                foreach(var token in tokens)
                {
                    if (token.keyword == EnforceScript.Keyword.symbol)
                    {
                        bool symbol_already_obfuscated = false;
                        foreach (var symbol in custom_symbols)
                        {
                            if (symbol.Old == token.value)
                            {
                                symbol_already_obfuscated = true;
                                break;
                            }
                        }

                        if (!symbol_already_obfuscated)
                            // need to add a check that a random name isnt used twice
                            custom_symbols.Add(new SymbolName { Old = token.value, New = GenerateRandomName() });
                    }

                }
                

                string obfuscated = content;
                foreach (var symbol in custom_symbols)
                    obfuscated = obfuscated.Replace(symbol.Old, symbol.New);

                File.WriteAllText(path, obfuscated);
            }
            */
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
