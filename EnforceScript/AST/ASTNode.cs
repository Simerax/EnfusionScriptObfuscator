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
        public override void accept(Visitor v)
        {
            throw new NotImplementedException();
        }
    }
}
