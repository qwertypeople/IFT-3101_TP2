using SemanticAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysisTests
{
    public class TopDownParserTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Parse_WhenEndTokenIsNotAtEndOfInput_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void Parse_WhenCannotMatchInput_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void Parse_WhenNoProductionDefined_ShouldThrowException()
        {
            Assert.Fail();
        }
    }
}
