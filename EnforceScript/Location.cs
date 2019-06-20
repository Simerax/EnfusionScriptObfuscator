using System;
using System.Collections.Generic;
using System.Text;

namespace EnforceScript
{
    public struct Location
    {
        public uint line;
        public uint column;

        public Location(uint line, uint column)
        {
            this.line = line;
            this.column = column;
        }
    }
}
