using System;
using System.Collections.Generic;
using System.Text;

namespace EnforceObfuscator.Enforce
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
    }

    public struct Token
    {
        public Keyword keyword;
    }
    class Tokenizer
    {
        public static readonly string[] arithmetic_operators = {
            "+",
            "-",
            "*",
            "/",
            "%"
        };

        public static readonly string[] assignment_operators = {
            "=",
            "+=",
            "-=",
            "*=",
            "/=",
            "++",
            "--",
        };

        public static readonly string[] relational_operators = {
            ">",
            "<",
            ">=",
            "<=",
            "==",
            "!=",
        };

        public static readonly string[] operators = {
            // Others
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
        public List<Token> Tokenize(string content)
        {
            var tokens = new List<Token>();

            return tokens;
        }
    }
}
