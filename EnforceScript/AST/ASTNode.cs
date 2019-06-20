using System;
using System.Collections.Generic;
using System.Text;

namespace EnforceScript.AST
{
    public abstract class Node
    {
        private Location begin;
        private Location end;

        public abstract void accept(Visitor v);
    }

    public class Class : Node
    {
        public string name;
        public bool modded = false;
        public string parent = "";

        public List<VariableDefinition> variables = new List<VariableDefinition>();

        public override void accept(Visitor v)
        {
            throw new NotImplementedException();
        }
    }


    public class Definition : Node
    {
        public string name;
        public AccessModifier access_modifier;
        public bool @static;

        public override void accept(Visitor v)
        {
            throw new NotImplementedException();
        }

    }

    /*
        https://community.bistudio.com/wiki/DayZ:Enforce_Script_Syntax#Variable_modifiers 
    */
    public class VariableDefinition : Definition
    {
        public bool @const;
        public string type;

        public VariableDefinition(Definition d)
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

    public class Block : Node
    {
        public override void accept(Visitor v)
        {
            throw new NotImplementedException();
        }
    }
}
