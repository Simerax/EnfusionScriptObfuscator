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
            foreach (var function in node.functions)
                function.accept(this);
        }

        virtual public void visit(AST.Block block)
        {
            foreach (var statement in block.statements)
                statement.accept(this);
        }

        virtual public void visit(AST.Statement statement)
        {
            statement.body.accept(this);
        }

        virtual public void visit(AST.Term t)
        {
            t.value.accept(this);
        }

        virtual public void visit(FunctionDefinition node)
        {
            node.body.accept(this);
        }

        virtual public void visit(AST.If @if)
        {
            @if.condition.accept(this);
            @if.then.accept(this);
            if (@if.@else != null)
                @if.@else.accept(this);
        }

        virtual public void visit(AST.Assignment assignment)
        {
            assignment.left.accept(this);
            assignment.right.accept(this);
        }

        virtual public void visit(AST.Expression expression)
        {
            expression.value.accept(this);
        }

        virtual public void visit(AST.True @true)
        {
            
        }

        virtual public void visit(AST.Condition condition)
        {
            condition.left.accept(this);
            if (condition.right != null)
                condition.right.accept(this);
        }

        virtual public void visit(AST.Variable variable)
        {
            if (variable.definition == null)
                throw new Exception($"Variable at {variable.begin} was not defined!");
        }

        virtual public void visit(AST.Number number)
        {
            
        }

    }
}
