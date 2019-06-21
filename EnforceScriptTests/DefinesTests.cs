using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using EnforceScript;

namespace EnforceScriptTests
{
    class DefinesTests
    {
        [Test]
        public void IsNumber()
        {
            Assert.AreEqual(true, Symbol.IsNumber("1"));
            Assert.AreEqual(true, Symbol.IsNumber("1398"));
            Assert.AreEqual(true, Symbol.IsNumber("125.25"));
            Assert.AreEqual(true, Symbol.IsNumber("-12"));
            Assert.AreEqual(true, Symbol.IsNumber("-123.229"));
            Assert.AreEqual(false, Symbol.IsNumber("1.asd"));
        }
    }
}
