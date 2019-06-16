using System;
using System.IO;
using System.Collections.Generic;

namespace Obfuscator
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string root_path = args[0];
                List<string> enfusion_scripts = EnfusionScriptFinder.FindScripts(root_path);

                try
                {
                    Obfuscator.Obfuscate(enfusion_scripts.ToArray());
                } catch (Exception e)
                {
                    Console.WriteLine("Could not Obfuscate Scripts. Reason: " + e.Message);
                }
            } catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
