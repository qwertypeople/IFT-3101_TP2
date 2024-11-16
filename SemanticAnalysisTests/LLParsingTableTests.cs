using SemanticAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysisTests
{
    public class LLParsingTableTests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void Constructor_WhenFirstFirstConflict_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void Constructor_WhenFirstFollowConflict_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void GetProduction_WhenNonterminalIsTerminal_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void GetProduction_WhenNonterminalIsSpecial_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void GetProduction_WhenTerminalIsNonterminal_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void GetProduction_WhenTerminalIsSpecialOtherThanEnd_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void GetProduction_WhenNonterminalIsNotInTable_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void GetProduction_WhenTerminalIsNotInTable_ShouldThrowException()
        {
            Assert.Fail();
        }
    }
}
