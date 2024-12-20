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
            // Arrange
            // Create symbols for the grammar S → A a, A → a | ε
            var symbol_S = new Symbol("S", SymbolType.Nonterminal);
            var symbol_A = new Symbol("A", SymbolType.Nonterminal);
            var terminal_a = new Symbol("a", SymbolType.Terminal);

            // Create productions
            var production_S = new Production(symbol_S, new List<Symbol> { symbol_A, terminal_a }); // S → A a
            var production_A1 = new Production(symbol_A, new List<Symbol> { terminal_a });          // A → a
            var production_A2 = new Production(symbol_A, new List<Symbol> { Symbol.EPSILON });      // A → ε

            // Create rules dictionary
            var rules = new Dictionary<Production, HashSet<SemanticAction>>
            {
                { production_S, new HashSet<SemanticAction>() },
                { production_A1, new HashSet<SemanticAction>() },
                { production_A2, new HashSet<SemanticAction>() }
            };

            // Create scheme
            var scheme = new SyntaxDirectedTranslationScheme(symbol_S, rules);

            // Set First and Follow sets
            // First(A) = {a, ε}
            scheme.First[symbol_A] = new HashSet<Symbol> { terminal_a, Symbol.EPSILON };
            // Follow(A) = {a} because A is followed by 'a' in S → A a
            scheme.Follow[symbol_A] = new HashSet<Symbol> { terminal_a };

            // Act & Assert
            var exception = Assert.Throws<WhenFirstFollowConflictException>(() => new LLParsingTable(scheme));

            // Verify exception details
            Assert.Multiple(() =>
            {
                Assert.That(exception.NonTerminal, Is.EqualTo(symbol_A), "Exception should reference non-terminal A");
                Assert.That(exception.ConflictingProduction, Is.EqualTo(production_A2), "Exception should reference A → ε production");
                Assert.That(exception.ConflictingSymbol, Is.EqualTo(terminal_a), "Exception should reference terminal 'a'");
            });
        }

        [Test]
        public void GetProduction_WhenNonterminalIsTerminal_ShouldThrowException()
        {
            // Arrange
            var symbol_S = new Symbol("S", SymbolType.Nonterminal);
            var terminal_a = new Symbol("a", SymbolType.Terminal);
            
            var production = new Production(symbol_S, new List<Symbol> { terminal_a });
            var rules = new Dictionary<Production, HashSet<SemanticAction>>
            {
                { production, new HashSet<SemanticAction>() }
            };

            var scheme = new SyntaxDirectedTranslationScheme(symbol_S, rules);
            var table = new LLParsingTable(scheme);

            // Act & Assert
            // Try to get production with terminal 'a' as nonterminal parameter
            var exception = Assert.Throws<WhenNonterminalIsTerminalException>(
                () => table.GetProduction(terminal_a, terminal_a)
            );

            // Verify the exception contains the correct terminal symbol
            Assert.That(exception.TerminalSymbol, Is.EqualTo(terminal_a));
        }

        [Test]
        public void GetProduction_WhenNonterminalIsSpecial_ShouldThrowException()
        {
            // Arrange
            var symbol_S = new Symbol("S", SymbolType.Nonterminal);
            var terminal_a = new Symbol("a", SymbolType.Terminal);
            var special_symbol = Symbol.EPSILON;  // EPSILON is a special symbol
            
            var production = new Production(symbol_S, new List<Symbol> { terminal_a });
            var rules = new Dictionary<Production, HashSet<SemanticAction>>
            {
                { production, new HashSet<SemanticAction>() }
            };

            var scheme = new SyntaxDirectedTranslationScheme(symbol_S, rules);
            var table = new LLParsingTable(scheme);

            // Act & Assert
            // Try to get production with EPSILON (special symbol) as nonterminal parameter
            var exception = Assert.Throws<WhenNonterminalIsSpecialException>(
                () => table.GetProduction(special_symbol, terminal_a)
            );

            // Verify the exception contains the correct special symbol
            Assert.That(exception.SpecialSymbol, Is.EqualTo(special_symbol));
        }

        [Test]
        public void GetProduction_WhenTerminalIsNonterminal_ShouldThrowException()
        {
            // Arrange
            var symbol_S = new Symbol("S", SymbolType.Nonterminal);
            var symbol_A = new Symbol("A", SymbolType.Nonterminal);  // This will be incorrectly used as terminal
            var terminal_a = new Symbol("a", SymbolType.Terminal);

            var production = new Production(symbol_S, new List<Symbol> { terminal_a });
            var rules = new Dictionary<Production, HashSet<SemanticAction>>
            {
                { production, new HashSet<SemanticAction>() }
            };

            var scheme = new SyntaxDirectedTranslationScheme(symbol_S, rules);
            var table = new LLParsingTable(scheme);

            // Act & Assert
            // Try to get production with non-terminal 'A' as terminal parameter
            var exception = Assert.Throws<WhenTerminalIsNonterminalException>(
                () => table.GetProduction(symbol_S, symbol_A)
            );

            // Verify the exception contains the correct non-terminal symbol
            Assert.That(exception.NonTerminalSymbol, Is.EqualTo(symbol_A));
        }

        [Test]
        public void GetProduction_WhenTerminalIsSpecialOtherThanEnd_ShouldThrowException()
        {
            // Arrange
            var symbol_S = new Symbol("S", SymbolType.Nonterminal);
            var terminal_a = new Symbol("a", SymbolType.Terminal);
            var special_epsilon = Symbol.EPSILON;
            var special_end = Symbol.END;

            // Create two productions:
            // S → a
            var production1 = new Production(symbol_S, new List<Symbol> { terminal_a });
            // S → ε (this will cause 'end' to be added from Follow set)
            var production2 = new Production(symbol_S, new List<Symbol> { Symbol.EPSILON });
            
            var scheme = new TestSyntaxDirectedTranslationScheme(
                startSymbol: symbol_S,
                terminals: new HashSet<Symbol> { terminal_a, special_end },  // Include 'end' in terminals
                nonterminals: new HashSet<Symbol> { symbol_S },
                rules: new Dictionary<Production, List<SemanticAction>>
                {
                    { production1, new List<SemanticAction>() },
                    { production2, new List<SemanticAction>() }
                },
                first: new Dictionary<Symbol, HashSet<Symbol>>
                {
                    { symbol_S, new HashSet<Symbol> { terminal_a, Symbol.EPSILON } },  // S can derive epsilon
                    { terminal_a, new HashSet<Symbol> { terminal_a } },
                    { special_end, new HashSet<Symbol> { special_end } },
                    { Symbol.EPSILON, new HashSet<Symbol> { Symbol.EPSILON } }  // Add First set for epsilon
                },
                follow: new Dictionary<Symbol, HashSet<Symbol>>
                {
                    { symbol_S, new HashSet<Symbol> { special_end } }  // Add 'end' to follow set
                }
            );

            var table = new LLParsingTable(scheme);

            // Act & Assert
            // 1. Verify that special symbol 'end' is allowed
            Assert.DoesNotThrow(() => table.GetProduction(symbol_S, special_end),
                "GetProduction should allow the special symbol 'end'");

            // 2. Verify that other special symbols throw an exception
            var exception = Assert.Throws<WhenTerminalIsSpecialOtherThanEndException>(() => 
                table.GetProduction(symbol_S, special_epsilon));

            // 3. Verify the exception contains the correct special symbol
            Assert.That(exception.Terminal, Is.EqualTo(Symbol.EPSILON),
                "Exception should contain the special symbol that caused it");
        }

        [Test]
        public void GetProduction_WhenNonterminalIsNotInTable_ShouldThrowException()
        {
            // Arrange
            var symbol_S = new Symbol("S", SymbolType.Nonterminal);
            var symbol_A = new Symbol("A", SymbolType.Nonterminal); // This one won't be in the table
            var terminal_a = new Symbol("a", SymbolType.Terminal);

            var production = new Production(symbol_S, new List<Symbol> { terminal_a });
            
            var scheme = new TestSyntaxDirectedTranslationScheme(
                startSymbol: symbol_S,
                terminals: new HashSet<Symbol> { terminal_a },
                nonterminals: new HashSet<Symbol> { symbol_S },  // Only S is in the grammar
                rules: new Dictionary<Production, List<SemanticAction>>
                {
                    { production, new List<SemanticAction>() }
                },
                first: new Dictionary<Symbol, HashSet<Symbol>>
                {
                    { symbol_S, new HashSet<Symbol> { terminal_a } },
                    { terminal_a, new HashSet<Symbol> { terminal_a } },
                    { Symbol.END, new HashSet<Symbol> { Symbol.END } }
                },
                follow: new Dictionary<Symbol, HashSet<Symbol>>
                {
                    { symbol_S, new HashSet<Symbol> { Symbol.END } }
                }
            );

            var table = new LLParsingTable(scheme);

            // Act & Assert
            var exception = Assert.Throws<WhenNonterminalIsNotInTableException>(() => 
                table.GetProduction(symbol_A, terminal_a));

            // Verify the exception contains the correct nonterminal
            Assert.That(exception.Nonterminal.Name, Is.EqualTo("A"),
                "Exception should contain the nonterminal that was not in the table");
        }

        [Test]
        public void GetProduction_WhenTerminalIsNotInTable_ShouldThrowException()
        {
            // Arrange
            var symbol_S = new Symbol("S", SymbolType.Nonterminal);
            var terminal_a = new Symbol("a", SymbolType.Terminal);
            var terminal_b = new Symbol("b", SymbolType.Terminal); // This one won't be in the table

            var production = new Production(symbol_S, new List<Symbol> { terminal_a });
            
            var scheme = new TestSyntaxDirectedTranslationScheme(
                startSymbol: symbol_S,
                terminals: new HashSet<Symbol> { terminal_a },  // Only 'a' is in the grammar
                nonterminals: new HashSet<Symbol> { symbol_S },
                rules: new Dictionary<Production, List<SemanticAction>>
                {
                    { production, new List<SemanticAction>() }
                },
                first: new Dictionary<Symbol, HashSet<Symbol>>
                {
                    { symbol_S, new HashSet<Symbol> { terminal_a } },
                    { terminal_a, new HashSet<Symbol> { terminal_a } },
                    { terminal_b, new HashSet<Symbol> { terminal_b } },
                    { Symbol.END, new HashSet<Symbol> { Symbol.END } }
                },
                follow: new Dictionary<Symbol, HashSet<Symbol>>
                {
                    { symbol_S, new HashSet<Symbol> { Symbol.END } }
                }
            );

            var table = new LLParsingTable(scheme);

            // Act & Assert
            var exception = Assert.Throws<WhenTerminalIsNotInTableException>(() => 
                table.GetProduction(symbol_S, terminal_b));

            // Verify the exception contains the correct symbols
            Assert.Multiple(() =>
            {
                Assert.That(exception.Terminal.Name, Is.EqualTo("b"),
                    "Exception should contain the terminal that was not in the table");
                Assert.That(exception.Nonterminal.Name, Is.EqualTo("S"),
                    "Exception should contain the nonterminal we were looking up");
            });
        }
    }
}
