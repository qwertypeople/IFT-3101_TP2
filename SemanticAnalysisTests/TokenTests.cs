using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysisTests
{
    public class TokenTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Constructor_WhenSymbolIsNonterminal_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void Constructor_WhenSymbolIsSpecialOtherThanEnd_ShouldThrowException()
        {
            Assert.Fail();
        }
    }
}
