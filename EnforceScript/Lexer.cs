using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace EnforceScript
{
    /*
        TODO:
        * "modded" class keyword has to check if the modded class is a "mod" class or a Standard DayZ Class.
            * We can't obfuscate a Standard DayZ Class e.g. "MissionServer"
        * Don't Obfuscate Standard Types like "Vector2" 
            * This also applies to function calls of such classes!  
    */




    public struct Word
    {
        public string value;
        public Location location;

        public Word(string value, uint line, uint column)
        {
            this.value = value;
            this.location = new Location(line, column);
        }

        public Word(string value, Location location)
        {
            this.value = value;
            this.location = location;
        }
    }
   

    public class Lexer
    {

        public static List<Word> Lex(string content)
        {
            var current_word = new Stack<char>();
            var words = new List<Word>();
            bool inside_oneline_comment = false;
            bool inside_multiline_comment = false;
            bool inside_string_literal = false;

            Location current_location = new Location(1, 0);

            for (int i = 0; i < content.Length; i++)
            {
                var previous_char = new char();
                var current_char = content[i];
                char next_char = new char();
                if (i + 1 < content.Length)
                    next_char = content[i + 1];
                if (i > 0)
                    previous_char = content[i - 1];

                // keep track of lines and columns
                current_location.column++;
                if (IsNewline(previous_char))
                {
                    current_location.line++;
                    current_location.column = 1;
                }


                if (current_char == '"' && previous_char != '\\')
                    if (!inside_string_literal)
                        inside_string_literal = true;
                    else
                        inside_string_literal = false;

                if (inside_string_literal)
                {
                    current_word.Push(current_char);
                    continue;
                }


                // One line comment
                if (current_char == '/' && next_char == '/')
                    inside_oneline_comment = true;

                if (inside_oneline_comment)
                    if (!IsNewline(current_char))
                        continue;
                    else
                        inside_oneline_comment = false;

                // Multiline Comment
                if (current_char == '/' && next_char == '*')
                    inside_multiline_comment = true;

                if (inside_multiline_comment)
                {
                    if (previous_char == '*' && current_char == '/')
                        inside_multiline_comment = false;
                    continue;
                }


                if (IsDelimiter(current_char) || i+1 == content.Length)
                {
                    var word = string.Empty;
                    foreach (var chr in current_word)
                        word += chr;
                    current_word.Clear();

                    if (word.Length > 0)
                    {
                        // since the stack is fifo the word is actually stored in reverse so we gotta turn it back into the correct order
                        words.Add(new Word(ReverseString(word), current_location.line, current_location.column - (uint)word.Length));
                    }

                    if (!IsWhitespace(current_char) && IsDelimiter(current_char))
                        words.Add(new Word(current_char.ToString(), current_location.line, current_location.column));

                } else
                {
                    current_word.Push(current_char);
                }
            }

            return words;
        }

        public static bool IsNewline(char c)
        {
            if (c == '\n')
                return true;
            return false;
        }

        public static bool IsDelimiter(char c)
        {
            foreach (var delim in delimiters)
                if (c == delim)
                    return true;
            return false;
        }

        public static bool IsWhitespace(char c)
        {
            foreach (var s in whitespace)
                if (c == s)
                    return true;
            return false;
        }


        public static string ReverseString(string input)
        {
            char[] arr = input.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

       

        public static readonly char[] whitespace =
        {
            ' ',
            '\r',
            '\n',
            '\t',
        };

        public static readonly char[] delimiters =
        {
            ' ',
            '\r',
            '\n',
            '\t',
            ';',
            '.',
            '(',
            ')',
            '{',
            '}',
            '<',
            '>'
        };
    }
}
