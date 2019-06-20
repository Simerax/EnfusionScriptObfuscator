using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Moq;
using EnforceScript;
using EnforceScript.AST;


namespace EnforceScriptTests
{

    public class TestVisitor : Visitor
    {
        public Action<VariableDefinition> OnVariableDefinition;
        public Action<Class> OnClass;

        public override void visit(Class node)
        {
            OnClass?.Invoke(node);
            base.visit(node);
        }

        public override void visit(VariableDefinition node)
        {
            OnVariableDefinition?.Invoke(node);
            base.visit(node);
        }
    }

    class ParserTests
    {
        [Test]
        public void Parse_VariableDefinition()
        {
            string content = @"
            class Test extends Base
            {
                protected string x;
            }";
            var input = Lexer.Lex(content);
            var result = Parser.Parse(input.ToArray());

            Assert.IsInstanceOf<Class>(result);

            var visitor = new TestVisitor();
            visitor.OnVariableDefinition += (VariableDefinition vd) => {
                Assert.AreEqual(AccessModifier.@protected, vd.access_modifier);
                Assert.AreEqual("string", vd.type);
                Assert.AreEqual("x", vd.name);
            };

            visitor.OnClass += (Class cl) => {
                Assert.AreEqual("Test", cl.name);
                Assert.AreEqual("Base", cl.parent);
                Assert.AreEqual(false, cl.modded);
            };

            visitor.visit((dynamic)result);
            
        }
    }
}
