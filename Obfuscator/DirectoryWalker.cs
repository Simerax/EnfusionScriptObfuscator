using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace Obfuscator
{
    class DirectoryWalker
    {
        
        public static void WalkRecursive(string root_path, Action<string> OnFile)
        {
            FileAttributes attr = File.GetAttributes(root_path);
            if ((attr & FileAttributes.Directory) != FileAttributes.Directory)
                throw new SystemException("Path is not a directory!");

            Stack<string> queue = new Stack<string>();
            queue.Push(root_path);

            while (queue.Count > 0)
            {
                var path = queue.Pop();

                foreach (var dir in Directory.GetDirectories(path))
                    queue.Push(dir);

                foreach (var file in Directory.GetFiles(path))
                    OnFile.Invoke(file);
            }
        }
    }
}
