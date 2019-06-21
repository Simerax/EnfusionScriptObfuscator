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

        public Node LexAndParse(string content)
        {
            return Parser.Parse(Lexer.Lex(content).ToArray());
        }

        [Test]
        public void ClassDefinition()
        {
            var result = LexAndParse("class Test {}");
            var visitor = new TestVisitor();
            visitor.OnClass += (Class cl) => {
                Assert.AreEqual("Test", cl.name);
                Assert.AreEqual(false, cl.modded);
                Assert.AreEqual("", cl.extends);
                Assert.AreEqual(0, cl.variables.Count);
            };
        }

        [Test]
        public void ClassDefinitionWithExtends()
        {
            var result = LexAndParse("class Myclass extends Timer {}");
            var visitor = new TestVisitor();
            visitor.OnClass += (Class cl) => {
                Assert.AreEqual("MyClass", cl.name);
                Assert.AreEqual(false, cl.modded);
                Assert.AreEqual("Timer", cl.extends);
                Assert.AreEqual(0, cl.variables.Count);
            };
        }

        [Test]
        public void ModdedClassDefinition()
        {
            var result = LexAndParse("modded Myclass {}");
            var visitor = new TestVisitor();
            visitor.OnClass += (Class cl) => {
                Assert.AreEqual("MyClass", cl.name);
                Assert.AreEqual(true, cl.modded);
                Assert.AreEqual("", cl.extends);
                Assert.AreEqual(0, cl.variables.Count);
            };
        }

        [Test]
        public void ImplicitPrivateVariableDeclaration()
        {
            string content = @"
            class Test extends Base
            {
                int x;
            }
            ";

            var result = LexAndParse(content);
            var visitor = new TestVisitor();

            visitor.OnVariableDefinition += (VariableDefinition vd) => {
                Assert.AreEqual("int", vd.type);
                Assert.AreEqual("x", vd.name);
                Assert.AreEqual(AccessModifier.@private, vd.access_modifier);
            };

            visitor.visit((dynamic)result);
        }

        [Test]
        public void ProtectedVariableDeclaration()
        {
            string content = @"
            class Test extends Base
            {
                protected string x;
            }";

            var result = LexAndParse(content);

            Assert.IsInstanceOf<Class>(result);

            var visitor = new TestVisitor();
            visitor.OnVariableDefinition += (VariableDefinition vd) => {
                Assert.AreEqual(AccessModifier.@protected, vd.access_modifier);
                Assert.AreEqual("string", vd.type);
                Assert.AreEqual("x", vd.name);
            };

            visitor.OnClass += (Class cl) => {
                Assert.AreEqual("Test", cl.name);
                Assert.AreEqual("Base", cl.extends);
                Assert.AreEqual(false, cl.modded);
            };
            visitor.visit((dynamic)result);
        }

        [Test]
        public void VariableDefinition()
        {
            string content = @"
            class Test
            {
                int x = 25;
                protected string l;
            }
            ";

            var result = LexAndParse(content);
            var visitor = new TestVisitor();

            VariableDefinition x = null;
            VariableDefinition l = null;

            visitor.OnVariableDefinition += (VariableDefinition vd) => {
                if (vd.name == "x")
                    x = vd;
                else if (vd.name == "l")
                    l = vd;
            };

            visitor.visit((dynamic)result);

            Assert.NotNull(x);
            Assert.NotNull(l);

            Assert.AreEqual("x", x.name);
            Assert.AreEqual("int", x.type);
            Assert.AreEqual(AccessModifier.@private, x.access_modifier);

            Assert.IsInstanceOf<Expression>((dynamic)x.init.right);
            Assert.IsInstanceOf<Number>(((Term)((Expression)x.init.right).value).value);
            Assert.AreEqual("25",((Number)((Term)((Expression)x.init.right).value).value).value);

            Assert.AreEqual("l", l.name);
            Assert.AreEqual("string", l.type);
            Assert.AreEqual(AccessModifier.@protected, l.access_modifier);

        }

    }
}
