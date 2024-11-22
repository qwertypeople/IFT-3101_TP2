using ProductionExecption;
using SemanticAnalysis;
using SemanticAnalysis.Parsing;
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
            // Arrange
            Symbol terminalHead = new Symbol("a", SymbolType.Terminal);

            // Act & Assert
            Assert.Throws<WhenHeadIsTerminalException>(
                () => new Production(terminalHead, new List<Symbol>())
            );
        }

        [Test]
        public void Constructor_WhenHeadIsSpecial_ShouldThrowException()
        {
            // Arrange
            Symbol specialHead = new Symbol("special", SymbolType.Special);

            // Act & Assert
            Assert.Throws<WhenHeadIsSpecialException>(
                () => new Production(specialHead, new List<Symbol>())
            );
        }

        [Test]
        public void Constructor_WhenBodyContainsSpecialOtherThanEpsilon_ShouldThrowException()
        {
            // Arrange
            Symbol head = new Symbol("A", SymbolType.Nonterminal);
            List<Symbol> body = new List<Symbol>
            {
                new Symbol("special", SymbolType.Special),
                new Symbol("b", SymbolType.Terminal)
            };

            // Act & Assert
            Assert.Throws<WhenBodyContainsSpecialOtherThanEpsilonException>(
                () => new Production(head, body)
            );
        }

        [Test]
        public void Constructor_WhenBodyWithMoreThanOneSymbolContainsEpsilon_ShouldThrowException()
        {
            // Arrange : Crée un symbole de tête valide et un corps contenant epsilon avec d'autres symboles
            Symbol head = new Symbol("A", SymbolType.Nonterminal);
            List<Symbol> body = new List<Symbol>
            {
                Symbol.EPSILON,
                new Symbol("b", SymbolType.Terminal) // Un autre symbole présent
            };

            // Act & Assert : Vérifie que l'exception attendue est levée
            Assert.Throws<WhenBodyWithMoreThanOneSymbolContainsEpsilonException>(
                () => new Production(head, body)
            );
        }
    }
}
