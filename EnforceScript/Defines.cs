using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace EnforceScript
{
    public enum PrimitiveType
    {
        @int,
        @float,
        @bool,
        @string,
        @vector,
        @void,
        @auto,
        @class,
    }

    public enum AccessModifier
    {
        @private,
        @protected,
    }

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

        //Control Flow
        @if,
        @else,
        @while,
        @switch,
        @break,
        @case,
        @for,
        @foreach,
    }


    public class Types
    {
        public static bool IsPrimitive(string word)
        {
            PrimitiveType t;
            if (Enum.TryParse(word, out t))
                return true;
            return false;
        }

        public static PrimitiveType ConvertToPrimitive(string word)
        {
            PrimitiveType t;
            if (Enum.TryParse(word, out t))
                return t;
            return t;
        }
    }

    public class Symbol
    {
        public static readonly string[] prefix_operators = {
            "!",
            "-",
            "+"
        };
        public static bool IsKeyword(string word)
        {
            Keyword k;
            if (Enum.TryParse(word, out k))
                return true;
            return false;
        }

        public static bool IsStringLiteral(string word)
        {
            if (word.StartsWith("\"") && word.EndsWith("\""))
                return true;
            return false;
        }

        public static bool IsPrefixOperator(string word)
        {
            foreach (var op in prefix_operators)
                if (op == word)
                    return true;
            return false;
        }

        public static bool IsNumber(string word)
        {
            return Regex.IsMatch(word, "^[-+]?[0-9]+(\\.[0-9]+)?$");
        }
    }
}
