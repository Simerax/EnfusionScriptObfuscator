using System;
using System.Collections.Generic;
using System.Text;

namespace EnforceScript
{
    class SymbolTable
    {
        public enum Type
        {
            root,
            scope_enter,
            variable_definition,
            function_definition,
        }
        public struct Symbol
        {
            public Type type;
            public AST.Definition def;
        }

        private Stack<Symbol> table = new Stack<Symbol>();

        public SymbolTable()
        {
            table.Push(root());
        }

        public void AddDefinition(AST.FunctionDefinition def)
        {
            table.Push(new Symbol { type = Type.function_definition, def = def });
        }

        public void AddDefinition(AST.VariableDefinition def)
        {
            table.Push(new Symbol { type = Type.variable_definition, def = def });
        }

        public bool IsVariableDefined(string variable_name)
        {
            foreach (var symbol in reversed_table())
                if(symbol.type == Type.variable_definition)
                    if (symbol.def is AST.VariableDefinition)
                        if (((AST.VariableDefinition)symbol.def).name == variable_name)
                            return true;
            return false;
        }

        public AST.VariableDefinition GetVariableDefinition(string variable_name)
        {
            foreach (var symbol in reversed_table())
            {
                if (symbol.type != Type.variable_definition)
                    continue;


                if (symbol.def is AST.VariableDefinition)
                {
                    var def = (AST.VariableDefinition)symbol.def;
                    if (def.name == variable_name)
                        return def;
                }
            }
            return null;
        }

        public AST.FunctionDefinition GetFunctionDefinition(string function_name)
        {
            foreach (var symbol in reversed_table())
            {
                if (symbol.type != Type.function_definition)
                    continue;


                if (symbol.def is AST.FunctionDefinition)
                {
                    var def = (AST.FunctionDefinition)symbol.def;
                    if (def.name == function_name)
                        return def;
                }
            }
            return null;
        }

        public bool IsFunctionDefined(string function_name)
        {
            foreach (var symbol in reversed_table())
                if(symbol.type == Type.function_definition)
                    if (symbol.def is AST.FunctionDefinition)
                        if (((AST.FunctionDefinition)symbol.def).name == function_name)
                            return true;
            return false;
        }


        public void EnterScope()
        {
            table.Push(new Symbol { type = Type.scope_enter });
        }

        private Symbol root()
        {
            return new Symbol { type = Type.root };
        }

        public void ExitScope()
        {
            while(true) {
                var popped = table.Pop();

                if (popped.type == Type.scope_enter)
                    break;
                if (popped.type == Type.root)
                {
                    table.Push(root());
                    break;
                }
            }
        }

        private Symbol[] reversed_table()
        {
            var arr = table.ToArray();
            Array.Reverse(arr);
            return arr;
        }

    }
}
