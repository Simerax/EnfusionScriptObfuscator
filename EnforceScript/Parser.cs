using System;
using System.Collections.Generic;
using System.Text;

namespace EnforceScript
{
    public class Parser
    {
        private Word[] words;
        private int current_symbol_index = 0;
        public Parser(Word[] words)
        {
            this.words = words;
        }

        private string GetNextSymbol()
        {
            current_symbol_index++;
            return words[current_symbol_index - 1].value;

        }
        private Word GetLastSymbol()
        {
            if (current_symbol_index == 0)
                throw new IndexOutOfRangeException("There is no Last Symbol!");
            return words[current_symbol_index - 1];
        }
        private bool LastSymbol(string sym)
        {
            if (current_symbol_index == 0)
                throw new IndexOutOfRangeException("There is no Last Symbol!");

            if (words[current_symbol_index - 1].value == sym)
                return true;
            return false;
        }

        private bool CurrentSymbol(string sym)
        {
            if (current_symbol_index >= words.Length)
                throw new IndexOutOfRangeException("No More Symbols");
            if (words[current_symbol_index].value == sym)
                return true;
            return false;
        }

        private bool NextSymbol(string sym)
        {
            if (current_symbol_index >= words.Length)
                throw new IndexOutOfRangeException("No More Symbols");
            if (words[current_symbol_index].value == sym)
            {
                GetNextSymbol();
                return true;
            }
            return false;
        }

        public static AST.Node Parse(Word[] words)
        {
            var p = new Parser(words);
            return p.Parse();
        }

        public AST.Node Parse()
        {
            if (NextSymbol("class") || NextSymbol("modded"))
                return Class();
            else
                return Block();
        }

        private AST.Class Class()
        {
            var cl = new AST.Class();
            if (LastSymbol("modded"))
                cl.modded = true;

            cl.name = GetNextSymbol();

            if (NextSymbol("extends"))
                cl.parent = GetNextSymbol();

            if (!NextSymbol("{"))
                throw new AST.UnexpectedSymbolException(GetLastSymbol());


            // Now its time to parse all the function and variable definitions of the class
            while(!CurrentSymbol("}"))
            {
                var definition = Definition();
                if (definition is AST.VariableDefinition)
                    cl.variables.Add((AST.VariableDefinition)definition);
                else
                    throw new AST.UnexpectedSymbolException("LOL WHATS THIS!?");
            }
            return cl;
        }


        private AST.Definition Definition()
        {
            var definition = new AST.Definition();
            var next_symbol = GetNextSymbol();

            if (next_symbol == "private")
            {
                definition.access_modifier = AccessModifier.@private;
                next_symbol = GetNextSymbol();
            }
            else if (next_symbol == "protected")
            {
                definition.access_modifier = AccessModifier.@protected;
                next_symbol = GetNextSymbol();
            }
            else
                definition.access_modifier = AccessModifier.@private;

            var type = next_symbol;
            definition.name = GetNextSymbol();

            // now we know it's a function definition
            if (NextSymbol("("))
            {

            }
            else
            {
                AST.VariableDefinition variable = new AST.VariableDefinition(definition);
                variable.type = type;

                // No Value assigned to the variable
                if (NextSymbol(";"))
                    return variable;
                else if (NextSymbol("="))
                    throw new NotImplementedException();
                else
                    throw new NotImplementedException();

            }

            return definition;
        }

        private AST.Node Block()
        {
            var next_symbol = GetNextSymbol();
            return new AST.Block();
        }
    }
}
