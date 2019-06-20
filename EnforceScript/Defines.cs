using System;
using System.Collections.Generic;
using System.Text;

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

    public class Tokens
    {
        public static bool IsKeyword(string word)
        {
            Keyword k;
            if (Enum.TryParse(word, out k))
                return true;
            return false;
        }
    }
}
