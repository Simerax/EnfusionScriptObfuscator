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

        private string GetCurrentSymbol()
        {
            if (current_symbol_index >= words.Length)
                throw new IndexOutOfRangeException("No More Symbols");
            return words[current_symbol_index].value;
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

        private bool GoToPreviousSymbol()
        {
            if (current_symbol_index == 0)
                throw new IndexOutOfRangeException("There is no Symbol before this one");
            current_symbol_index--;
            return true;
        }

        private bool CurrentSymbol(string sym)
        {
            if (current_symbol_index >= words.Length)
                throw new IndexOutOfRangeException("No More Symbols");
            if (words[current_symbol_index].value == sym)
                return true;
            return false;
        }

        private bool Accept(string sym)
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

        private bool Accept(bool val)
        {
            if (current_symbol_index >= words.Length)
                throw new IndexOutOfRangeException("No More Symbols");
            if(val)
            {
                GetNextSymbol();
                return true;
            }
            return false;
        }

        private bool Id()
        {
            var next_symbol = GetNextSymbol();

            if (Symbol.IsKeyword(next_symbol) || Symbol.IsStringLiteral(next_symbol))
                return false;
            else
                return true;
        }

        private bool Expect(bool val)
        {
            if (current_symbol_index >= words.Length)
                throw new IndexOutOfRangeException("No More Symbols");
            if (val)
                return true;
            else
                throw new AST.UnexpectedSymbolException(words[current_symbol_index]);
        }

        private bool PrefixOperator()
        {
            return Symbol.IsPrefixOperator(GetCurrentSymbol());
        }

        private bool StringLiteral()
        {
            return Symbol.IsStringLiteral(GetCurrentSymbol());
        }

        private bool Number()
        {
            return Symbol.IsNumber(GetCurrentSymbol());
        }

        private bool Expect(string sym)
        {
            if (current_symbol_index >= words.Length)
                throw new IndexOutOfRangeException("No More Symbols");
            if (words[current_symbol_index].value == sym)
            {
                GetNextSymbol();
                return true;
            }
            throw new AST.UnexpectedSymbolException(words[current_symbol_index]);
        }

        public static AST.Node Parse(Word[] words)
        {
            var p = new Parser(words);
            return p.Parse();
        }

        public AST.Node Parse()
        {
            if (Accept("class") || Accept("modded"))
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

            if (Accept("extends"))
                cl.extends = GetNextSymbol();

            Expect("{");

            // Now its time to parse all the function and variable definitions of the class
            while(!CurrentSymbol("}"))
            {
                var definition = Definition();
                if (definition is AST.VariableDefinition)
                    cl.variables.Add((AST.VariableDefinition)definition);
                else if (definition is AST.FunctionDefinition)
                    cl.functions.Add((AST.FunctionDefinition)definition);
                else
                    throw new AST.UnexpectedSymbolException(words[current_symbol_index]);
            }
            return cl;
        }

        private AST.Statement Statement()
        {
            var statement = new AST.Statement();
            return statement;
        }

        private AST.Term Term()
        {
            var term = new AST.Term();

            if (Accept(PrefixOperator()))
                term.prefix = GetLastSymbol().value;
            if (Accept(StringLiteral()))
            {
                term.isLiteral = true;
                term.value = new AST.StringLiteral(GetLastSymbol());
            }
            if(Accept(Number()))
            {
                term.isLiteral = true;
                term.value = new AST.Number(GetLastSymbol());
            }

            return term;
        }

        private AST.Expression Expression()
        {
            var exp = new AST.Expression();
            exp.value = Term();

            Expect(";");
            return exp;
        }



        private List<AST.Arg> ArgList()
        {
            var args = new List<AST.Arg>();

            if(Expect("("))
            {
                // read all the arguments
                while(true)
                {
                    var argument = new AST.Arg();

                    // first we expect the type then the name
                    if(Expect(Id()))
                        argument.type = GetCurrentSymbol();
                    if (Expect(Id()))
                        argument.name = GetCurrentSymbol();

                    if (Accept("=")) // Default value for argument
                        throw new NotImplementedException();
                    if (Accept(",")) // next argument
                        args.Add(argument);
                    else if (Accept(")")) // no more arguments left
                    {
                        args.Add(argument);
                        break;
                    }
                }
            }

            return args;
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
            if (Accept("("))
            {
                GoToPreviousSymbol();
                AST.FunctionDefinition function = new AST.FunctionDefinition(definition);
                function.args = ArgList();
            }
            else
            {
                AST.VariableDefinition variable = new AST.VariableDefinition(definition);
                variable.type = type;

                // No Value assigned to the variable
                if (Accept(";"))
                    return variable;
                else if (Accept("="))
                {
                    var assignment = new AST.Assignment();
                    assignment.left = variable;
                    assignment.right = Expression();
                    variable.init = assignment;
                    return variable;
                }
                else
                    throw new AST.UnexpectedSymbolException(GetLastSymbol());
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
