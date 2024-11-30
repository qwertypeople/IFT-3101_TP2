using LLParsingTableException;
using SemanticAnalysis;
using SemanticAnalysis.Attributes;
using SemanticAnalysis.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysisTests
{
    // Implémentation minimale pour ISyntaxDirectedTranslationScheme
    public class TestSyntaxDirectedTranslationScheme : ISyntaxDirectedTranslationScheme
    {
        public Symbol StartSymbol { get; }
        public HashSet<Symbol> Terminals { get; }
        public HashSet<Symbol> Nonterminals { get; }
        public Dictionary<Production, List<SemanticAction>> Rules { get; }
        public Dictionary<Symbol, HashSet<Symbol>> First { get; }
        public Dictionary<Symbol, HashSet<Symbol>> Follow { get; }

        public TestSyntaxDirectedTranslationScheme(
            Symbol startSymbol,
            HashSet<Symbol> terminals,
            HashSet<Symbol> nonterminals,
            Dictionary<Production, List<SemanticAction>> rules,
            Dictionary<Symbol, HashSet<Symbol>> first,
            Dictionary<Symbol, HashSet<Symbol>> follow)
        {
            StartSymbol = startSymbol;
            Terminals = terminals;
            Nonterminals = nonterminals;
            Rules = rules;
            First = first;
            Follow = follow;
        }

        public HashSet<Symbol> FirstOfBody(List<Symbol> symbols)
        {
            var firstSet = new HashSet<Symbol>();

            foreach (var symbol in symbols)
            {
                if (First.ContainsKey(symbol))
                {
                    firstSet.UnionWith(First[symbol]);
                }

                if (!First[symbol].Contains(Symbol.EPSILON))
                {
                    break;
                }
            }

            return firstSet;
        }
    }

    public class LLParsingTableTests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void Constructor_WhenFirstFirstConflict_ShouldThrowException()
        {
            // Arrange
            // Définition des symboles
            var symbol_S = new Symbol("S", SymbolType.Nonterminal);
            var symbol_A = new Symbol("A", SymbolType.Nonterminal);
            var symbol_B = new Symbol("B", SymbolType.Nonterminal);
            var terminal_a = new Symbol("a", SymbolType.Terminal);

            // Définition des productions
            var production1 = new Production(symbol_S, new List<Symbol> { symbol_A });
            var production2 = new Production(symbol_S, new List<Symbol> { symbol_B });
            var productionA = new Production(symbol_A, new List<Symbol> { terminal_a });
            var productionB = new Production(symbol_B, new List<Symbol> { terminal_a });

            // Implémentation minimale de ISyntaxDirectedTranslationScheme
            var scheme = new TestSyntaxDirectedTranslationScheme(
                startSymbol: symbol_S,
                terminals: new HashSet<Symbol> { terminal_a },
                nonterminals: new HashSet<Symbol> { symbol_S, symbol_A, symbol_B },
                rules: new Dictionary<Production, List<SemanticAction>>
                {
            { production1, new List<SemanticAction>() },
            { production2, new List<SemanticAction>() },
            { productionA, new List<SemanticAction>() },
            { productionB, new List<SemanticAction>() }
                },
                first: new Dictionary<Symbol, HashSet<Symbol>>
                {
            { symbol_A, new HashSet<Symbol> { terminal_a } },
            { symbol_B, new HashSet<Symbol> { terminal_a } }
                },
                follow: new Dictionary<Symbol, HashSet<Symbol>>() // Pas pertinent ici
            );

            // Act & Assert
            Assert.Throws<WhenFirstFirstConflictException>(() =>
            {
                new LLParsingTable(scheme); // Doit lever une exception à cause du conflit
            });
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
