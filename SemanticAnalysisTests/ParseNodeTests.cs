using ParseNodeExecption;
using SemanticAnalysis.Attributes;
using SemanticAnalysis.Parsing;
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
            // Arrange
            Symbol nonTerminalSymbol = new Symbol("A", SymbolType.Nonterminal);

            // Production est null
            Production? production = null;

            // Act & Assert
            // Vérifier qu'une exception est levée lorsque la production est null pour un symbole non-terminal
            Assert.Throws<WhenSymbolIsNonterminalAndProductionIsNullException>(() =>
                new ParseNode(nonTerminalSymbol, production)
            );
        }

        [Test]
        public void Constructor_WhenSymbolIsNonterminalAndIsNotHeadOfProduction_ShouldThrowException()
        {
            // Arrange
            Symbol nonterminalSymbol = new Symbol("A", SymbolType.Nonterminal);
            Symbol anotherNonterminalSymbol = new Symbol("B", SymbolType.Nonterminal);

            // Créer une production avec un non-terminal "A" comme tête et un autre symbole "B" dans le corps
            Production production = new Production(nonterminalSymbol, new List<Symbol> { anotherNonterminalSymbol });

            // Act & Assert
            // Vérifier qu'une exception est levée si le symbole du nœud n'est pas la tête de la production
            Assert.Throws<WhenSymbolIsNonterminalAndIsNotHeadOfProductionException>(() =>
                new ParseNode(anotherNonterminalSymbol, production)
            );
        }

        [Test]
        public void Constructor_WhenSymbolIsTerminalAndProductionIsNotNull_ShouldThrowException()
        {
            // Arrange
            Symbol terminalSymbol = new Symbol("a", SymbolType.Terminal); // Créer un symbole terminal
            Production production = new Production(new Symbol("A", SymbolType.Nonterminal), new List<Symbol> { terminalSymbol }); // Créer une production non-null

            // Act & Assert
            // Vérifier qu'une exception est levée lorsque le symbole est terminal et qu'il y a une production non-null
            Assert.Throws<WhenSymbolIsTerminalAndProductionIsNotNullException>(() =>
                new ParseNode(terminalSymbol, production)
            );
        }

        [Test]
        public void Constructor_WhenSymbolIsEpsilonAndProductionIsNotNull_ShouldThrowException()
        {
            // Arrange
            Symbol nonTerminalSymbol = new Symbol("A", SymbolType.Nonterminal);

            // Production est null
            Production? production = null;

            // Act & Assert
            // Vérifier qu'une exception est levée lorsque la production est null pour un symbole non-terminal
            Assert.Throws<WhenSymbolIsNonterminalAndProductionIsNullException>(() =>
                new ParseNode(nonTerminalSymbol, production)
            );
        }

        [Test]
        public void Constructor_WhenSymbolIsSpecialOtherThanEpsilon_ShouldThrowException()
        {
            // Arrange
            Symbol specialSymbol = new Symbol("special", SymbolType.Special);
            Production? production = null;  // La production peut être null car c'est un symbole spécial

            // Act & Assert
            // Vérifier qu'une exception est levée lorsque le symbole est spécial et n'est pas epsilon
            Assert.Throws<WhenSymbolIsSpecialOtherThanEpsilonException>(() =>
                new ParseNode(specialSymbol, production)
            );
        }

        [Test]
        public void GetAttributeValue_WhenNoValueForAttribute_ShouldThrowException()
        {
            // Arrange
            // Créer un symbole (par exemple, un non-terminal)
            Symbol symbol = new Symbol("a", SymbolType.Terminal);

            // Créer un attribut (avec un type et un nom, par exemple "Inherited")
            SemanticAttribute<int> attribute = new SemanticAttribute<int>("attr", AttributeType.Inherited);

            // Créer un nœud (ParseNode) sans valeur pour cet attribut
            ParseNode node = new ParseNode(symbol, null); // Production est null, pas d'attribut défini

            // Act & Assert
            // Vérifier qu'une exception est levée lorsqu'on tente d'obtenir la valeur sans valeur définie
            Assert.Throws<WhenNoValueForAttributeException>(() =>
            {
                // Appeler une méthode de récupération de valeur pour l'attribut
                node.GetAttributeValue(attribute);  // Supposons que cette méthode est implémentée dans ParseNode
            });
        }

        [Test]
        public void GetBindedNode_WhenNodeCannotBeFound_ShouldThrowException()
        {
            // Arrange
            // Crée un symbole et un nœud de parse associé
            Symbol headSymbol = new Symbol("A", SymbolType.Nonterminal);
            List<Symbol> bodySymbols = new List<Symbol>
            {
                new Symbol("B", SymbolType.Terminal),
                new Symbol("C", SymbolType.Terminal)
            };
            Production production = new Production(headSymbol, bodySymbols);
            ParseNode node = new ParseNode(headSymbol, production);  // Le nœud n'a pas de production associée

            Symbol childSymbol = new Symbol("B", SymbolType.Terminal);
            ParseNode childNode = new ParseNode(childSymbol, null);  // Nœud enfant de type terminal
            

            // Crée un binding pour un attribut qui correspond à un symbole qui n'existe pas dans l'arbre
            IAttributeBinding binding = new AttributeBinding<int>(new Symbol("C", SymbolType.Nonterminal), 0, new SemanticAttribute<int>("attr", AttributeType.Inherited));

            // Act & Assert
            // Appelle GetBindedNode avec un binding qui ne correspond à aucun nœud dans l'arbre
            Assert.Throws<WhenNodeCannotBeFoundException>(() =>
            {
                node.GetBindedNode(binding); // Recherche un nœud avec un symbole inexistant
            });
        }
    }
}
