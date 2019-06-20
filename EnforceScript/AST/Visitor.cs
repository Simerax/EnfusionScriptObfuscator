using System;
using System.Collections.Generic;
using System.Text;

namespace EnforceScript.AST
{
    public class Visitor
    {
        public void visit(AST.Node node)
        {
            throw new NotImplementedException();
        }

        public void visit(AST.Class node)
        {
            node.accept(this);
        }

    }
}
