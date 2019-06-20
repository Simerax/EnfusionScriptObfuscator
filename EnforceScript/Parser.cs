using System;
using System.Collections.Generic;
using System.Text;

namespace EnforceScript
{
    public enum SymbolType
    {
        enter_scope,
        exit_scope,
        variable,
        @class,
    }

    public struct Symbol
    {
        public SymbolType type;
        public string value;
    };


    public struct Type
    {
        public PrimitiveType p_type;
        public string class_name;

        public Type(PrimitiveType t)
        {
            p_type = t;
            class_name = "";
        }
    }


    public class Variable
    {
        public Type type;
        public string name;

    }

    public class Function
    {
        public Type returntype;
        public string name;
        public List<Type> parameters;
    }


    public class Class
    {
        public string name;
        public string parent;
        public bool modded = false;
        public List<Variable> members = new List<Variable>();
        public List<Function> methods = new List<Function>();
    }

    public class Parser
    {
        private Word[] words;
        private int current_symbol_index = 0;
        private List<Class> classes;
        private Class current_class;
        public Parser(Word[] words)
        {
            this.words = words;
            classes = new List<Class>();
            current_class = new Class();
        }

        private string ConsumeSymbol()
        {
            current_symbol_index++;
            return words[current_symbol_index - 1].value;

        }

        private bool NextSymbol(string sym)
        {
            if (current_symbol_index >= words.Length)
                throw new IndexOutOfRangeException("No More Symbols");
            if (words[current_symbol_index].value == sym)
            {
                ConsumeSymbol();
                return true;
            }
            return false;
        }

        public void parse()
        {
            Block();
        }

        private void ParseClass()
        {
            if (NextSymbol("class") || NextSymbol("modded"))
            {
                current_class.name = ConsumeSymbol();
                if (NextSymbol("extends"))
                    current_class.parent = ConsumeSymbol();

                if (NextSymbol("{"))
                    Block();
                else
                    throw new Exception("After a class name their should be a class definition");

            } else
            {
                throw new Exception("Invalid Syntax - No Class Name given");
            }
        }

        private void Block()
        {
            // we are in a class definition
            if (NextSymbol("class"))
            {

            }


            if (NextSymbol("private") || NextSymbol("protected"))
            {
                NextSymbol("static"); // just eat the static keyword we dont care if its there or not
                var type_str = ConsumeSymbol();
                var name = ConsumeSymbol();

                Type type = new Type();
                if (Types.IsPrimitive(type_str))
                    type.p_type = Types.ConvertToPrimitive(type_str);
                else
                {
                    type.p_type = PrimitiveType.@class;
                    type.class_name = type_str;
                }

                // its a function
                if (NextSymbol("("))
                {
                    var fun = new Function();
                    fun.returntype = type;
                    fun.name = name;

                    while(true)
                    {
                        var sym = ConsumeSymbol();



                        // TODO: Add check eof
                        if (sym == ")")
                            break;
                    }
                }
                // its a variable
                else
                {
                    var variable = new Variable();
                    variable.name = name;
                    variable.type = type;
                    current_class.members.Add(variable);
                }
                    
            }
        }
    }
}
