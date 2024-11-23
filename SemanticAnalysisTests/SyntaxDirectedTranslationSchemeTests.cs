using ProductionExecption;
using SemanticAnalysis;
using SemanticAnalysis.Attributes;
using SemanticAnalysis.Parsing;
using SyntaxDirectedTranslationSchemeException;
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
            // Arrange
            Symbol startSymbol = new Symbol("S", SymbolType.Terminal);  // Le symbole de départ est un terminal
            Dictionary<Production, HashSet<SemanticAction>> definition = new Dictionary<Production, HashSet<SemanticAction>>();

            // Act & Assert
            var exception = Assert.Throws<WhenStartSymbolIsTerminalException>(() =>
            {
                new SyntaxDirectedTranslationScheme(startSymbol, definition);
            });     
        }

        [Test]
        public void Constructor_WhenStartSymbolIsSpecial_ShouldThrowException()
        {
            // Arrange
            Symbol startSymbol = new Symbol("S", SymbolType.Special);  // Le symbole de départ est un terminal
            Dictionary<Production, HashSet<SemanticAction>> definition = new Dictionary<Production, HashSet<SemanticAction>>();

            // Act & Assert
            var exception = Assert.Throws<WhenStartSymbolIsSpecialException>(() =>
            {
                new SyntaxDirectedTranslationScheme(startSymbol, definition);
            });
        }

        [Test]
        public void Constructor_WhenStartSymbolIsNotDefined_ShouldThrowException()
        {
            // Arrange
            // Définir un symbole de départ qui n'est pas dans les règles
            Symbol startSymbol = new Symbol("S", SymbolType.Nonterminal);  // Non-terminal S
            Dictionary<Production, HashSet<SemanticAction>> definition = new Dictionary<Production, HashSet<SemanticAction>>();

            // Aucune production ne définit S
            // Ajouter des productions qui ne contiennent pas le symbole de départ
            Production p1 = new Production(new Symbol("A", SymbolType.Nonterminal), new List<Symbol> { new Symbol("a", SymbolType.Terminal) });
            definition.Add(p1, new HashSet<SemanticAction>());

            // Act & Assert
            var exception = Assert.Throws<WhenStartSymbolIsNotDefinedException>(() =>
            {
                new SyntaxDirectedTranslationScheme(startSymbol, definition);
            });
        }

        [Test]
        public void Constructor_WhenNonterminalIsNotDefined_ShouldThrowException()
        {
            // Arrange
            // Définir un symbole non-terminal qui n'est pas défini dans les productions
            Symbol startSymbol = new Symbol("S", SymbolType.Nonterminal);
            Dictionary<Production, HashSet<SemanticAction>> definition = new Dictionary<Production, HashSet<SemanticAction>>();
                                    
            List<Symbol> symbols = new List<Symbol>();
            symbols.Add(new Symbol("a", SymbolType.Terminal));
            symbols.Add(new Symbol("B", SymbolType.Nonterminal));

            Production p1 = new Production(startSymbol, symbols);
            definition.Add(p1, new HashSet<SemanticAction>());

            // Créer un objet de la classe SyntaxDirectedTranslationScheme
            // Le non-terminal "S" n'est pas défini dans les productions
            var exception = Assert.Throws<WhenNonterminalIsNotDefinedException>(() =>
            {
                new SyntaxDirectedTranslationScheme(startSymbol, definition);
            });
        }

        [Test]
        public void Constructor_WhenNoTerminalInProductions_ShouldThrowException()
        {
            // Arrange
            // Définir un symbole non-terminal qui n'est pas défini dans les productions
            Symbol startSymbol = new Symbol("S", SymbolType.Nonterminal);

            List<Symbol> symbols = new List<Symbol>();
            symbols.Add(new Symbol("B", SymbolType.Nonterminal));

            Production p1 = new Production(startSymbol, symbols);

            Dictionary<Production, HashSet<SemanticAction>> definition = new Dictionary<Production, HashSet<SemanticAction>>();
            definition.Add(p1, new HashSet<SemanticAction>());
            // Act & Assert : Vérifiez que l'exception WhenNoTerminalInProductionsException est lancée
            var ex = Assert.Throws<WhenNoTerminalInProductionsException>(() => new SyntaxDirectedTranslationScheme(startSymbol, definition));
        }
    

        [Test]
        public void Constructor_WhenProductionsAreEquivalent_ShouldThrowException()
        {
            // Arrange : Créez des productions équivalentes.
            Symbol startSymbol = new Symbol("S", SymbolType.Nonterminal);

            Production production1 = new Production(startSymbol, new List<Symbol> { new Symbol("a", SymbolType.Terminal) });
            Production production2 = new Production(startSymbol, new List<Symbol> { new Symbol("a", SymbolType.Terminal) });

            //List<Production> productions = new List<Production> { production1, production2 };

            Dictionary<Production, HashSet<SemanticAction>> definition = new Dictionary<Production, HashSet<SemanticAction>>();
            definition.Add(production1, new HashSet<SemanticAction>());
            definition.Add(production2, new HashSet<SemanticAction>());

            // Act & Assert : Vérifiez que l'exception WhenProductionsAreEquivalentException est lancée.
            var ex = Assert.Throws<WhenProductionsAreEquivalentException>(() => new SyntaxDirectedTranslationScheme(startSymbol, definition));
           
        }

        [Test]
        public void Constructor_WhenDefinitionIsNotLAttributed_ShouldThrowException()                                
        {
        //// Arrange
        //Symbol startSymbol = new Symbol("S", SymbolType.Nonterminal);

        //// Create non-terminal A
        //Symbol A = new Symbol("A", SymbolType.Nonterminal);
        //Symbol B = new Symbol("B", SymbolType.Nonterminal);

        //// Create terminal a
        //Symbol a = new Symbol("a", SymbolType.Terminal);

        //// Production1 S -> Aa
        //Production production1 = new Production(startSymbol, new List<Symbol> { A, a });
        //// Production2 A -> a
        //Production production2 = new Production(A, new List<Symbol> { a });

        //// Create the semantic action with incorrect L-attribution
        //// Assuming SemanticAction requires target, sources and action. 
        //// The action is not L-attributed because of incorrect source/target usage.

        //SemanticAttribute<int> inh = new SemanticAttribute<int>("d", AttributeType.Inherited);
        //AttributeBinding<int> binding1 = new AttributeBinding<int>(A, 0, inh); // Target is non-terminal A
        //AttributeBinding<int> binding2 = new AttributeBinding<int>(B, 0, inh); // Target is non-terminal A
        //HashSet<IAttributeBinding> sources1 = new HashSet<IAttributeBinding> { binding2 };
        //HashSet<IAttributeBinding> sources2 = new HashSet<IAttributeBinding> { binding1 };

        //// Creating semantic action with an invalid L-attribution (since a non-terminal target should not have an inherited attribute)            
        //SemanticAction invalidAction1 = new SemanticAction(binding1, sources1, _ => { });
        //SemanticAction invalidAction2 = new SemanticAction(binding2, sources2, _ => { });

        //// Add the production and its semantic action to the grammar definition
        //Dictionary<Production, HashSet<SemanticAction>> definition = new Dictionary<Production, HashSet<SemanticAction>>
        //{
        //    { production1, new HashSet<SemanticAction> { invalidAction1 } },
        //    { production2, new HashSet<SemanticAction> { invalidAction2 } }
        //};

        //// Act & Assert: Ensure that an exception is thrown when the scheme is created
        //Assert.Throws<WhenDefinitionIsNotLAttributedException>(() =>
        //{
        //    // Creating the scheme should trigger the exception
        //    SyntaxDirectedTranslationScheme scheme = new SyntaxDirectedTranslationScheme(startSymbol, definition);
        //});
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

        //Test ajouté
        [Test]
        public void SetSymbols_WhenSymbolIsNullInProduction_ShouldThrowException()
        {
            Assert.Fail();
        }
    }
}