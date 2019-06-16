using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using EnforceScript;

namespace EnforceScriptTests
{
    class TokenizerTests
    {
        public Tokenizer MakeTokenizer()
        {
            return new Tokenizer();
        }

        public List<Token> MakeEmptyTokenList()
        {
            return new List<Token>();
        }

        [Test]
        public void ParseSingleCommand_Success()
        {
            string input = "private";
            var t = MakeTokenizer();
            var expected = MakeEmptyTokenList();
            expected.Add(new Token { keyword = Keyword.@private });

            var result = t.Tokenize("private");

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Parse_VariableDefinition()
        {
            string input = "static const string ModName;";
            var t = MakeTokenizer();
            var expected = MakeEmptyTokenList();
            expected.Add(new Token { keyword = Keyword.@static });
            expected.Add(new Token { keyword = Keyword.@const });
            expected.Add(new Token { keyword = Keyword.@string });
            expected.Add(new Token { keyword = Keyword.symbol, value = "ModName" });
            /*
            expected.Add(new Token { keyword = Keyword.@operator });
            expected.Add(new Token { keyword = Keyword.@string_literal, value = "Test"});
            */

            var result = t.Tokenize(input);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Parse_ClassDefinition()
        {
            string input = @"
                class BlurFade extends TimerBase
                {
                    protected bool m_fade_out;

                    void BlurFade()
                    {
                        OnInit();
                    }
                }
            ";
            var t = MakeTokenizer();
            var expected = MakeEmptyTokenList();
            expected.Add(new Token { keyword = Keyword.@class });
            expected.Add(new Token { keyword = Keyword.symbol, value = "BlurFade" });
            expected.Add(new Token { keyword = Keyword.extends });
            expected.Add(new Token { keyword = Keyword.symbol, value = "TimerBase" });
            expected.Add(new Token { keyword = Keyword.@protected });
            expected.Add(new Token { keyword = Keyword.@bool });
            expected.Add(new Token { keyword = Keyword.symbol, value = "m_fade_out" });
            expected.Add(new Token { keyword = Keyword.@void });
            expected.Add(new Token { keyword = Keyword.symbol, value = "BlurFade" });
            expected.Add(new Token { keyword = Keyword.symbol, value = "OnInit" });

            var result = t.Tokenize(input);

            Assert.AreEqual(expected, result);
        }
    }
}
