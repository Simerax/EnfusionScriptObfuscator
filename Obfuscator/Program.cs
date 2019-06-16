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

                foreach (var script in enfusion_scripts)
                {
                    Console.WriteLine("Processing '" + script + "'");
                    try
                    {
                        Obfuscator.Obfuscate(script);
                    } catch (Exception e)
                    {
                        Console.WriteLine("Could not Obfuscate Script '" + script + "' Reason: " + e.Message);
                    }
                }


            } catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
