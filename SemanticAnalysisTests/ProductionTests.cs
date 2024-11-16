using SemanticAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysisTests
{
    public class ProductionTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Constructor_WhenHeadIsTerminal_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void Constructor_WhenHeadIsSpecial_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void Constructor_WhenBodyContainsSpecialOtherThanEpsilon_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void Constructor_WhenBodyWithMoreThanOneSymbolContainsEpsilon_ShouldThrowException()
        {
            Assert.Fail();
        }
    }
}
