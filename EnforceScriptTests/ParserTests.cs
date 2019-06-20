using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using EnforceScript;

namespace EnforceScriptTests
{
    class ParserTests
    {
        [Test]
        public void Parse_01()
        {
            string content = @"
                class Timer Base
                {
                    void Test()
                    {
                    }
                }
            ";
            var input = Lexer.lex(content);

        }
    }
}
