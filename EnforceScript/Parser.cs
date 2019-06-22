using System;
using System.Collections.Generic;
using System.Text;

namespace EnforceScript
{
    public class Parser
    {
        private Word[] words;
        private int current_symbol_index = 0;

        private SymbolTable symbol_table = new SymbolTable();
        public Parser(Word[] words)
        {
            this.words = words;
        }

        private string GetNextSymbol()
        {
            current_symbol_index++;
            return words[current_symbol_index - 1].value;

        }


        // TODO: This should return a Word
        private Word GetCurrentSymbol()
        {
            if (current_symbol_index >= words.Length)
                throw new IndexOutOfRangeException("No More Symbols");
            return words[current_symbol_index];
        }

        private Word? GetSymbol(uint ahead = 0)
        {
            if (current_symbol_index + ahead >= words.Length)
                return null;
            else
                return words[current_symbol_index + ahead];
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

        private bool Peek(string sym, uint ahead = 0)
        {
            if (current_symbol_index + ahead >= words.Length)
                return false;
            if (words[current_symbol_index + ahead].value == sym)
                return true;
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
                UnexpectedSymbolError();
            return false; // we will never reach this but VisualStudio doesn't get it
        }

        private void UnexpectedSymbolError()
        {
            throw new AST.UnexpectedSymbolException(GetCurrentSymbol());
        }

        private bool BoolLiteral()
        {
            var current = GetCurrentSymbol();
            if (current.value == "true" || current.value == "false")
                return true;
            return false;
        }

        private bool Keyword()
        {
            return Symbol.IsKeyword(GetCurrentSymbol().value);
        }

        private bool PrefixOperator()
        {
            return Symbol.IsPrefixOperator(GetCurrentSymbol().value);
        }

        private bool ComparisonOperator()
        {
            return Symbol.IsComparisonOperator(GetCurrentSymbol().value);
        }

        private bool InfixOperator()
        {
            return Symbol.IsInfixOperator(GetCurrentSymbol().value);
        }

        private bool Operator()
        {
            return Symbol.IsOperator(GetCurrentSymbol().value);
        }

        private bool StringLiteral()
        {
            return Symbol.IsStringLiteral(GetCurrentSymbol().value);
        }

        private bool Number()
        {
            return Symbol.IsNumber(GetCurrentSymbol().value);
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
            UnexpectedSymbolError();
            return false;
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
            symbol_table.EnterScope();

            // Now its time to parse all the function and variable definitions of the class
            while(!CurrentSymbol("}"))
            {
                var def = Definition();
                if (def is AST.VariableDefinition)
                    cl.variables.Add((AST.VariableDefinition)def);
                else if (def is AST.FunctionDefinition)
                    cl.functions.Add((AST.FunctionDefinition)def);
            }
            symbol_table.ExitScope();
            return cl;
        }

        private AST.Condition Condition()
        {
            var condition = new AST.Condition();
            condition.left = Expression();

            if(Accept(")"))
            {
                // the closing parenthesis is probably needed by the caller
                GoToPreviousSymbol();
                condition.single_expression = true;
                return condition;
            }
            else if (ComparisonOperator())
                condition.@operator = GetCurrentSymbol().value;
            else
                UnexpectedSymbolError();

            condition.right = Expression();

            return condition;
        }

        private AST.If IfStatement()
        {
            var if_expr = new AST.If();
            Expect("if");
            Expect("(");
            if_expr.condition = Condition();
            Expect(")");
            if_expr.then = Block();
            if (Accept("else"))
                if_expr.@else = Block();
            // TODO: Parse else if()
            return if_expr;
        }

        private AST.Statement Statement()
        {
            var statement = new AST.Statement();

            if (Accept("if"))
            {
                GoToPreviousSymbol(); // let IfStatement() re-read the "if"
                statement.body = IfStatement();
            }
            else if (Accept("while"))
                throw new NotImplementedException();
            else if (Accept("return"))
            {
                throw new NotImplementedException();
            }
            else // if nothing matches then we are either in a variable definition or in some assignment
            {
                // Variable Definition
                // This would mean that the next symbol is the type, then after that the name and then either a ";" or an assignment
                if (Peek(";", 2) || Peek("=", 2) || Peek("const"))
                {
                    bool is_const = false;
                    if (Accept("const"))
                        is_const = true;

                    var type = GetNextSymbol();
                    var name = GetNextSymbol();
                    var definition = new AST.VariableDefinition(name, type, is_const);
                    symbol_table.AddDefinition(definition);
                    if (Accept("="))
                    {
                        var assignment = new AST.Assignment(definition, Expression());
                        definition.init = assignment;
                        statement.body = definition;
                    }
                    else if (Accept(";"))
                    {
                        statement.body = definition;
                    }
                    else
                        UnexpectedSymbolError();
                }
                else if (Peek("=", 1))
                {
                    statement.body = Expression();

                }
                else
                    throw new AST.UnexpectedSymbolException(GetSymbol(2) ?? default(Word)); // This is really shitty

            }

            return statement;
        }

        private AST.Assignment Assignment()
        {
            var assignment = new AST.Assignment();
            assignment.left = Expression();
            Expect("=");
            assignment.right = Expression();
            return assignment;
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
            else if (Accept(Number()))
            {
                term.isLiteral = true;
                term.value = new AST.Number(GetLastSymbol());
            }
            else if (Accept("true"))
            {
                term.isLiteral = true;
                term.value = new AST.True();
            }
            else if (Accept("false"))
            {
                term.isLiteral = true;
                term.value = new AST.False();
            }
            else if (Keyword())
                UnexpectedSymbolError();
            else if (Peek("(", 1)) // Function call 
            {
                throw new NotImplementedException();
            }
            else // has to be a variable
            {
                var variable_name = GetNextSymbol();
                var definition = symbol_table.GetVariableDefinition(variable_name);
                if (definition == null)
                    throw new Exception($"Variable {variable_name} not defined!");

                var variable = new AST.Variable();
                variable.definition = definition;
                term.value = variable;
            }

            return term;
        }

        private AST.Expression Expression()
        {
            var exp = new AST.Expression();
            exp.value = Term();

            if (Accept(";"))
                return exp;
            else if(Accept("="))
            {
                var assignment = new AST.Assignment();
                assignment.left = exp.value;
                assignment.right = Expression();
                exp.value = assignment;
            }
            else if (Accept(")"))
            {
                // the closing Parenthesis might be important to the calling function
                GoToPreviousSymbol(); 
                return exp;
            }
            else
                UnexpectedSymbolError();
            return exp;
        }



        private List<AST.Arg> ArgList()
        {
            var args = new List<AST.Arg>();

            if(Expect("("))
            {
                if (Accept(")"))
                    return args;
                // read all the arguments
                while(true)
                {
                    var argument = new AST.Arg();

                    // first we expect the type then the name
                    if(Expect(Id()))
                        argument.type = GetCurrentSymbol().value;
                    if (Expect(Id()))
                        argument.name = GetCurrentSymbol().value;

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


            // function can be either "static private xyz" or "private static xyz"
            // and Enforce actually allows a function to be "static private static xyz"
            if (Accept("static"))
                definition.@static = true;

            if (Accept("private"))
                definition.access_modifier = AccessModifier.@private;
            else if (Accept("protected"))
                definition.access_modifier = AccessModifier.@protected;
            else // default is private
                definition.access_modifier = AccessModifier.@private;

            if (Accept("static"))
                definition.@static = true;


            var type = GetNextSymbol();
            definition.name = GetNextSymbol();



            // now we know it's a function definition
            if (Accept("("))
            {
                GoToPreviousSymbol();
                AST.FunctionDefinition function = new AST.FunctionDefinition(definition);
                function.return_type = type;
                symbol_table.AddDefinition(function);
                function.args = ArgList();
                function.body = Block();
                return function;
            }
            else
            {
                AST.VariableDefinition variable = new AST.VariableDefinition(definition);
                variable.type = type;
                symbol_table.AddDefinition(variable);

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
            throw new Exception("We should never be here!");
        }



        private AST.Block Block()
        {
            var block = new AST.Block();
            Expect("{");
            symbol_table.EnterScope();
            while(!Accept("}"))
            {
                var statement = Statement();
                block.statements.Add(statement);
            }
            symbol_table.ExitScope();
            return block;
        }
    }
}
