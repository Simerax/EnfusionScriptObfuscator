using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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
        @array,
        @Vector2,

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
        @symbol_function,
        @symbol_variable,
        @symbol_class,
        @operator,

        @string_literal,

        @unknown,
    }

    public struct Token
    {
        public Keyword keyword;
        public string value;
    };
    public class Tokenizer
    {
        public static Regex empty_or_whitespace_only = new Regex("^\\s*$");
        public static Regex whitespace = new Regex("\\s");
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

        public static readonly string[] delimiter =
        {
            "\r",
            "\n",
            "\t",
            ";",
        };

        // There are a LOT of symbols that we dont really care about
        // Of course if this should ever be a complete tokenizer for Enfusion Script then of course we need to care about them
        // We probably need to do that at some point anyway
        public static readonly string[] ignore_chars =
        {
            "(",
            ")",
            "{",
            "}",
            ">",
            "<",
        };

        public static List<Token> Tokenize(string content)
        {
            var tokens = new List<Token>();

            content = content.Replace("\n", " ");
            bool done = false;

            var words = content.Split(' ');
            for(int index = 0; index < words.Length; index++)
            {
                var current_word = words[index];
                current_word = current_word.Replace(";", "");

                Keyword k;
                if (IsKeyword(current_word, out k))
                    tokens.Add(new Token { keyword = k });
                else if (IsFunctionCall(current_word))
                    foreach (var symbol in ParseFunctionCall(current_word))
                        tokens.Add(symbol);
                else if (IsOperator(current_word))
                    tokens.Add(new Token { keyword = Keyword.@operator, value = current_word });
                else // gotta be a variable ....or a class name.. we need to support this
                    tokens.Add(new Token { keyword = Keyword.symbol_variable, value = current_word });
            }

            return tokens;
        }

        public static bool IsKeyword(string word, out Keyword k)
        {
            if (Enum.TryParse(word, out k))
                return true;
            return false;
        }

        public static bool IsFunctionCall(string word)
        {
            if (word.Contains("("))
                return true;
            return false;
        }

        public static bool IsOperator(string word)
        {
            foreach (var op in operators)
                if (op == word)
                    return true;
            return false;
        }

        public static List<Token> ParseFunctionCall(string call)
        {
            var symbols = call.Split('.');
            var tokens = new List<Token>();

            if (symbols.Length > 0) // chained function call
            {
                for(int i = 0; i < symbols.Length; i++)
                {
                    var symbol = symbols[i];
                    var token = new Token { keyword = Keyword.symbol_variable, value = symbol };
                    if (symbol.Contains("("))
                    {
                        token.keyword = Keyword.symbol_function;
                        token.value = Regex.Replace(symbol, "\\(.*$", "");
                    }

                    tokens.Add(token);
                }
            }
            else // simple call without chaining
            {
                tokens.Add(new Token { keyword = Keyword.symbol_function, value = Regex.Replace(call, "\\(.*$", "") });
            }

            return tokens;
        }
    }
}
