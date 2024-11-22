using ParseNodeExecption;
using SemanticActionExecption;
using SemanticAnalysis.Attributes;
using SemanticAnalysis.Parsing;
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
            // Arrange
            // Create a symbol to represent the target
            Symbol TPrime = new Symbol("TPrime", SymbolType.Nonterminal);

            // Create a SemanticAttribute (assuming it exists and works with int)
            SemanticAttribute<int> inh = new SemanticAttribute<int>("d", AttributeType.Inherited);

            // Create the binding for the target
            AttributeBinding<int> binding1 = new AttributeBinding<int>(TPrime, 0, inh);

            // Create a HashSet of sources with the target binding
            HashSet<IAttributeBinding> sources = new HashSet<IAttributeBinding> { binding1 };

            // Act & Assert
            Assert.Throws<WhenTargetIsInSourcesException>(() =>
                new SemanticAction(binding1, sources, _ => { })
            );
        }

        [Test]
        public void Execute_WhenNodeSymbolIsNonterminalAndTargetIsNotInNode_ShouldThrowException()
        {
            // Arrange
            Symbol nonTerminalSymbol = new Symbol("A", SymbolType.Nonterminal);
            Production? nullProduction = null;

            // Act & Assert
            Assert.Throws<WhenSymbolIsNonterminalAndProductionIsNullException>(() =>
                new ParseNode(nonTerminalSymbol, nullProduction)
            );
        }

        [Test]
        public void Execute_WhenNodeSymbolIsNonterminalAndSourceIsNotInNode_ShouldThrowException()
        {
            // Arrange
            Symbol nonTerminalSymbol = new Symbol("A", SymbolType.Nonterminal);
            Symbol productionHead = new Symbol("B", SymbolType.Nonterminal);  // La tête de la production est différente du symbole du noeud
            Symbol bodySymbol = new Symbol("C", SymbolType.Terminal);
            Production production = new Production(productionHead, new List<Symbol> { bodySymbol });

            // Act & Assert
            Assert.Throws<WhenSymbolIsNonterminalAndIsNotHeadOfProductionException>(() =>
                new ParseNode(nonTerminalSymbol, production)
            );
        }

        [Test]
        public void Execute_WhenNodeSymbolIsTerminal_ShouldThrowException()
        {
            // Arrange
            Symbol terminalSymbol = new Symbol("a", SymbolType.Terminal);

            Symbol headSymbol = new Symbol("A", SymbolType.Nonterminal);
            Symbol bodySymbol = new Symbol("b", SymbolType.Terminal);
            Production production = new Production(headSymbol, new List<Symbol> { bodySymbol });

            // Act & Assert
            Assert.Throws<WhenSymbolIsTerminalAndProductionIsNotNullException>(() =>
                new ParseNode(terminalSymbol, production)
            );
        }

        [Test]
        public void Execute_WhenNodeSymbolIsSpecial_ShouldThrowException()
        {
            // Arrange
            // Créer un nœud avec ce symbole spécial, sans production
            ParseNode node = new ParseNode(Symbol.EPSILON, null);

            // Créer une action sémantique avec un target et des sources appropriées
            IAttributeBinding target = new AttributeBinding<int>(Symbol.EPSILON, 0, new SemanticAttribute<int>("attr", AttributeType.Inherited));
            IAttributeBinding source = new AttributeBinding<int>(Symbol.EPSILON, 0, new SemanticAttribute<int>("attr", AttributeType.Inherited));
            HashSet<IAttributeBinding> sources = new HashSet<IAttributeBinding> { source };

            SemanticAction action = new SemanticAction(target, sources, _ => { });

            // Act & Assert
            // Vérifier qu'une exception est levée lorsque la méthode Execute est appelée sur le nœud spécial
            Assert.Throws<WhenNodeSymbolIsSpecialException>(() => action.Execute(node));
        }
    }
}
