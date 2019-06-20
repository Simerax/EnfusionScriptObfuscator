using System;
using System.Collections.Generic;
using System.Text;

namespace EnforceScript.AST
{
    public class Visitor
    {
        virtual public void visit(AST.Node node)
        {
            throw new NotImplementedException();
        }

        virtual public void visit(AST.VariableDefinition node)
        {

        }

        virtual public void visit(AST.Class node)
        {
            foreach (var variable in node.variables)
                variable.accept(this);
        }

    }
}
