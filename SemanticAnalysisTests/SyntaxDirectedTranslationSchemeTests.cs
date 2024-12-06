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
            // Arrange: Création des symboles
            Symbol symbol_B = new Symbol("B", SymbolType.Nonterminal);
            Symbol symbol_C = new Symbol("C", SymbolType.Nonterminal);
            Symbol symbol_a = new Symbol("a", SymbolType.Terminal);

            // Tête de la production
            Symbol startSymbol = new Symbol("S", SymbolType.Nonterminal);

            // Corps de la production principale
            var body1 = new List<Symbol> { symbol_B, symbol_C, symbol_a };
            var body2 = new List<Symbol> { symbol_a };
            var body3 = new List<Symbol> { symbol_a };

            // Définition des productions
            Production production1 = new Production(startSymbol, body1);
            Production production2 = new Production(symbol_B, body2);
            Production production3 = new Production(symbol_C, body3);


            // Dictionnaire des règles
            var definition = new Dictionary<Production, HashSet<SemanticAction>>();

            // Créer un attribut hérité pour B (cible) et une source sur C (après B)
            var inh = new SemanticAttribute<int>("a", AttributeType.Inherited);
            var binding1 = new AttributeBinding<int>(symbol_B, 0, inh); // Cible : B
            var binding2 = new AttributeBinding<int>(symbol_C, 0, inh); // Source : C

            // Action sémantique non L-attribuée (source C après B)
            var action = new SemanticAction(
                target: binding1,
                sources: new HashSet<IAttributeBinding> { binding2 },
                action: _ => { } // Simule une action
            );

            // Associer l'action à la production principale            
            definition.Add(production1, new HashSet<SemanticAction> { action });
            definition.Add(production2, new HashSet<SemanticAction>());
            definition.Add(production3, new HashSet<SemanticAction>());
            

            // Act & Assert: Vérification que l'exception est levée
            Assert.Throws<WhenDefinitionIsNotLAttributedException>(() =>
            {
                // Initialisation de la grammaire qui vérifie les contraintes L-attribuées
                new SyntaxDirectedTranslationScheme(startSymbol, definition);
            });
        }
 
        [Test]
        public void FirstOfBody_WhenInputIsEmpty_ShouldThrowException()
        {
            // Arrange
            Symbol startSymbol = new Symbol("S", SymbolType.Nonterminal);
            Production production1 = new Production(startSymbol, new List<Symbol> { new Symbol("a", SymbolType.Terminal) });

            Dictionary<Production, HashSet<SemanticAction>> definition = new Dictionary<Production, HashSet<SemanticAction>>();
            definition.Add(production1, new HashSet<SemanticAction>());

            SyntaxDirectedTranslationScheme scheme = new SyntaxDirectedTranslationScheme(startSymbol, definition);

            // Act & Assert
            Assert.Throws<WhenInputIsEmptyException>(() =>
            {
                scheme.FirstOfBody(new List<Symbol>()); // Liste vide
            });
        }

        [Test]
        public void FirstOfBody_WhenInputSymbolIsNotInGrammar_ShouldThrowException()
        {
            // Arrange
            Symbol startSymbol = new Symbol("S", SymbolType.Nonterminal);
            Production production1 = new Production(startSymbol, new List<Symbol> { new Symbol("a", SymbolType.Terminal) });
            Dictionary<Production, HashSet<SemanticAction>> definition = new Dictionary<Production, HashSet<SemanticAction>>();
            definition.Add(production1, new HashSet<SemanticAction>());
            SyntaxDirectedTranslationScheme scheme = new SyntaxDirectedTranslationScheme(startSymbol, definition);

            // Ajouter quelques symboles valides dans la grammaire
            scheme.Terminals.Add(new Symbol("a", SymbolType.Terminal));
            scheme.Nonterminals.Add(new Symbol("A", SymbolType.Nonterminal));

            // Créer une liste avec un symbole invalide (non présent dans la grammaire)
            var invalidSymbol = new Symbol("X", SymbolType.Nonterminal);
            var symbols = new List<Symbol> { invalidSymbol };

            // Act & Assert
            Assert.Throws<WhenInputSymbolIsNotInGrammarException>(() =>
            {
                scheme.FirstOfBody(symbols); // Liste contenant un symbole invalide
            });
        }

        ////Test ajouté
        //[Test]
        //public void SetSymbols_WhenSymbolIsNullInProduction_ShouldThrowException()
        //{
        //    Assert.Fail();
        //}
    }
}