using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysisTests
{
    public class ParseNodeTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Constructor_WhenSymbolIsNonterminalAndProductionIsNull_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void Constructor_WhenSymbolIsNonterminalAndIsNotHeadOfProduction_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void Constructor_WhenSymbolIsTerminalAndProductionIsNotNull_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void Constructor_WhenSymbolIsEpsilonAndProductionIsNotNull_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void Constructor_WhenSymbolIsSpecialOtherThanEpsilon_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void GetAttributeValue_WhenNoValueForAttribute_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void GetBindedNode_WhenNodeCannotBeFound_ShouldThrowException()
        {
            Assert.Fail();
        }
    }
}
