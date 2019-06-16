using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace EnforceObfuscator
{
    class EnfusionScriptFinder
    {
        private static Regex EnforceScriptFileName = new Regex("\\.c$");
        public static List<string> FindScripts(string root_path)
        {
            var found = new List<string>();

            Action<string> finder = delegate(string file_path)
            {
                if (EnforceScriptFileName.IsMatch(file_path))
                    found.Add(file_path);
            };

            DirectoryWalker.WalkRecursive(root_path, finder);

            return found;
        }


    }
}
