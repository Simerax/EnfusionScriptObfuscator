using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using EnforceObfuscator.Enforce;

namespace EnforceObfuscator.Tests.Enforce
{
    class TokenizerTest
    {
        public Tokenizer MakeTokenizer()
        {
            return new Tokenizer();
        }
        [Test]
        public void test_01()
        {
            var tokenizer = MakeTokenizer();

            string input = "private";
            var expected = new List<Token>();
            expected.Add(new Token { keyword = Keyword.@private});
            var result = tokenizer.Tokenize(input);

            Assert.AreEqual(expected, result);
        }
    }
}
