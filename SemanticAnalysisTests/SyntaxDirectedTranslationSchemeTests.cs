using SemanticAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysisTests
{
    public class SyntaxDirectedTranslationSchemeTests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void Constructor_WhenStartSymbolIsTerminal_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void Constructor_WhenStartSymbolIsSpecial_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void Constructor_WhenStartSymbolIsNotDefined_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void Constructor_WhenNonterminalIsNotDefined_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void Constructor_WhenNoTerminalInProductions_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void Constructor_WhenProductionsAreEquivalent_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void Constructor_WhenDefinitionIsNotLAttributed_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void FirstOfBody_WhenInputIsEmpty_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void FirstOfBody_WhenInputSymbolIsNotInGrammar_ShouldThrowException()
        {
            Assert.Fail();
        }
    }
}