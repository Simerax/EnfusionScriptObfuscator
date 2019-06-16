using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using EnforceScript;

namespace EnforceScriptTests
{
    class TokenizerTests
    {

        public List<Token> MakeEmptyTokenList()
        {
            return new List<Token>();
        }

        [Test]
        public void ParseSingleCommand()
        {
            string input = "private";
            var expected = MakeEmptyTokenList();
            expected.Add(new Token { keyword = Keyword.@private });

            var result = Tokenizer.Tokenize("private");

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Parse_VariableDefinition()
        {
            string input = "static const string ModName;";
            var expected = MakeEmptyTokenList();
            expected.Add(new Token { keyword = Keyword.@static });
            expected.Add(new Token { keyword = Keyword.@const });
            expected.Add(new Token { keyword = Keyword.@string });
            expected.Add(new Token { keyword = Keyword.symbol_variable, value = "ModName" });

            var actual = Tokenizer.Tokenize(input);

            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void AssignVariableToVariable()
        {
            string input = "static const string ModName = x";
            var expected = MakeEmptyTokenList();
            expected.Add(new Token { keyword = Keyword.@static });
            expected.Add(new Token { keyword = Keyword.@const });
            expected.Add(new Token { keyword = Keyword.@string });
            expected.Add(new Token { keyword = Keyword.symbol_variable, value = "ModName" });
            expected.Add(new Token { keyword = Keyword.@operator, value = "=" });
            expected.Add(new Token { keyword = Keyword.symbol_variable, value = "x" });

            var actual = Tokenizer.Tokenize(input);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AssignFunctionReturnValueToVariable()
        {
            string input = "static const string ModName = x()";
            var expected = MakeEmptyTokenList();
            expected.Add(new Token { keyword = Keyword.@static });
            expected.Add(new Token { keyword = Keyword.@const });
            expected.Add(new Token { keyword = Keyword.@string });
            expected.Add(new Token { keyword = Keyword.symbol_variable, value = "ModName" });
            expected.Add(new Token { keyword = Keyword.@operator, value = "=" });
            expected.Add(new Token { keyword = Keyword.symbol_function, value = "x" });

            var actual = Tokenizer.Tokenize(input);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Parse_FunctionCall()
        {
            string input = "m_nodes.Count()";
            var expected = MakeEmptyTokenList();
            expected.Add(new Token { keyword = Keyword.symbol_variable, value = "m_nodes"});
            expected.Add(new Token { keyword = Keyword.symbol_function, value = "Count" });

            var actual = Tokenizer.ParseFunctionCall(input);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Parse_ChainedFunctionCall()
        {
            string input = "m_nodes.Count().Nice()";
            var expected = MakeEmptyTokenList();
            expected.Add(new Token { keyword = Keyword.symbol_variable, value = "m_nodes" });
            expected.Add(new Token { keyword = Keyword.symbol_function, value = "Count" });
            expected.Add(new Token { keyword = Keyword.symbol_function, value = "Nice" });

            var actual = Tokenizer.ParseFunctionCall(input);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Parse_ChainedFunctionCallWithMemberAccess()
        {
            string input = "m_nodes.Count().Nice().member_var";
            var expected = MakeEmptyTokenList();
            expected.Add(new Token { keyword = Keyword.symbol_variable, value = "m_nodes" });
            expected.Add(new Token { keyword = Keyword.symbol_function, value = "Count" });
            expected.Add(new Token { keyword = Keyword.symbol_function, value = "Nice" });
            expected.Add(new Token { keyword = Keyword.symbol_variable, value = "member_var" });

            var actual = Tokenizer.ParseFunctionCall(input);

            Assert.AreEqual(expected, actual);
        }


        /*Reworking the Tokenizer.. these tests will eventual be reused*/
        /*

        [Test]
        public void Parse_FunctionCallWithoutParameters()
        {
            string input = @"m_nodes.Count()";

            var expected = MakeEmptyTokenList();
            expected.Add(new Token { keyword = Keyword.symbol, value = "m_nodes" });
            expected.Add(new Token { keyword = Keyword.symbol, value = "Count" });

            var actual = Tokenizer.Tokenize(input);

            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void Parse_BuiltinArrayAndVectorType()
        {
            string input = @"ref array<ref Vector2> m_nodes;";

            var expected = MakeEmptyTokenList();
            expected.Add(new Token { keyword = Keyword.@ref });
            expected.Add(new Token { keyword = Keyword.@array });
            expected.Add(new Token { keyword = Keyword.@ref });
            expected.Add(new Token { keyword = Keyword.Vector2 });
            expected.Add(new Token { keyword = Keyword.symbol, value = "m_nodes" });

            var actual = Tokenizer.Tokenize(input);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Parse_ComplexStructure01()
        {
            string input = @"
class Area2d
{
	float m_nodes;
	
	void Area2d(float nodes)
	{
		m_nodes = nodes;
	}
}
            ";
            var expected = MakeEmptyTokenList();
            expected.Add(new Token { keyword = Keyword.@class });
            expected.Add(new Token { keyword = Keyword.symbol, value = "Area2d" });
            expected.Add(new Token { keyword = Keyword.@float });
            expected.Add(new Token { keyword = Keyword.symbol, value = "m_nodes" });
            expected.Add(new Token { keyword = Keyword.@void });
            expected.Add(new Token { keyword = Keyword.@symbol, value = "Area2d" });
            expected.Add(new Token { keyword = Keyword.@float });
            expected.Add(new Token { keyword = Keyword.@symbol, value = "nodes" });
            expected.Add(new Token { keyword = Keyword.symbol, value = "m_nodes" });
            expected.Add(new Token { keyword = Keyword.@operator, value = "=" });
            expected.Add(new Token { keyword = Keyword.symbol, value = "nodes" });


            var actual = Tokenizer.Tokenize(input);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Parse_ComplexStructure02()
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

            var result = Tokenizer.Tokenize(input);

            Assert.AreEqual(expected, result);
        }
        */
    }
}
