using System;
using System.Collections.Generic;
using System.Text;

namespace EnforceScript
{
    public enum Keyword
    {
        // Function/Method modifiers
        // some of them are shared with Variable modifiers
        @private,
        @protected,
        @static,
        @override,
        @proto,
        @native,

        // Variable modifiers
        @autoptr,
        @ref,
        @const,
        @out,
        @inout,

        // Class modifiers
        @modded,


        // Other Keywords
        @new,
        @delete,
        @class,
        @extends,
        @typedef,
        @return,
        @null,
        @this,
        @super,

        //Types
        @int,
        @float,
        @bool,
        @string,
        @vector,
        @void,
        @typename,
        @auto,

        //Control Flow
        @if,
        @else,
        @while,
        @switch,
        @break,
        @case,
        @for,
        @foreach,

        @symbol, // user defined symbol
        @operator,

        @string_literal,

        @unknown,
    }

    public struct Token
    {
        public Keyword keyword;
        public string value;
    }
    public class Tokenizer
    {
        public static readonly string[] operators = {
            "+",
            "-",
            "*",
            "/",
            "%",
            "=",
            "+=",
            "-=",
            "*=",
            "/=",
            "++",
            "--",
            ">",
            "<",
            ">=",
            "<=",
            "==",
            "!=",
            "&&",
            "||",
            "&",
            "|",
            "~",
            "<<",
            ">>",
            "[]",
            "!",
        };

        public readonly string[] delimiter =
        {
            "\r",
            "\n",
            ";",
            // yes parenthesis aren't actually delimiter but for our usecase we dont really care about them
            // at least yet...
            "(",
            ")",
            "{",
            "}",
        };

        public List<Token> Tokenize(string content)
        {
            var tokens = new List<Token>();

            content = content.Replace("\n", " ");

            foreach(var raw_word in content.Split(' '))
            {
                string word = raw_word;
                    foreach(var delim in delimiter)
                            word = word.Replace(delim, "");
                if (word == "")
                    continue;

                var token = new Token { keyword = Keyword.unknown};


                // Is the word a keyword?
                Keyword k;
                if (Enum.TryParse(word, out k))
                {
                    token.keyword = k;
                }
                else
                {
                    // is the word an operator?
                    foreach (var op in operators)
                    {
                        if (word == op)
                        {
                            token.keyword = Keyword.@operator;
                            token.value = op;
                        }
                    }

                    // otherwise it should be a custom symbol
                    if (token.keyword == Keyword.unknown)
                    {
                        token.keyword = Keyword.symbol;
                        token.value = word;
                    }

                }

                if (token.keyword == Keyword.unknown)
                    // TODO: this should probably be something like a "unknown token exception"
                    throw new Exception("Unkown keyword '" + word + "'");
                else
                    tokens.Add(token);
            }

            return tokens;
        }
    }
}
