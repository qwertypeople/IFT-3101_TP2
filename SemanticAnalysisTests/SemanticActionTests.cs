using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysisTests
{
    public class SemanticActionTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Constructor_WhenTargetIsInSources_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void Execute_WhenNodeSymbolIsNonterminalAndTargetIsNotInNode_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void Execute_WhenNodeSymbolIsNonterminalAndSourceIsNotInNode_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void Execute_WhenNodeSymbolIsTerminal_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void Execute_WhenNodeSymbolIsSpecial_ShouldThrowException()
        {
            Assert.Fail();
        }
    }
}
