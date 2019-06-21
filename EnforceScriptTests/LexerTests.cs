using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using EnforceScript;

namespace EnforceScriptTests
{
    class LexerTests
    {

        public List<Word> MakeEmptySymbolList()
        {
            return new List<Word>();
        }

        public Word MakeWord(string value, uint line, uint column)
        {
            return new Word { value = value, location = new Location(line, column) };
        }


        [Test]
        public void SymbolFromWord()
        {
            string input = "private;";
            var expected = MakeEmptySymbolList();
            expected.Add(MakeWord("private", 1, 1));
            expected.Add(MakeWord(";", 1, 8));

            var result = Lexer.Lex(input);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void StringLiteral()
        {
            string input = "private x = \"lol this is a string 123\";";
            var expected = MakeEmptySymbolList();
            expected.Add(MakeWord("private", 1, 1));
            expected.Add(MakeWord("x", 1, 9));
            expected.Add(MakeWord("=", 1, 11));
            expected.Add(MakeWord("\"lol this is a string 123\"", 1, 13));
            expected.Add(MakeWord(";", 1, 39));

            var result = Lexer.Lex(input);

            Assert.AreEqual(expected, result);
        }


        [Test]
        public void IgnoreOneLineComments()
        {
            string input = "class Test\n{\n// This is a comment and class is not interpreted\nvoid Test()\n{\n}\n}";

            var expected = MakeEmptySymbolList();
            expected.Add(new Word("class", 1, 1));
            expected.Add(new Word("Test", 1, 7));
            expected.Add(new Word("{", 2, 1));
            expected.Add(new Word("void", 4, 1));
            expected.Add(new Word("Test", 4, 6));
            expected.Add(new Word("(", 4, 10));
            expected.Add(new Word(")", 4, 11));
            expected.Add(new Word("{", 5, 1));
            expected.Add(new Word("}", 6, 1));
            expected.Add(new Word("}", 7, 1));

            var result = Lexer.Lex(input);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void IgnoreMultiLineComments()
        {
            string input = "class Test\n{\n    /*\n    This is a comment and class is not interpreted\n*/\nvoid Test()\n{\n}\n}";

            var expected = MakeEmptySymbolList();
            expected.Add(new Word("class", 1, 1));
            expected.Add(new Word("Test", 1, 7));
            expected.Add(new Word("{", 2, 1));
            expected.Add(new Word("void", 6, 1));
            expected.Add(new Word("Test", 6, 6));
            expected.Add(new Word("(", 6, 10));
            expected.Add(new Word(")", 6, 11));
            expected.Add(new Word("{", 7, 1));
            expected.Add(new Word("}", 8, 1));
            expected.Add(new Word("}", 9, 1));

            var result = Lexer.Lex(input);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ParseSymbols_ClassDefinition()
        {
            string input = @"class BlurFade extends TimerBase
{
protected bool m_fade_out;

void BlurFade(float x)
{
OnInit(x);
}
}
            ";
            var expected = MakeEmptySymbolList();
            expected.Add(new Word("class", 1, 1));
            expected.Add(new Word("BlurFade", 1, 7));
            expected.Add(new Word("extends", 1, 16));
            expected.Add(new Word("TimerBase", 1, 24));
            expected.Add(new Word("{", 2, 1));
            expected.Add(new Word("protected", 3, 1));
            expected.Add(new Word("bool", 3, 11));
            expected.Add(new Word("m_fade_out", 3, 16));
            expected.Add(new Word(";", 3, 26));
            expected.Add(new Word("void", 5, 1));
            expected.Add(new Word("BlurFade", 5, 6));
            expected.Add(new Word("(", 5, 14));
            expected.Add(new Word("float", 5, 15));
            expected.Add(new Word("x", 5, 21));
            expected.Add(new Word(")", 5, 22));
            expected.Add(new Word("{", 6, 1));
            expected.Add(new Word("OnInit", 7, 1));
            expected.Add(new Word("(", 7, 7));
            expected.Add(new Word("x", 7, 8));
            expected.Add(new Word(")", 7, 9));
            expected.Add(new Word(";", 7, 10));
            expected.Add(new Word("}", 8, 1));
            expected.Add(new Word("}", 9, 1));

            var result = Lexer.Lex(input);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ParseSymbols_VariableMemberAccess()
        {
            string input = "node._y;";
            var expected = MakeEmptySymbolList();
            expected.Add(new Word("node", 1, 1));
            expected.Add(new Word(".", 1, 5));
            expected.Add(new Word("_y", 1, 6));
            expected.Add(new Word(";", 1, 8));

            var result = Lexer.Lex(input);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ParseSymbols_VariableDefinition_01()
        {
            string input = "static const string node;";
            var expected = MakeEmptySymbolList();
            expected.Add(new Word("static", 1, 1));
            expected.Add(new Word("const", 1, 8));
            expected.Add(new Word("string", 1, 14));
            expected.Add(new Word("node", 1, 21));
            expected.Add(new Word(";", 1, 25));


            var result = Lexer.Lex(input);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ParseSymbols_VariableDefinition_02()
        {
            string input = "static const string ModName = x;";
            var expected = MakeEmptySymbolList();

            expected.Add(new Word("static", 1, 1));
            expected.Add(new Word("const", 1, 8));
            expected.Add(new Word("string", 1, 14));
            expected.Add(new Word("ModName", 1, 21));
            expected.Add(new Word("=", 1, 29));
            expected.Add(new Word("x", 1, 31));
            expected.Add(new Word(";", 1, 32));

            var actual = Lexer.Lex(input);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseSymbols_FunctionCall_01()
        {
            string input = "nodes.add(x);";
            var expected = MakeEmptySymbolList();

            expected.Add(new Word("nodes", 1, 1));
            expected.Add(new Word(".", 1, 6));
            expected.Add(new Word("add", 1, 7));
            expected.Add(new Word("(", 1, 10));
            expected.Add(new Word("x", 1, 11));
            expected.Add(new Word(")", 1, 12));
            expected.Add(new Word(";", 1, 13));


            var result = Lexer.Lex(input);

            Assert.AreEqual(expected, result);
        }



        /*
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
            string input = "static const string ModName = x;";
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
        public void Parse_FunctionCallWithParameters()
        {
            string input = "m_nodes.find(x)";
            var expected = MakeEmptyTokenList();
            expected.Add(new Token { keyword = Keyword.symbol_variable, value = "m_nodes" });
            expected.Add(new Token { keyword = Keyword.symbol_function, value = "find" });
            expected.Add(new Token { keyword = Keyword.symbol_variable, value = "x" });

            var actual = Tokenizer.Tokenize(input);

            Assert.AreEqual(expected, actual);
        }

        /*
        [Test]
        public void Parse_FunctionCallWithFunctionAsParameter()
        {
            string input = "m_nodes.find(x())";
            var expected = MakeEmptyTokenList();
            expected.Add(new Token { keyword = Keyword.symbol_variable, value = "m_nodes" });
            expected.Add(new Token { keyword = Keyword.symbol_function, value = "find" });
            expected.Add(new Token { keyword = Keyword.symbol_function, value = "x" });

            var actual = Tokenizer.Tokenize(input);

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

        [Test]
        public void Parse_ClassDefinition_01()
        {
            string input = @"
                class BlurFade extends TimerBase { }
            ";
            var expected = MakeEmptyTokenList();
            expected.Add(new Token { keyword = Keyword.@class });
            expected.Add(new Token { keyword = Keyword.symbol_class, value = "BlurFade" });
            expected.Add(new Token { keyword = Keyword.extends });
            expected.Add(new Token { keyword = Keyword.symbol_class, value = "TimerBase" });

            var result = Tokenizer.Tokenize(input);

            Assert.AreEqual(expected, result);
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
            expected.Add(new Token { keyword = Keyword.symbol_class, value = "array" });
            expected.Add(new Token { keyword = Keyword.@ref });
            expected.Add(new Token { keyword = Keyword.symbol_class, value = "Vector2" });
            expected.Add(new Token { keyword = Keyword.symbol_variable, value = "m_nodes" });

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
            expected.Add(new Token { keyword = Keyword.symbol_class, value = "Area2d" });
            expected.Add(new Token { keyword = Keyword.@float });
            expected.Add(new Token { keyword = Keyword.symbol_variable, value = "m_nodes" });
            expected.Add(new Token { keyword = Keyword.@void });
            expected.Add(new Token { keyword = Keyword.@symbol_function, value = "Area2d" });
            expected.Add(new Token { keyword = Keyword.@float });
            expected.Add(new Token { keyword = Keyword.@symbol_variable, value = "nodes" });
            expected.Add(new Token { keyword = Keyword.symbol_variable, value = "m_nodes" });
            expected.Add(new Token { keyword = Keyword.@operator, value = "=" });
            expected.Add(new Token { keyword = Keyword.symbol_variable, value = "nodes" });


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
            expected.Add(new Token { keyword = Keyword.symbol_class, value = "BlurFade" });
            expected.Add(new Token { keyword = Keyword.extends });
            expected.Add(new Token { keyword = Keyword.symbol_class, value = "TimerBase" });
            expected.Add(new Token { keyword = Keyword.@protected });
            expected.Add(new Token { keyword = Keyword.@bool });
            expected.Add(new Token { keyword = Keyword.symbol_variable, value = "m_fade_out" });
            expected.Add(new Token { keyword = Keyword.@void });
            expected.Add(new Token { keyword = Keyword.symbol_function, value = "BlurFade" });
            expected.Add(new Token { keyword = Keyword.symbol_function, value = "OnInit" });

            var result = Tokenizer.Tokenize(input);

            Assert.AreEqual(expected, result);
        }
        */

    }
}
