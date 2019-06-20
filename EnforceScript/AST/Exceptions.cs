using System;
using System.Collections.Generic;
using System.Text;

namespace EnforceScript.AST
{
    public class UnexpectedSymbolException : Exception
    {
        public UnexpectedSymbolException(string message)
            : base(message)
        {

        }

        public UnexpectedSymbolException(Word word)
            : base($"Unexpected Symbol {word.value} at {word.location.line}:{word.location.column}")
        {
        }
    }
}
