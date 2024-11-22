using SemanticAnalysis.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenExecption;

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
            // Arrange
            Symbol terminalSymbol = new Symbol("a", SymbolType.Terminal);

            // Act
            Token token = new Token(terminalSymbol, "value");

            // Assert
            Assert.That(token.Symbol, Is.EqualTo(terminalSymbol));
            Assert.That(token.Value, Is.EqualTo("value"));
        }

        [Test]
        public void Constructor_WhenSymbolIsSpecialOtherThanEnd_ShouldThrowException()
        {
            // Arrange
            Symbol endSymbol = Symbol.END;

            // Act
            Token token = new Token(endSymbol, "value");

            // Assert
            Assert.That(token.Symbol, Is.EqualTo(endSymbol));
            Assert.That(token.Value, Is.EqualTo("value"));
        }
    }
}
