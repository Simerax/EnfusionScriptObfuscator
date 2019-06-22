using System;
using System.Collections.Generic;
using System.Text;

namespace EnforceScript.AST
{
    public abstract class Node
    {
        public Location begin;

        public abstract void accept(Visitor v);
    }

    public class Class : Node
    {
        public string name;
        public bool modded = false;
        public string extends = "";

        public List<VariableDefinition> variables = new List<VariableDefinition>();
        public List<FunctionDefinition> functions = new List<FunctionDefinition>();

        public override void accept(Visitor v)
        {
            v.visit(this);
        }
    }

    public class StringLiteral : Node
    {
        public string value;
        public override void accept(Visitor v)
        {
            v.visit(this);
        }

        public StringLiteral(Word word)
        {
            this.value = word.value;
            this.begin = word.location;
        }

        public StringLiteral()
        {
            this.value = string.Empty;
        }
    }

    public class Number : Node
    {
        public string value;
        public override void accept(Visitor v)
        {
            v.visit(this);
        }

        public Number(Word word)
        {
            this.value = word.value;
            this.begin = word.location;
        }

        public Number()
        {
            this.value = string.Empty;
        }
    }

    public class True : Node
    {
        public override void accept(Visitor v)
        {
            v.visit(this);
        }
    }
    public class False : Node
    {
        public override void accept(Visitor v)
        {
            v.visit(this);
        }
    }

    public class If : Node
    {
        // If condition then {} else {}
        public Condition condition;
        public Block then;
        public Block @else;

        public override void accept(Visitor v)
        {
            v.visit(this);
        }
    }

    public class Condition : Node
    {
        public Node left;
        public string @operator;
        public Node right;
        public bool single_expression = false; // if the condition only contains a single expression (in left)

        public override void accept(Visitor v)
        {
            v.visit(this);
        }

    }

    public class Assignment : Node
    {
        public Node left;
        public Node right;

        public Assignment() { }
        public Assignment(Node left, Node right)
        {
            this.left = left;
            this.right = right;
        }

        public override void accept(Visitor v)
        {
            v.visit(this);
        }
    }

    public class Term : Node
    {
        public string prefix = string.Empty;
        public bool isLiteral = false;
        public string type;
        public Node value;
        public override void accept(Visitor v)
        {
            v.visit(this);
        }
    }


    public class Definition : Node
    {
        public string name;
        public AccessModifier access_modifier;
        public bool @static;

        public override void accept(Visitor v)
        {
            v.visit(this);
        }

    }

    public class ScopeEnter : Definition
    {
        public override void accept(Visitor v)
        {
            v.visit(this);
        }
    }

    public class ScopeExit : Definition
    {
        public override void accept(Visitor v)
        {
            v.visit(this);
        }
    }

    /*
        https://community.bistudio.com/wiki/DayZ:Enforce_Script_Syntax#Variable_modifiers 
    */
    public class VariableDefinition : Definition
    {
        public bool @const;
        public string type;
        public Assignment init; // Initial Variable assignment

        public VariableDefinition(Definition d)
        {
            this.name = d.name;
            this.access_modifier = d.access_modifier;
            this.@static = d.@static;
        }

        public VariableDefinition(string name, string type, bool @const = false, bool @static = false, Assignment init = null)
        {
            this.name = name;
            this.type = type;
            this.@static = @static;
            this.@const = @const;
            this.init = init;
            this.access_modifier = AccessModifier.method;
        }

        public override void accept(Visitor v)
        {
            v.visit(this);
        }
    }

    public class Variable : Node
    {
        public VariableDefinition definition;

        public override void accept(Visitor v)
        {
            v.visit(this);
        }
    }

    public class FunctionDefinition : Definition
    {
        public List<Arg> args;
        public string return_type;
        public Block body;
        public FunctionDefinition(Definition d)
        {
            this.name = d.name;
            this.access_modifier = d.access_modifier;
            this.@static = d.@static;
        }
        public override void accept(Visitor v)
        {
            v.visit(this);
        }
    }

    public class Arg : Node
    {
        public string type;
        public string name;
        public Node default_value;
        public override void accept(Visitor v)
        {
            v.visit(this);
        }
    }

    public class Expression : Node
    {
        public Node value;

        public override void accept(Visitor v)
        {
            v.visit(this);
        }
    }

    public class Statement : Node
    {
        public Node body;

        public override void accept(Visitor v)
        {
            v.visit(this);
        }
    }

    public class Block : Node
    {
        public List<Statement> statements = new List<Statement>();
        public override void accept(Visitor v)
        {
            v.visit(this);
        }
    }
}
